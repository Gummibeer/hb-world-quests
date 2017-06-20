using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media;
using CommonBehaviors.Actions;
using Styx;
using Styx.Common;
using Styx.CommonBot.Coroutines;
using Styx.WoWInternals;

namespace WorldQuestSettings.GroupFinder
{
    public class LfgList
    {
        private static readonly Dictionary<int, ActivityInfo> CachedActivityInfos = new Dictionary<int, ActivityInfo>();

        private static IEnumerable<int> GetAvailableCategoryIds
        {
            get
            {
                using (StyxWoW.Memory.AcquireFrame())
                {
                    const string getIdsLua = @"local categories = C_LFGList.GetAvailableCategories();
local results = {}
for i = 1, #categories do        
local id = categories[i]
if (id ~= nil) then 
table.insert(results, id) 
end 
end 
return unpack(results)";
                    var l = new List<int>();
                    var getResultIds = Lua.GetReturnValues(getIdsLua);
                    l.AddRange(getResultIds.Select(Lua.ParseLuaValue<int>));
                    return l;
                }
            }
        }

        public static List<CategoryInfo> GetAvailableCategories
        {
            get
            {
                using (StyxWoW.Memory.AcquireFrame())
                {
                    return GetAvailableCategoryIds.Select(GetCategoryInfo).ToList();
                }
            }
        }

        public static List<ApplicantInfo> GetApplicants
        {
            get
            {
                var l = new List<ApplicantInfo>();
                const string getAppIds = @"local applicants = C_LFGList.GetApplicants();
local results = {}
if (applicants) then
for i=1, #applicants do   
local id = C_LFGList.GetApplicantInfo(applicants[i]);
if (id ~= nil) then 
table.insert(results, id) 
end
end
end
return unpack(results)";
                using (StyxWoW.Memory.AcquireFrame())
                {
                    var getResults = Lua.GetReturnValues(getAppIds);
                    l.AddRange(getResults.Select(r => ApplicantInfo.GetApplicantInfo(Lua.ParseLuaValue<uint>(r))));
                }
                return l;
            }
        }

        /// <summary>
        ///     Retrieve the search information for the last search only
        /// </summary>
        public static List<SearchResultInfo> GetSearchResults
        {
            get
            {
                const string getIdsLua = @"local numResults, resultIDTable = C_LFGList.GetSearchResults();
local results = {}
for i = 1, numResults do        
local id = resultIDTable[i]
if (id ~= nil) then 
table.insert(results, id) 
end
end
return unpack(results)";

                var l = new List<int>();
                using (StyxWoW.Memory.AcquireFrame())
                {
                    var getResultIds = Lua.GetReturnValues(getIdsLua);
                    l.AddRange(getResultIds.Select(Lua.ParseLuaValue<int>));
                }
                var x = (from i in l where i != 0 select GetSearchResultInfo(i)).ToList();
                return x;
            }
        }

        public static int NumActiveApplications => Lua.GetReturnVal<int>("return C_LFGList.GetNumApplications()", 1);

        public static int GetNumInvitedApplicantMembers
            => Lua.GetReturnVal<int>("return C_LFGList.GetNumInvitedApplicantMembers()", 0);

        public static bool IsGroupLeader =>
            Lua.GetReturnVal<bool>("return UnitIsGroupLeader(\"player\")", 0);

        /// <summary>
        ///     Gets all the currently applied for groups
        /// </summary>
        public static List<ApplicationInfo> GetApplicationInfos
        {
            get
            {
                var l = new List<ApplicationInfo>();

                using (StyxWoW.Memory.AcquireFrame())
                {
                    var apps = GetApplicationIds;
                    l.AddRange(apps.Select(GetApplicationInfo));
                }
                return l;
            }
        }

        private static IEnumerable<int> GetApplicationIds
        {
            get
            {
                var l = new List<int>();
                const string lua = @"local apps = C_LFGList.GetApplications(); 
local results = {}
for i = 1, #apps do        
local id = apps[i]
if (id ~= nil) then 
table.insert(results, id) 
end
end
return unpack(results)";
                var getResultIds = Lua.GetReturnValues(lua);
                l.AddRange(getResultIds.Select(Lua.ParseLuaValue<int>));
                return l;
            }
        }

