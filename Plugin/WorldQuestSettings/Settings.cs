using System.ComponentModel;
using System.IO;
using Styx.Helpers;

namespace WorldQuestSettings
{
    public class Settings : Styx.Helpers.Settings
    {
        public static readonly string _settingsPath = Path.Combine(CharacterSettingsDirectory, "Profiles",
            "NuokWorldQuestSettings.xml");

        private static Settings _instance;

        public Settings() : base(_settingsPath)
        {
        }

        public static Settings Instance => _instance ?? (_instance = new Settings());

        [Setting]
        [Category("WQGF")]
        [DisplayName("Auto Group")]
        [Description("Attempt to join groups created through the WQGF addon")]
        [Styx.Helpers.DefaultValue(false)]
        public bool WQGF { get; set; }

        [Setting]
        [Category("WQGF")]
        [DisplayName("Min Leave Time")]
        [Description("The minium time in seconds to leave the group after completing a quest")]
        [Styx.Helpers.DefaultValue(3)]
        [Limit(1, 100)]
        public int WQGFMin { get; set; }

        [Setting]
        [Category("WQGF")]
        [DisplayName("Max Leave Time")]
        [Description("The maximum time in seconds to leave the group after completing a quest")]
        [Styx.Helpers.DefaultValue(10)]
        [Limit(1, 100)]
        public int WQGFMax { get; set; }

        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do Wanted Quests")]
        [Styx.Helpers.DefaultValue(true)]
        public bool DoWanted { get; set; }

        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do DANGER Quests")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoDangerQuests { get; set; }

        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do Elite Quests")]
        [Description("Quests the require a group")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoElite { get; set; }

        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do Rare Quests")]
        [Description("Quests that are hard but can be done solo")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoRare { get; set; }

        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do PvP Quests")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoPvP { get; set; }

        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do Pet Battle Quests")]
        [Description("Requires a pet battle plugin like Pairidaeza")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoPetBattles { get; set; }

        [Setting]
        [Category("Hard Quests")]
        [DisplayName("Do World Bosses")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoWorldBosses { get; set; }

        [Setting]
        [Category("Professions")]
        [DisplayName("Do Professions Quests")]
        [Styx.Helpers.DefaultValue(true)]
        public bool DoProfessions { get; set; }

        [Setting]
        [Category("Professions")]
        [DisplayName("Do Work Order Quests")]
        [Styx.Helpers.DefaultValue(false)]
        public bool DoWorkOrders { get; set; }

        [Setting]
        [Category("Utility")]
        [DisplayName("Skip HB Relog Task")]
        [Styx.Helpers.DefaultValue(true)]
        public bool SkipHBRelog { get; set; }

        [Setting]
        [Category("Utility")]
        [DisplayName("SVN Update")]
        [Description("Runs the updater.bat file to try and update profiles.")]
        [Styx.Helpers.DefaultValue(false)]
        public bool SVNUpdate { get; set; }

        [Setting]
        [Category("Utility")]
        [DisplayName("Loop Profile")]
        [Description("Runs the profile in a loop to try and prevent it from missing quests")]
        [Styx.Helpers.DefaultValue(false)]
        public bool LoopProfile { get; set; }

        #region zones

        [Setting]
        [Category("Zone")]
        [DisplayName("Azsuna")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Azsuna { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Val'sharah")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Valsharah { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Stormheim")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Stormheim { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Suramar")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Suramar { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Highmountain")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Highmountain { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Eye of Azshara")]
        [Styx.Helpers.DefaultValue(true)]
        public bool EyeofAzshara { get; set; }

        [Setting]
        [Category("Zone")]
        [DisplayName("Dalaran")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Dalaran { get; set; }

        #endregion

        #region factions

        [Setting]
        [Category("Faction")]
        [DisplayName("The Wardens")]
        [Styx.Helpers.DefaultValue(true)]
        public bool TheWardens { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Court of Farondis")]
        [Styx.Helpers.DefaultValue(true)]
        public bool CourtOfFarondis { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Kirin Tor")]
        [Styx.Helpers.DefaultValue(true)]
        public bool KirinTor { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Highmountain Tribe")]
        [Styx.Helpers.DefaultValue(true)]
        public bool HighmountainTribe { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Dreamweavers")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Dreamweavers { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("The Nightfallen")]
        [Styx.Helpers.DefaultValue(true)]
        public bool TheNightfallen { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Valarjar")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Valarjar { get; set; }

        [Setting]
        [Category("Faction")]
        [DisplayName("Do Emissary First")]
        [Description("Complete the Emissary Quest First")]
        [Styx.Helpers.DefaultValue(true)]
        public bool EmissaryFirst { get; set; }

        #endregion

        #region rewards

        [Setting]
        [Category("Rewards")]
        [DisplayName("Artifact Power")]
        [Description("Rewards that increase Artifact Power")]
        [Styx.Helpers.DefaultValue(true)]
        public bool ArtifactPower { get; set; }

        [Setting]
        [Category("Rewards")]
        [DisplayName("Equipment")]
        [Description("Gear for your character")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Equipment { get; set; }

        [Setting]
        [Category("Rewards")]
        [DisplayName("Item's")]
        [Description("Includes Trade Goods & Profession Items")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Items { get; set; }

        [Setting]
        [Category("Rewards")]
        [DisplayName("Blood of Sargeras")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Blood { get; set; }

        [Setting]
        [Category("Rewards")]
        [DisplayName("Order Hall Resource")]
        [Styx.Helpers.DefaultValue(true)]
        public bool OrderHall { get; set; }

        [Setting]
        [Category("Rewards")]
        [DisplayName("Gold")]
        [Styx.Helpers.DefaultValue(true)]
        public bool Gold { get; set; }

        #endregion
    }
}