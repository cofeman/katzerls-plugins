using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Inventory;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;


namespace FCBot.Helpers
{
    class QuestingAndInteraction
    {
        // Returns a Dictionary of all equipped items
        private static Dictionary<InventorySlot, WoWItem> EquippedItems
        {
            get
            {
                Dictionary<InventorySlot, WoWItem> equipped = new Dictionary<InventorySlot, WoWItem>();
                WoWItem[] items = StyxWoW.Me.Inventory.Equipped.Items;

                equipped.Clear();
                for (int i = 0; i < 23; i++)
                {
                    equipped.Add((InventorySlot) (i + 1), items[i]);
                }

                return equipped;
            }
        }

        //public static bool Repaired = false;
        private static readonly WeightSetEx WeightSet = WeightSetEx.CurrentWeightSet;

        public static void AttachQuestingEvents()
        {
            Lua.Events.AttachEvent("QUEST_GREETING", OnQuestGreeting);
            Lua.Events.AttachEvent("QUEST_DETAIL", OnQuestDetail);
            Lua.Events.AttachEvent("GOSSIP_SHOW", OnGossipShow);
            Lua.Events.AttachEvent("QUEST_COMPLETE", OnQuestComplete);
            Lua.Events.AttachEvent("QUEST_PROGRESS", OnQuestProgress);

        }

        public static void DetatchQuestingEvents()
        {
            Lua.Events.DetachEvent("QUEST_GREETING", OnQuestGreeting);
            Lua.Events.DetachEvent("QUEST_DETAIL", OnQuestDetail);
            Lua.Events.DetachEvent("GOSSIP_SHOW", OnGossipShow);
            Lua.Events.DetachEvent("QUEST_COMPLETE", OnQuestComplete);
            Lua.Events.DetachEvent("QUEST_PROGRESS", OnQuestProgress);

        }


        public static void OnQuestProgress(object obj, LuaEventArgs args)
        {
            Log.Info("OnQuestProgress .... ");
            
            if (Setting.Instance.QuestTurnIn.OK())
            Lua.DoString("RunMacroText(\"/click QuestFrameCompleteButton\")");
        }

        public static void OnQuestDetail(object obj, LuaEventArgs args)
        {
            Log.Info("OnQuestDetail .... ");

            if (Setting.Instance.QuestPickUp.OK())
            Lua.DoString("AcceptQuest()");
        }

        public static void OnQuestGreeting(object obj, LuaEventArgs args)
        {
            Log.Info("OnQuestGreeting ....");
            int activeQuests = Lua.GetReturnVal<Int32>("return GetNumGossipActiveQuests()", 0);

            if (Setting.Instance.QuestPickUp.OK()) 
            {
                if (Lua.GetReturnVal<Int32>("return GetNumGossipAvailableQuests()", 0) > 0)
                {
                    Lua.DoString("SelectGossipAvailableQuest(GetNumGossipAvailableQuests())");
                    Lua.DoString("AcceptQuest()");
                }
            }
            
            
            if (activeQuests > 0 && Setting.Instance.QuestTurnIn.OK())
            {
                Log.Info("Active Quests: " + activeQuests.ToString(CultureInfo.InvariantCulture));
                for (int i = 1; i <= activeQuests; i++)
                {

                    bool isComplete = Lua.GetReturnVal<bool>("return ({GetGossipActiveQuests()})[" + (i * 4).ToString(CultureInfo.InvariantCulture) + "]", 0);

                    if (isComplete)
                    {
                        Lua.DoString("SelectGossipActiveQuest(" + i.ToString(CultureInfo.InvariantCulture) + ")");
                    }
                }
            }
        }