        /// <summary>
        ///     Get all available activity ids
        /// </summary>
        private static List<int> GetAvailableActivitiesIds
        {
            get
            {
                // /dump C_LFGList.GetAvailableActivities()
                const string lua = "return unpack(C_LFGList.GetAvailableActivities());";
                var r = Lua.GetReturnValues(lua);
                return r.Select(Lua.ParseLuaValue<int>).ToList();
            }
        }

        /// <summary>
        ///     Get all available activities
        /// </summary>
        public static List<ActivityInfo> GetAvailableActivities
        {
            get
            {
                var r = new List<ActivityInfo>();
                using (StyxWoW.Memory.AcquireFrame())
                {
                    var ids = GetAvailableActivitiesIds;
                    r.AddRange(ids.Select(GetActivityInfo));
                }
                return r;
            }
        }

        public static void InviteApplicant(uint id) => Lua.DoString($"C_LFGList.InviteApplicant({id})");

        /// <summary>
        ///     Opens the LFG Frame in game
        /// </summary>
        public static void LFGFrame_Show()
        {
            Lua.DoString("PVEFrame_ShowFrame('GroupFinderFrame', LFGListPVEStub)");
        }

        private static void Log(string format, params object[] args)
        {
            Logging.WriteDiagnostic(Colors.Goldenrod, "LFGList: " + format, args);
        }

        /// <summary>
        ///     Applies to join the group
        /// </summary>
        /// <param name="id">Group Id</param>
        public static void ApplyToGroup(int id)
        {
            Lua.DoString(
                $"local tank, heal, dd = C_LFGList.GetAvailableRoles(); C_LFGList.ApplyToGroup({id}, '', tank, heal, dd);");
        }

        /// <summary>
        ///     Applies to join the group
        /// </summary>
        /// <param name="id">Group Id</param>
        /// <param name="comment"></param>
        public static void ApplyToGroup(int id, string comment)
        {
            Lua.DoString(
                $"local tank, heal, dd = C_LFGList.GetAvailableRoles(); C_LFGList.ApplyToGroup({id}, '{comment}', tank, heal, dd);");
        }

        /// <summary>
        ///     Applies to join the group
        /// </summary>
        /// <param name="application"></param>
        public static void ApplyToGroup(SearchResultInfo application)
        {
            Log($"Applying to Group {application.Name} on {application.Realm} (AutoInvite: {application.AutoInvite})");
            ApplyToGroup(application.Id);
        }

        /// <summary>
        ///     Applies to join the group
        /// </summary>
        /// <param name="application"></param>
        public static void ApplyToGroup(SearchResultInfo application, string comment)
        {
            Log(
                $"Applying to Group {application.Name} on {application.Realm} (AutoInvite: {application.AutoInvite}), (Comment {comment})");
            ApplyToGroup(application.Id, comment);
        }

        /// <summary>
        ///     Accepts group invite pop ups
        /// </summary>
        public static void AcceptPopUps()
        {
            Log("Accepting PopUps");
            Lua.DoString("LFGListInviteDialog.AcceptButton:Click(); LFGListInviteDialog.AcknowledgeButton:Click();");
        }

        private static CategoryInfo GetCategoryInfo(int categoryId)
        {
            var a = Lua.GetReturnValues($"return C_LFGList.GetCategoryInfo({categoryId});");

            var name = Lua.ParseLuaValue<string>(a[0]);
            var separate = Lua.ParseLuaValue<bool>(a[1]);
            var auto = Lua.ParseLuaValue<bool>(a[2]);
            var prefer = Lua.ParseLuaValue<bool>(a[3]);

            return new CategoryInfo
            {
                Category = (LFGCategory) categoryId,
                Name = name,
                SeparateRecommended = separate,
                AutoChoose = auto,
                PreferCurrentArea = prefer
            };
        }

