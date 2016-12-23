using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.WoWInternals;

namespace WorldQuestSettings.GroupFinder
{
    internal class WQGF
    {
        private const string WqgfComment = "WorldQuestGroupFinder User";
        private static uint _currentQuestId;
        private static DateTime _lastSearchTime = DateTime.MinValue;
        private static WaitTimer _leaveTimer;
        private static bool Setting => Settings.Instance.WQGF;

        public static void AttachEvent()
        {
            Lua.Events.AttachEvent("LFG_LIST_SEARCH_RESULT_UPDATED", SearchResultsUpdated);
            Lua.Events.AttachEvent("LFG_LIST_SEARCH_RESULTS_RECEIVED", ResultsReceived);
            Lua.Events.AttachEvent("LFG_LIST_APPLICANT_UPDATED", AppliationUpdated);
            Lua.Events.AttachEvent("QUEST_TURNED_IN", QuestTurnedIn);
            BotEvents.Profile.OnNewProfileLoaded += Profile_OnNewProfileLoaded;
        }


        public static void DetachEvent()
        {
            Lua.Events.DetachEvent("LFG_LIST_SEARCH_RESULT_UPDATED", SearchResultsUpdated);
            Lua.Events.DetachEvent("LFG_LIST_SEARCH_RESULTS_RECEIVED", ResultsReceived);
            Lua.Events.DetachEvent("LFG_LIST_APPLICANT_UPDATED", AppliationUpdated);
            Lua.Events.DetachEvent("QUEST_TURNED_IN", QuestTurnedIn);
            BotEvents.Profile.OnNewProfileLoaded -= Profile_OnNewProfileLoaded;
        }

        private static void QuestTurnedIn(object sender, LuaEventArgs args)
        {
            if (!Setting) return;
            Log("QUEST_TURNED_IN");
            var id = Lua.ParseLuaValue<uint>(args.Args[0].ToString());
            if (id != _currentQuestId) return;
            var rnd = new Random();
            _leaveTimer = new WaitTimer(TimeSpan.FromSeconds(rnd.Next(3, 10)));
            _leaveTimer.Reset();
            Log($"Quest Completed {id} leaving group in {_leaveTimer.TimeLeft.TotalSeconds}");
            _currentQuestId = 0;
        }

        private static void AppliationUpdated(object sender, LuaEventArgs args)
        {
            Log("LFG_LIST_APPLICANT_UPDATED");
            if (!LfgList.IsGroupLeader)
            {
                Log("Your not the group leader");
                return;
            }
            var currentMembers = StyxWoW.Me.GroupInfo.NumPartyMembers;
            var pendingInvited = LfgList.GetNumInvitedApplicantMembers;
            Log(
                $"Your the group leader, you currently have {currentMembers}/5 members with {pendingInvited} pending invites");
            var canInvite = 5 - currentMembers - pendingInvited;
            if (canInvite == 0) return;
            var applicants = LfgList.GetApplicants;

            foreach (var a in applicants)
            {
                if (canInvite == 0) break;
                if (a.Comment != WqgfComment || a.Status != ApplicationState.Applied) continue;
                LfgList.InviteApplicant(a.Id);
                canInvite--;
            }
        }

        private static void Profile_OnNewProfileLoaded(BotEvents.Profile.NewProfileLoadedEventArgs args)
        {
            var file = Path.GetFileNameWithoutExtension(args.NewProfile.Path);
            if (file != null)
            {
                var quest = file.Substring(0, 5);
                if (!uint.TryParse(quest, out _currentQuestId)) return;
                Log($"Current quest id set too {_currentQuestId}");
            }
        }

        private static void ResultsReceived(object sender, LuaEventArgs args)
        {
            if (!Setting) return;
            Log("LFG_LIST_SEARCH_RESULTS_RECEIVED");

            var results = LfgList.GetSearchResults;
            Log($"Found {results.Count} Results");

            foreach (var r in results)
            {
                if (r.Comment == null || !r.Comment.Contains($"#WQ:{_currentQuestId}")) continue;
                if (r.IsDelisted || r.PlayerCount > 4) continue;
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
            Logging.WriteDiagnostic(Colors.GreenYellow, "WQGF: " + format, args);
        }

        public static void Pulse()
        {
            if (!Setting) return;
            if (StyxWoW.Me.GroupInfo.IsInParty && StyxWoW.Me.GroupInfo.NumPartyMembers == 1)
            {
                Log("Leaving group were the only one in it :(");
                LfgList.LeaveGroup();
                return;
            }

            if (_leaveTimer != null && _leaveTimer.IsFinished)
            {
                Log("Leaving Group");
                LfgList.LeaveGroup();
                _leaveTimer = null;
                return;
            }

            if (_currentQuestId == 0) return;
            if (!StyxWoW.Me.QuestLog.ContainsQuest(_currentQuestId))
                return;

            if (StyxWoW.Me.GroupInfo.IsInParty || StyxWoW.Me.GroupInfo.IsInRaid) return;
            var diff = DateTime.Now.Subtract(_lastSearchTime).TotalSeconds;
            if (diff < 15) return;
            Log("Starting a new search last timed out");
            LfgList.Search(LFGCategory.Questing, $"#WQ:{_currentQuestId}");
            _lastSearchTime = DateTime.Now;
        }
    }
}