        public static void OnGossipShow(object obj, LuaEventArgs args)
        {
            Log.Info("OnGossipShow ....");

            //Force a sleep here to make sure the gossip window shows up (CanMerchantRepair was not working until I added this in)
            StyxWoW.SleepForLagDuration();
            Thread.Sleep(1000);
            
            // Repair if we need to
            if (Lua.GetReturnVal<Boolean>("return CanMerchantRepair()", 0))
            {
                //Repaired = true;

                Lua.DoString("RepairAllItems(1)");
                StyxWoW.SleepForLagDuration();
                
                Lua.DoString("RepairAllItems()");
                StyxWoW.SleepForLagDuration();
            }

            
            // Returns the number of active quests that you should eventually turn in to this NPC.
            int activeQuests = Lua.GetReturnVal<Int32>("return GetNumGossipActiveQuests()", 0);


            if (Setting.Instance.QuestPickUp.OK())
            {
                // Returns the number of quests available from the current Gossip NPC
                if (Lua.GetReturnVal<Int32>("return GetNumGossipAvailableQuests()", 0) > 0)
                {
                    Lua.DoString("SelectGossipAvailableQuest(GetNumGossipAvailableQuests())");

                    // Accepts the currently offered quest
                    Lua.DoString("AcceptQuest()");
                }
            }


            if (activeQuests > 0 && Setting.Instance.QuestTurnIn.OK())
            {
                Log.Info("Active Quests: " + activeQuests.ToString(CultureInfo.InvariantCulture));
                for (int i = 1; i <= activeQuests; i++)
                {

                    bool isComplete =
                        Lua.GetReturnVal<bool>(
                            "return ({GetGossipActiveQuests()})[" + (i*4).ToString(CultureInfo.InvariantCulture) + "]",
                            0);

                    if (isComplete)
                    {
                        // Selects an active quest from a gossip list. 
                        Lua.DoString("SelectGossipActiveQuest(" + i.ToString(CultureInfo.InvariantCulture) + ")");
                    }
                }
            }
        }



        public static void OnQuestComplete(object obj, LuaEventArgs args)
        {
            Log.Info("Completing Quest...");
            if (!Setting.Instance.QuestTurnIn.OK()) return;

            int numItems = Lua.GetReturnVal<Int32>("return GetNumQuestChoices()", 0);

            if (numItems > 1 && Setting.Instance.QuestReward.OK())
            {
                float bestOverallItemScore = float.MinValue;
                int bestOverallItemChoice = -1;

                for (int i = 1; i <= numItems; i++)
                {
                    Log.Info("Getting ItemInfo (" + i.ToString(CultureInfo.InvariantCulture) + ")");
                    string itemLink = Lua.GetReturnVal<string>("return GetQuestItemLink(\"choice\", " + i.ToString(CultureInfo.InvariantCulture) + ")", 0);
                    string[] splitted = itemLink.Split(':');

                    uint itemId;
                    if (string.IsNullOrEmpty(itemLink) || (splitted.Length == 0 || splitted.Length < 2) || (!uint.TryParse(splitted[1], out itemId) || itemId == 0))
                    {
                        Log.Info("Parsing ItemLink for quest item failed!");
                        Log.Info("ItemLink: {0}", itemLink);
                        continue;
                    }

                    ItemInfo choiceItemInfo = ItemInfo.FromId(itemId);
                    if (choiceItemInfo == null)
                    {
                        Log.Info("Retrieving item info for roll item failed");
                        Log.Info("Item Id:{0} ItemLink:{1}", itemId, itemLink);
                        continue;
                    }

                    // Score of the item being rolled for.
                    float choiceItemScore = WeightSet.EvaluateItem(choiceItemInfo, new ItemStats(itemLink));

                    // Score the equipped item if any. otherwise 0
                    float bestEquipItemScore = float.MinValue;

                    // The best slot
                    InventorySlot bestSlot = InventorySlot.None;

                    var inventorySlots = InventoryManager.GetInventorySlotsByEquipSlot(choiceItemInfo.EquipSlot);
                    foreach (InventorySlot slot in inventorySlots)
                    {
                        WoWItem equipped = EquippedItems[slot];

                        if (equipped != null)
                        {
                            bestEquipItemScore = WeightSet.EvaluateItem(equipped, true);
                            bestSlot = slot;
                        }
                    }

                    Log.Info("Item " + choiceItemInfo.Name + " scored " + choiceItemScore.ToString(CultureInfo.InvariantCulture));
                    if (bestEquipItemScore > float.MinValue)
                    {
                        Log.Info("Equipped Item {0} scored {1}", EquippedItems[bestSlot].Name, bestEquipItemScore.ToString(CultureInfo.InvariantCulture));
                    }
                    //if (bestEquipItemScore != float.MinValue)
                    //	slog("Equipped item in slot {0} scored {1} while quest-reward item scored: {2}", bestSlot, bestEquipItemScore, choiceItemScore);

                    bool goodArmor = choiceItemInfo.ItemClass == WoWItemClass.Armor &&
                                     (choiceItemInfo.ArmorClass == WeightSet.GetWantedArmorClass());// || miscArmorType.Contains(choiceItemInfo.InventoryType));

                    if (choiceItemScore > bestEquipItemScore && bestSlot != InventorySlot.None)
                    {
                        if (goodArmor && (choiceItemScore - bestEquipItemScore) > bestOverallItemScore)
                        {
                            bestOverallItemScore = (choiceItemScore - bestEquipItemScore);
                            bestOverallItemChoice = i;
                        }
                    }
                }

// ReSharper disable CompareOfFloatsByEqualityOperator
                if (bestOverallItemScore == float.MinValue)
// ReSharper restore CompareOfFloatsByEqualityOperator
                {
                    Log.Info("No best item found, going by item value");
                    ItemInfo highestItemInfo = null;

                    for (int i = 1; i <= numItems; i++)
                    {
                        Log.Info("Getting ItemInfo (" + i.ToString(CultureInfo.InvariantCulture) + ")");
                        string itemLink = Lua.GetReturnVal<string>("return GetQuestItemLink(\"choice\", " + i.ToString(CultureInfo.InvariantCulture) + ")", 0);
                        string[] splitted = itemLink.Split(':');

                        uint itemId;
                        if (string.IsNullOrEmpty(itemLink) || (splitted.Length == 0 || splitted.Length < 2) || (!uint.TryParse(splitted[1], out itemId) || itemId == 0))
                        {
                            Log.Info("Parsing ItemLink for quest item failed!");
                            Log.Info("ItemLink: {0}", itemLink);
                            continue;
                        }

                        ItemInfo choiceItemInfo = ItemInfo.FromId(itemId);
                        if (choiceItemInfo == null)
                        {
                            Log.Info("Retrieving item info for roll item failed");
                            Log.Info("Item Id:{0} ItemLink:{1}", itemId, itemLink);
                            continue;
                        }

                        if (highestItemInfo == null || highestItemInfo.SellPrice > choiceItemInfo.SellPrice)
                        {
                            highestItemInfo = choiceItemInfo;
                            bestOverallItemChoice = i;
                        }
                    }
                }

                Log.Info("Best Overall Item Choice: " + bestOverallItemChoice.ToString(CultureInfo.InvariantCulture));
                if (bestOverallItemChoice > 0)
                {
                    Lua.DoString("RunMacroText(\"/click QuestInfoItem" + bestOverallItemChoice.ToString(CultureInfo.InvariantCulture) + "\")");
                }
                
            }

            Lua.DoString("RunMacroText(\"/click QuestFrameCompleteQuestButton\")");
        }


