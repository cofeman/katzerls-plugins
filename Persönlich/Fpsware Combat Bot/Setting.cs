using Styx;
using Styx.Common;
using Styx.Helpers;

namespace FCBot
{
    class Setting : Settings
    {
        // Required declarations and such
        private static Setting _instance;
        public Setting() : base(SettingsPath + ".xml") { }
        public Setting(string settingsPath) : base(settingsPath) { }
        private static string SettingsPath { get { return string.Format("{0}\\Settings\\Fpsware Combat Bot\\{1}", Utilities.AssemblyDirectory, StyxWoW.Me.Name); } }
        public static Setting Instance { get { return _instance ?? (_instance = new Setting()); } }

        
        // Settings here.
        [Setting, DefaultValue(5)]
        public int CombatFollowDistance { get; set; }

        [Setting, DefaultValue("Never")]
        public string CombatFollow { get; set; }

        [Setting, DefaultValue(15)]
        public int FollowDistance { get; set; }

        [Setting, DefaultValue("[None]")]
        public string FollowLeader { get; set; }

        [Setting, DefaultValue("Manual Targeting")]
        public string Targeting { get; set; }

        [Setting, DefaultValue("Never")]
        public string FaceTarget { get; set; }

        [Setting, DefaultValue("Never")]
        public string AllowCommands { get; set; }

        [Setting, DefaultValue("Never")]
        public string Loot { get; set; }

        [Setting, DefaultValue("Never")]
        public string MoveToTarget { get; set; }

        [Setting, DefaultValue("Enabled")]
        public string Plugins { get; set; }

        [Setting, DefaultValue("Never")]
        public string AttackFromBehind { get; set; }

        [Setting, DefaultValue(30)]
        public int TicksPerSecond { get; set; }

        // keys

        [Setting, DefaultValue("[None]")]
        public string KeyMoveToTarget { get; set; }

        [Setting, DefaultValue("[None]")]
        public string KeyMoveToTargetMod { get; set; }

        [Setting, DefaultValue("[None]")]
        public string KeyPauseBotbase { get; set; }

        [Setting, DefaultValue("[None]")]
        public string KeyPauseBotbaseMod { get; set; }

        [Setting, DefaultValue("[None]")]
        public string KeyLooting { get; set; }

        [Setting, DefaultValue("[None]")]
        public string KeyLootingMod { get; set; }

        [Setting, DefaultValue("[None]")]
        public string KeyAssist { get; set; }

        [Setting, DefaultValue("[None]")]
        public string KeyAssistMod { get; set; }

        [Setting, DefaultValue("[None]")]
        public string KeyCombatFollow { get; set; }

        [Setting, DefaultValue("[None]")]
        public string KeyCombatFollowMod { get; set; }

        [Setting, DefaultValue("Never")]
        public string ForceAutoAttack { get; set; }

        // chat commands
        [Setting, DefaultValue("stand here | move here | come here")]
        public string ChatMoveHereCommand { get; set; }

        [Setting, DefaultValue("follow me | get over here")]
        public string ChatFollowCommand { get; set; }

        [Setting, DefaultValue("wait here | don't follow")]
        public string ChatNoFollowCommand { get; set; }

        [Setting, DefaultValue("loot now | please loot | enable looting")]
        public string ChatLootCommand { get; set; }

        [Setting, DefaultValue("don't loot | disable looting | no looting")]
        public string ChatNoLootCommand { get; set; }

        [Setting, DefaultValue("assist me | target my target")]
        public string ChatAssistCommand { get; set; }

        [Setting, DefaultValue("click the | interact with")]
        public string ChatClickThisCommand { get; set; }

        [Setting, DefaultValue("use item | use inventory item")]
        public string ChatInventoryItemCommand { get; set; }

        [Setting, DefaultValue("follow distance | set follow distance")]
        public string ChatFollowDistanceCommand { get; set; }

        [Setting, DefaultValue("combat follow | glue | follow me now")]
        public string ChatCombatFollowCommand { get; set; }

        [Setting, DefaultValue("stop combat follow | get off me")]
        public string ChatNoCombatFollowCommand { get; set; }

        [Setting, DefaultValue("just follow me | slash follow")]
        public string ChatSlashFollowCommand { get; set; }




        [Setting, DefaultValue("Automatically Accept")]
        public string AutomationRecurrection { get; set; }

        [Setting, DefaultValue("Automatically Accept")]
        public string AutomationReadyCheck { get; set; }

        [Setting, DefaultValue("Automatically Accept")]
        public string AutomationLFGReady { get; set; }



        // Questing

        [Setting, DefaultValue("Ignore")]
        public string QuestPickUp { get; set; }

        [Setting, DefaultValue("Ignore")]
        public string QuestTurnIn { get; set; }

        [Setting, DefaultValue("Ignore")]
        public string QuestReward { get; set; }

        [Setting, DefaultValue("Ignore")]
        public string QuestObjectInteract { get; set; }

        // Interact
        [Setting, DefaultValue("Ignore")]
        public string InteractSkinning { get; set; }

        [Setting, DefaultValue("Ignore")]
        public string InteractHerbalism { get; set; }

        [Setting, DefaultValue("Ignore")]
        public string InteractMining { get; set; }

        [Setting, DefaultValue(20)]
        public int InteractMaxRange { get; set; }



        [Setting, DefaultValue("Ignore")]
        public string LootRolling { get; set; }
        

    }
}