        public static void Search(int categoryId, string param = "", int filters = 0, int preferredFilters = 0)
        {
            Lua.DoString("C_LFGList.ClearSearchResults(); " +
                "local languages = C_LFGList.GetLanguageSearchFilter(); " +
                $"C_LFGList.Search({categoryId}, LFGListSearchPanel_ParseSearchTerms(\"{param}\"), {filters}, {preferredFilters}, languages);");
        }

        /// <summary>
        ///     Initializes a search for groups within the category
        /// </summary>
        /// <param name="category">CategoryInfo</param>
        public static void Search(CategoryInfo category)
        {
            Search(category.Category);
        }

        public static void Search(CategoryInfo category, string param)
        {
            Search(category.Category, param);
        }

        /// <summary>
        ///     Starts the search for groups
        /// </summary>
        /// <param name="category">The category you wish to search</param>
        public static void Search(LFGCategory category)
        {
            Log($"Starting a search of {category}");
            Search((int) category);
        }

        public static void Search(LFGCategory category, string param)
        {
            Log($"Starting a search of {category}, {param}");
            Search((int) category, param);
        }

        public static void Search(LFGCategory category, string param, int filters, int preferredFilters)
        {
            Log($"Starting a search of {category}, {param}");
            Search((int)category, param, filters, preferredFilters);
        }

        private static ApplicationInfo GetApplicationInfo(int applicationId)
        {
            var info = Lua.GetReturnValues($"return C_LFGList.GetApplicationInfo({applicationId});");

            return new ApplicationInfo
            {
                Id = Lua.ParseLuaValue<int>(info[0]),
                Status = GetApplicationState(Lua.ParseLuaValue<string>(info[1])),
                PendingStatus = Lua.ParseLuaValue<bool>(info[2]),
                ApplicationDuration = TimeSpan.FromMinutes(Lua.ParseLuaValue<double>(info[3])),
                AcceptedRole = Lua.ParseLuaValue<string>(info[4])
            };
        }

        private static SearchResultInfo GetSearchResultInfo(int resultId)
        {
            var r = Lua.GetReturnValues($"return C_LFGList.GetSearchResultInfo({resultId});");
            //retrieve the realm & player name to see if they check out
            var s = Lua.ParseLuaValue<string>(r[12]);

            if (s == null)
                return new SearchResultInfo
                {
                    Id = resultId,
                    ActivityId = Lua.ParseLuaValue<int>(r[1]),
                    Name = Lua.ParseLuaValue<string>(r[2]),
                    Comment = Lua.ParseLuaValue<string>(r[3]),
                    VoiceChatDesc = r[4],
                    VoiceChat = Lua.ParseLuaValue<bool>(r[5]),
                    iLevel = Lua.ParseLuaValue<int>(r[6]),
                    Age = TimeSpan.FromSeconds(Lua.ParseLuaValue<int>(r[7])),
                    NumBNetFriends = Lua.ParseLuaValue<int>(r[8]),
                    NumCharFriends = Lua.ParseLuaValue<int>(r[9]),
                    NumGuildMates = Lua.ParseLuaValue<int>(r[10]),
                    IsDelisted = Lua.ParseLuaValue<bool>(r[11]),
                    PlayerCount = Lua.ParseLuaValue<int>(r[13]),
                    AutoInvite = Lua.ParseLuaValue<bool>(r[14]),
                    Realm = "null",
                    LeaderName = "null"
                };

            char[] sep = {'-'};
            var wowSplit = s.Split(sep, 2);

            var l = wowSplit.Length < 1 ? "null" : wowSplit[0];
            var ra = wowSplit.Length < 2 ? "null" : wowSplit[1];


            return new SearchResultInfo
            {
                Id = resultId,
                ActivityId = Lua.ParseLuaValue<int>(r[1]),
                Name = Lua.ParseLuaValue<string>(r[2]),
                Comment = Lua.ParseLuaValue<string>(r[3]),
                VoiceChatDesc = r[4],
                VoiceChat = Lua.ParseLuaValue<bool>(r[5]),
                iLevel = Lua.ParseLuaValue<int>(r[6]),
                Age = TimeSpan.FromSeconds(Lua.ParseLuaValue<int>(r[7])),
                NumBNetFriends = Lua.ParseLuaValue<int>(r[8]),
                NumCharFriends = Lua.ParseLuaValue<int>(r[9]),
                NumGuildMates = Lua.ParseLuaValue<int>(r[10]),
                IsDelisted = Lua.ParseLuaValue<bool>(r[11]),
                PlayerCount = Lua.ParseLuaValue<int>(r[13]),
                AutoInvite = Lua.ParseLuaValue<bool>(r[14]),
                LeaderName = l,
                Realm = ra
            };
        }