        /// <summary>
        /// Returns a list of objects within 30 yards associated with quests. Eg need to collect X number of items
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static List<WoWGameObject> QuestItems(int distance)
        {
           return
                ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o =>((o.SubType == WoWGameObjectType.Chest || o.SubType == WoWGameObjectType.Goober) && !o.IsHerb && !o.IsMineral && o.CanLoot) || (o.SubType == WoWGameObjectType.QuestGiver && (o.QuestGiverStatus == QuestGiverStatus.Available || o.QuestGiverStatus == QuestGiverStatus.TurnIn)) && o.Location.Distance(FpswareCombatBot.Leader.Location) <= distance && !o.IsChest && !o.IsHerb && !o.IsMineral && !Blacklist.Contains(o.Guid, BlacklistFlags.Loot)).OrderBy(o => o.Distance).ToList();
        }

        public static List<WoWGameObject> HerbalismItems(int distance)
        {
            return ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.IsHerb && o.CanLoot && !Blacklist.Contains(o.Guid, BlacklistFlags.Node) && o.Location.Distance(FpswareCombatBot.Leader.Location) <= distance).OrderBy(o => o.Distance).ToList();
        }

        public static List<WoWGameObject> OreItems(int distance)
        {
            return ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.IsMineral && o.CanLoot && !Blacklist.Contains(o.Guid, BlacklistFlags.Node) && o.Location.Distance(FpswareCombatBot.Leader.Location) <= distance).OrderBy(o => o.Distance).ToList();
        }


        internal const int DistanceForQuestItem = 30;
        internal static WoWGameObject ObjectToGrab;

        


    }
}
