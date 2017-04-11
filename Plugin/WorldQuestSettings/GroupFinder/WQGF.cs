using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.WoWInternals;
using WorldQuestSettings.WorldQuestSettings.GroupFinder;

namespace WorldQuestSettings.GroupFinder
{
    internal class WQGF
    {
        private static string WqgfComment => "WorldQuestGroupFinderUser-"+_currentQuestId;
        private static uint _currentQuestId;
        private static string _currentQuestName;
        private static bool _currentIsRaidQuest;
        private static bool _currentIsBlacklisted;
        private static DateTime _lastSearchTime = DateTime.MinValue;
        private static WaitTimer _leaveTimer;
        private static bool Setting => Settings.Instance.WQGF;

        private static string GetQuestName(string questId)
            => Lua.GetReturnVal<string>($"return C_TaskQuest.GetQuestInfoByQuestID({questId})", 0);

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

            var min = Settings.Instance.WQGFMin;
            var max = Settings.Instance.WQGFMax;
            if (min > max)
            {
                Log($"WARNING Max ({max}) leave time is less than min ({min}) leave time");
                max = min + max;
            }

            _leaveTimer = new WaitTimer(TimeSpan.FromSeconds(rnd.Next(min, max)));
            _leaveTimer.Reset();
            Log($"Quest Completed {_currentQuestName} leaving group in {_leaveTimer.TimeLeft.TotalSeconds}");
            _currentQuestId = 0;
            _currentIsBlacklisted = true;
            _currentIsRaidQuest = false;
            _currentQuestName = "Not Set";
        }

        private static void AppliationUpdated(object sender, LuaEventArgs args)
        {
            if (!Setting) return;
            Log("LFG_LIST_APPLICANT_UPDATED");
            if (!LfgList.IsGroupLeader)
            {
                Log("Your not the group leader");
                return;
            }
            var currentMembers = StyxWoW.Me.GroupInfo.NumPartyMembers;
            var pendingInvited = LfgList.GetNumInvitedApplicantMembers;
            if (!_currentIsRaidQuest)
                Log($"Your the group leader, you currently have {currentMembers}/5 members with {pendingInvited} pending invites");


            var canInvite = 5 - currentMembers - pendingInvited;
            if (canInvite <= 0 && !_currentIsRaidQuest) return;
            var applicants = LfgList.GetApplicants;

            foreach (var a in applicants)
            {
                if (canInvite <= 0 && !_currentIsRaidQuest)
                {
                    Log($"Group Full CanInvite = {canInvite}");
                    return;
                }
                if (a.Comment != WqgfComment || a.Status != ApplicationState.Applied) continue;
                LfgList.InviteApplicant(a.Id);
                canInvite--;
                Log($"Invited {a.ApplicantId}");
            }
        }

        private static void Profile_OnNewProfileLoaded(BotEvents.Profile.NewProfileLoadedEventArgs args)
        {
            var file = Path.GetFileNameWithoutExtension(args.NewProfile.Path);
            if (file == null) return;
            var quest = file.Substring(0, 5);
            if (!uint.TryParse(quest, out _currentQuestId)) return;
            _currentQuestName = GetQuestName(quest);
            _currentIsBlacklisted = QuestLists.BlackListed.Contains(_currentQuestId);
            _currentIsRaidQuest = QuestLists.RaidQuests.Contains(_currentQuestId);
            Log( $"Current Quest Set To {_currentQuestName} ({_currentQuestId}) Blacklisted {_currentIsBlacklisted} RaidQuest {_currentIsRaidQuest}");
        }

        private static void ResultsReceived(object sender, LuaEventArgs args)
        {
            if (!Setting) return;
            if(_currentQuestId == 0) return;
            if (_currentIsBlacklisted) return;
            
            Log("LFG_LIST_SEARCH_RESULTS_RECEIVED");

            var results = LfgList.GetSearchResults;
            Log($"Found {results.Count} Results");

            foreach (var r in results)
            {
                if (r.IsDelisted) continue;
                if (r.PlayerCount > 4 && !_currentIsRaidQuest) continue;
                Log($"Applying to {r}");
                if (r.Comment != null && r.Comment.Contains($"#WQ:{_currentQuestId}"))
                    r.ApplyToGroup(WqgfComment);
                else
                    r.ApplyToGroup();
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

            if (!StyxWoW.Me.GroupInfo.IsInRaid &&
                StyxWoW.Me.GroupInfo.IsInParty &&
                StyxWoW.Me.GroupInfo.NumPartyMembers == 1 &&
                !StyxWoW.Me.CurrentMap.IsScenario &&
                StyxWoW.Me.CurrentMap.IsContinent)
            {
                Log("Leaving party were the only one in it :(");
                LfgList.LeaveGroup();
                return;
            }

            if (StyxWoW.Me.GroupInfo.IsInRaid &&
                StyxWoW.Me.GroupInfo.NumRaidMembers == 1 &&
                !StyxWoW.Me.CurrentMap.IsScenario &&
                StyxWoW.Me.CurrentMap.IsContinent)
            {
                Log("Leaving raid were the only one in it :(");
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
            if (_currentIsBlacklisted) return;

            if (StyxWoW.Me.GroupInfo.IsInRaid && !_currentIsRaidQuest)
            {
                Log($"Leaving group cant complete {_currentQuestName} in a Raid");
                LfgList.LeaveGroup();
                return;
            }

            if (!StyxWoW.Me.QuestLog.ContainsQuest(_currentQuestId))
                return;

            if (StyxWoW.Me.GroupInfo.IsInParty || StyxWoW.Me.GroupInfo.IsInRaid) return;
            var diff = DateTime.Now.Subtract(_lastSearchTime).TotalSeconds;
            if (diff < 15) return;
            Log($"Starting a search for {_currentQuestName} {_currentQuestId}");
            LfgList.Search(LFGCategory.Questing, _currentQuestName, 0, 4);
            _lastSearchTime = DateTime.Now;
        }
    }
}