        public static bool LeaveGroup()
        {
            Log("Leaving Current Group");
            Lua.DoString("LeaveParty();");
            var s = new SleepForLagDuration();
            s.ExecuteCoroutine();
            return StyxWoW.Me.GroupInfo.IsInParty || StyxWoW.Me.GroupInfo.IsInRaid;
        }

        /// <summary>
        ///     Casts application state to enum ApplicationState
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public static ApplicationState GetApplicationState(string application)
        {
            switch (application)
            {
                case "invited":
                    return ApplicationState.Invited;
                case "failed":
                    return ApplicationState.Failed;
                case "cancelled":
                    return ApplicationState.Cancelled;
                case "applied":
                    return ApplicationState.Applied;
                case "inviteaccepted":
                    return ApplicationState.InviteAccepted;
                case "none":
                    return ApplicationState.None;
                case "declined":
                    return ApplicationState.Declined;
                case "invitedeclined":
                    return ApplicationState.InviteDeclined;
                case "declined_delisted":
                    return ApplicationState.DeclinedDelisted;
                default:
                {
                    Log($"{application} not listed as a state");
                    return ApplicationState.Failed;
                }
            }
        }

        /// <summary>
        ///     Get the activity info by id
        /// </summary>
        /// <param name="activityId">Activity Id</param>
        /// <returns>Active info for the activity id</returns>
        public static ActivityInfo GetActivityInfo(int activityId)
        {
            if (CachedActivityInfos.ContainsKey(activityId)) return CachedActivityInfos[activityId];

            // //dump C_LFGList.GetActivityInfo(3)
            var info = Lua.GetReturnValues($"return C_LFGList.GetActivityInfo({activityId});");

            var r = new ActivityInfo
            {
                //fullName, shortName, categoryID, groupID, itemLevel, filters, minLevel, maxPlayers, displayType, activityOrder
                FullName = info[0],
                ShortName = info[1],
                Category = (LFGCategory) Lua.ParseLuaValue<int>(info[2]),
                GroupId = Lua.ParseLuaValue<int>(info[3]),
                ItemLevel = Lua.ParseLuaValue<int>(info[4]),
                MinLevel = Lua.ParseLuaValue<int>(info[6]),
                MaxPlayers = Lua.ParseLuaValue<int>(info[7]),
                ActivityId = activityId
            };

            CachedActivityInfos.Add(activityId, r);
            return r;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum LFGCategory
    {
        None = 0,
        Questing = 1,
        Dungeons = 2,
        Raids = 3,
        Arenas = 4,
        Scenarios = 5,
        Custom = 6,
        Skirmishes = 7,
        Battlegrounds = 8,
        RatedBGs = 9,
        Ashran = 10
    }

    public class ActivityInfo
    {
        public LFGCategory Category { get; set; }
        public int ActivityId { get; set; }
        public string FullName { get; set; }
        public int GroupId { get; set; }
        public int ItemLevel { get; set; }
        public int MaxPlayers { get; set; }
        public int MinLevel { get; set; }
        public string ShortName { get; set; }

        public override string ToString()
        {
            return $"ActivityInfo {FullName} (Id {ActivityId})";
        }

        protected bool Equals(ActivityInfo other)
        {
            return ActivityId == other.ActivityId;
        }
    }

    [Flags]
    public enum ApplicationState
    {
        Invited,
        Failed,
        Cancelled,
        Applied,
        InviteAccepted,
        Declined,
        InviteDeclined,
        None,
        DeclinedDelisted
    }

    public class CategoryInfo
    {
        public CategoryInfo()
        {
            LastSearchTime = DateTime.MinValue;
        }

        public bool AutoChoose { get; set; }
        public string Name { get; set; }
        public bool PreferCurrentArea { get; set; }
        public bool SeparateRecommended { get; set; }

        public LFGCategory Category { get; set; }

        public DateTime LastSearchTime { get; set; }

        /// <summary>
        ///     Default entry for category info
        /// </summary>
        public static CategoryInfo None => new CategoryInfo
        {
            AutoChoose = false,
            Category = LFGCategory.None,
            LastSearchTime = DateTime.MinValue,
            Name = "None",
            PreferCurrentArea = false,
            SeparateRecommended = false
        };

        public void SearchCatergory()
        {
            LfgList.Search(this);
            LastSearchTime = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        protected bool Equals(CategoryInfo other)
        {
            return Category == other.Category;
        }
    }

    public class ApplicationInfo
    {
        public string AcceptedRole { get; set; }
        public TimeSpan ApplicationDuration { get; set; }
        public int Id { get; set; }
        public bool PendingStatus { get; set; }
        public ApplicationState Status { get; set; }

        public override string ToString()
        {
            return $"ApplicationId {Id} {Status}";
        }

        protected bool Equals(ApplicationInfo other)
        {
            return Id == other.Id;
        }

        protected bool Equals(SearchResultInfo other)
        {
            return Id == other.Id;
        }
    }

    public class ApplicantInfo
    {
        public uint Id { get; set; }
        public uint ApplicantId { get; set; }
        public ApplicationState Status { get; set; }
        public bool PendingStatus { get; set; }
        public uint NumMembers { get; set; }
        public bool IsNew { get; set; }
        public string Comment { get; set; }

        public static ApplicantInfo GetApplicantInfo(uint applicantId)
        {
            var info = Lua.GetReturnValues($"return C_LFGList.GetApplicantInfo({applicantId})");
            //local id, status, pendingStatus, numMembers, isNew, comment = C_LFGList.GetApplicantInfo(applicantID)
            return new ApplicantInfo
            {
                ApplicantId = applicantId,
                Id = Lua.ParseLuaValue<uint>(info[0]),
                Status = LfgList.GetApplicationState(info[1]),
                PendingStatus = Lua.ParseLuaValue<bool>(info[2]),
                NumMembers = Lua.ParseLuaValue<uint>(info[3]),
                IsNew = Lua.ParseLuaValue<bool>(info[4]),
                Comment = info[5]
            };
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class SearchResultInfo
    {
        private ActivityInfo _cActivityInfo;
        public int ActivityId { get; set; }
        public ActivityInfo ActivityInfo => _cActivityInfo ?? (_cActivityInfo = LfgList.GetActivityInfo(ActivityId));

        public TimeSpan Age { get; set; }
        public bool AutoInvite { get; set; }
        public string Comment { get; set; }
        public int Id { get; set; }
        public int iLevel { get; set; }
        public bool IsDelisted { get; set; }
        public string LeaderName { get; set; }
        public string Name { get; set; }
        public int NumBNetFriends { get; set; }
        public int NumCharFriends { get; set; }
        public int NumGuildMates { get; set; }
        public int PlayerCount { get; set; }
        public string Realm { get; set; }
        public bool VoiceChat { get; set; }
        public string VoiceChatDesc { get; set; }
        private bool HasApplied { get; set; }

        public void ApplyToGroup()
        {
            if (HasApplied) return;
            HasApplied = true;
            LfgList.ApplyToGroup(this);
        }

        public void ApplyToGroup(string comment)
        {
            if (HasApplied) return;
            HasApplied = true;
            LfgList.ApplyToGroup(this, comment);
        }

        public override string ToString()
        {
            return $"{Name} on {Realm} ({Id}, {ActivityInfo.FullName}, AutoInvite: {AutoInvite})";
        }


        protected bool Equals(SearchResultInfo other)
        {
            return Id == other.Id;
        }

        protected bool Equals(ApplicationInfo other)
        {
            return Id == other.Id;
        }
    }
}