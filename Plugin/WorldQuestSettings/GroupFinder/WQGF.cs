using System;
using System.Linq;
using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.WoWInternals;

namespace WorldQuestSettings.GroupFinder
{
    internal class WQGF
    {
        private const string WqgfComment = "WorldQuestGroupFinder User";
        private static uint _currentQuestId;
        private static DateTime _lastSearchTime = DateTime.MinValue;
        private static bool Setting => Settings.Instance.WQGF;

        public static void AttachEvent()
        {
            Lua.Events.AttachEvent("LFG_LIST_SEARCH_RESULT_UPDATED", SearchResultsUpdated);
            Lua.Events.AttachEvent("LFG_LIST_SEARCH_RESULTS_RECEIVED", ResultsReceived);
            Lua.Events.AttachEvent("QUEST_ACCEPTED", QuestAccepted);
            Lua.Events.AttachEvent("QUEST_TURNED_IN", QuestTurnedIn);
        }

        public static void DetachEvent()
        {
            Lua.Events.DetachEvent("LFG_LIST_SEARCH_RESULT_UPDATED", SearchResultsUpdated);
            Lua.Events.DetachEvent("LFG_LIST_SEARCH_RESULTS_RECEIVED", ResultsReceived);
            Lua.Events.DetachEvent("QUEST_ACCEPTED", QuestAccepted);
            Lua.Events.DetachEvent("QUEST_TURNED_IN", QuestTurnedIn);
        }

        private static void QuestTurnedIn(object sender, LuaEventArgs args)
        {
            if(!Setting) return;
            Log("QUEST_TURNED_IN");
            var id = Lua.ParseLuaValue<uint>(args.Args[0].ToString());
            if (id != _currentQuestId) return;
            Log($"Quest Completed {id} leaving group");
            LfgList.LeaveGroup();
            _currentQuestId = 0;
        }

        private static void QuestAccepted(object sender, LuaEventArgs args)
        {
            if (!Setting) return;
            Log("QUEST_ACCEPTED");
            if (args.Args.Length == 0) return;
            if (args.Args[1] == null) return;
            var id = Lua.ParseLuaValue<uint>(args.Args[1].ToString());
            if (!IsWorldQuest(id)) return;

            _currentQuestId = id;
            Log($"Current Objective Id Set To {id}");
            LfgList.Search(LFGCategory.Questing, $"#WQ:{_currentQuestId}");
            _lastSearchTime = DateTime.Now;
        }

        private static bool IsWorldQuest(uint questId)
            => Lua.GetReturnVal<bool>($"return QuestUtils_IsQuestWorldQuest({questId})", 0);

        private static void ResultsReceived(object sender, LuaEventArgs args)
        {
            if (!Setting) return;
            Log("LFG_LIST_SEARCH_RESULTS_RECEIVED");

            var results = LfgList.GetSearchResults;
            Log($"Found {results.Count} Results");

            foreach (var r in results)
            {
                if ((r.Comment == null) || !r.Comment.Contains($"#WQ:{_currentQuestId}")) continue;
                if (r.IsDelisted || (r.PlayerCount > 4)) continue;
                Log($"Applying to {r}");
                r.ApplyToGroup(WqgfComment);
            }
        }

        private static void SearchResultsUpdated(object sender, LuaEventArgs args)
        {
            if (!Setting) return;
            Log("LFG_LIST_SEARCH_RESULT_UPDATED");
            var applications = LfgList.GetApplicationInfos;

            if (applications.Any(a => a.Status == ApplicationState.Invited))
                LfgList.AcceptPopUps();
        }


        private static void Log(string format, params object[] args)
        {
            Logging.WriteDiagnostic(Colors.Goldenrod, "WQGF: " + format, args);
        }

        public static void Pulse()
        {
            if (!Setting) return;
            if (_currentQuestId == 0) return;
            if (!StyxWoW.Me.QuestLog.ContainsQuest(_currentQuestId))
            {
                Log("No longer have quest in log leaving group");
                _currentQuestId = 0;         
                LfgList.LeaveGroup();
                return;
            }

            if (StyxWoW.Me.GroupInfo.IsInParty && StyxWoW.Me.GroupInfo.NumPartyMembers == 1)
            {
                Log("Leaving group were the only one in it :(");
                LfgList.LeaveGroup();
                return;
            }

            if (StyxWoW.Me.GroupInfo.IsInParty || StyxWoW.Me.GroupInfo.IsInRaid) return;
            var diff = DateTime.Now.Subtract(_lastSearchTime).TotalSeconds;
            if (diff < 30) return;
            Log("Starting a new search last timed out");
            LfgList.Search(LFGCategory.Questing, $"#WQ:{_currentQuestId}");
            _lastSearchTime = DateTime.Now;
        }
    }
}