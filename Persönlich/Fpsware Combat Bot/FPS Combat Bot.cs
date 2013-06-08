//
// Credit where its due. Some parts of this botbase are taken from LazyRaider, RaidBot and CombatBot
// Some of the questing and events are taken from Multibox Suit released by tjhasty
// Credit to the original authors where due. 
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CommonBehaviors.Actions;
using FCBot.UI;
using JetBrains.Annotations;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.Helpers;
using Styx.Pathing;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.CommonBot.Profiles;
using Action = Styx.TreeSharp.Action;
using FCBot.Helpers;

namespace FCBot
{
    class FpswareCombatBot : BotBase
    {
        #region Local declarations
        public static LocalPlayer Me { get { return StyxWoW.Me; } }
        public static WoWUnit CT { get { return Me.CurrentTarget; } }
        public static bool IsInGroup { get { return Me.GroupInfo.IsInRaid || Me.GroupInfo.IsInParty; } }
        public static IEnumerable<WoWPlayer> GroupMembers { get { return !Me.GroupInfo.IsInRaid ? Me.PartyMembers : Me.RaidMembers; } }
        public static IEnumerable<WoWPartyMember> GroupMemberInfos { get { return !Me.GroupInfo.IsInRaid ? Me.GroupInfo.PartyMembers : Me.GroupInfo.RaidMembers; } }

        public static bool PauseBotbase = false;
        private static bool _forceAttacking;
        private byte _oldTps;
        private Composite _root;
        private static WoWPoint _lastDest;
        private static bool _botMovement;
        private static PulseFlags _pulseFlags;
        private readonly Version _version = new Version(0, 0, 1);

        #endregion

        #region Overrides

        public override PulseFlags PulseFlags
        {
            get
            {
                return _pulseFlags;
            }
        }

        public override string Name {get { return string.Format("FPS Combat Bot"); }}

        private UIForm _frm;
        public override Form ConfigurationForm
        {
            get
            {
                if (_frm == null || !_frm.Visible) _frm = new UIForm();

                return _frm;
            }
        }

        public override Composite Root { get { return _root ?? (_root = new PrioritySelector(CreateRootBehavior())); } }

        //private bool _looting;
        private bool _vendoring;

        #endregion

        #region Composites
        private Composite CreateRootBehavior()
        {
            return
                new PrioritySelector(

                    // Dead, ghost or botbase paused then do nothing
                    new Decorator(ret => !Me.IsAlive || Me.IsGhost, new Action(ret => RunStatus.Success)),
                    new Decorator(ret => PauseBotbase, new Action(ret => RunStatus.Success)),
                    
                    // Not in combat behavior, Pre combat buffs and Resting etc
                    new Decorator(ret => !Me.Combat, CreateNotInCombatBehavior()),
                    
                    // In combat behavior, Combat, Healing, combat buffs
                    new Decorator(ret => Me.Combat, CreateCombatBehavior()),
                    
                    // Follow the leader
                    CreateFollowTheLeaderBehavior()


            );
        }

        #region In combat behavior
        private Composite CreateCombatBehavior()
        {
            return

                new LockSelector(

                    // Find a target if we need one. Applies to DPS and Tanks
                    CreateFindATargetBehavior(),

                    // Heal
                    RoutineManager.Current.HealBehavior,

                    // Combat Buff and Combat Behavior
                    new Decorator(ret => Me.GotTarget && CT.Attackable && !CT.IsDead,
                                  new PrioritySelector(

                                      RoutineManager.Current.CombatBuffBehavior,

                                      // Move to target and face 
                                      CreateCombatMovementAndFaceTarget(),

                                      // Auto attack for melee classes
                                      new Decorator(ret =>!StyxWoW.Me.IsAutoAttacking && StyxWoW.Me.AutoRepeatingSpellId != 75 && CT.IsWithinMeleeRange && Helpers.Extensions.IsMeMeleeClass,
                                          new Action(delegate{Lua.DoString("StartAttack()");return RunStatus.Failure;})),
                                    
                                       // Update HB status message with combat
                                       new Action(context => { TreeRoot.StatusText = "Combat with " + CT.Name; return RunStatus.Failure; }),
                                         
                                      // Perform the CC combat behavior
                                      RoutineManager.Current.CombatBehavior

                                      )));
        }
        private Composite CreateFindATargetBehavior()
        {
            return

                new PrioritySelector(
                    
                // Find a target if we need one
                            new Decorator(ret => Setting.Instance.Targeting.Contains("Assist") && Leader != null && Leader.GotTarget && Leader.Combat && (!Me.GotTarget || Me.GotTarget && CT.IsDead || Me.GotTarget && !CT.IsValidTarget() || Me.GotTarget && CT.Guid != Leader.CurrentTarget.Guid),
                                          new Sequence(
                                              new Action(context => Leader.CurrentTarget.Target()),
                                              //new LogMessage(string.Format("Assisting {0} with {1}", Leader.Name, Leader.CurrentTarget.Name))
                                              new Action(context => Log.Info(string.Format("Assisting {0} with {1}", Leader.Name, Leader.CurrentTarget.Name), true))
                                              )
                                ),

                            // Find a target if we need one
                            new ThrottlePasses(TimeSpan.FromMilliseconds(400),
                                new Decorator(ret => Setting.Instance.Targeting.Contains("Closest") && (!Me.GotTarget || Me.GotTarget && CT.IsDead || !CT.IsValidTarget()) && Units.AttackableUnits.FirstOrDefault() != null,
                                              new Sequence(
                                                  new Action(context => Units.AttackableUnits.OrderBy(u=>u.Distance).FirstOrDefault().Target()),
                                                  new Action(context => Log.Info(string.Format("Finding closest mob")))
                                                  )
                                )),

                                // Find a target if we need one
                            new ThrottlePasses(TimeSpan.FromMilliseconds(400),
                                new Decorator(ret => Setting.Instance.Targeting.Contains("Lowest Threat") && (!Me.GotTarget || Me.GotTarget && CT.IsDead || !CT.IsValidTarget()) && Units.AttackableUnits.FirstOrDefault() != null,
                                              new Sequence(
                                                  new Action(context =>
                                                      {
                                                          WoWUnit target = null;
                                                          int lowestThreat = 5000000;

                                                          List<WoWPlayer> members = StyxWoW.Me.GroupInfo.IsInRaid ? StyxWoW.Me.RaidMembers : StyxWoW.Me.PartyMembers;
                                                          foreach (WoWUnit attackableUnit in Units.AttackableUnits)
                                                          {
                                                              int aggroDifference = GetAggroDifferenceFor(attackableUnit, members);
                                                              if (aggroDifference > lowestThreat) continue;

                                                              lowestThreat = aggroDifference;
                                                              target = attackableUnit;
                                                          }

                                                          if (target != null) target.Target();
                                                      }),
                                                  new Action(context => Log.Info(string.Format("Finding lowest threat mob")))
                                                  )
                                ))

                                );
        }

        private Composite CreateFollowTheLeaderBehavior()
        {
            return
                // Follow leader - if we're not doing a lot of other stuff
                new Decorator(ret => 
                    Lootable == null && 
                    SkinnableMob == null && 
                    OreHarvestable == null && 
                    HerbHarvestable == null && 
                    !_forceAttacking &&
                    !_vendoring && 
                    !Me.HasAura("Eat") && !Me.HasAura("Drink") && 
                    ((Me.Combat && Setting.Instance.CombatFollow.OK()) || !Me.Combat),

                    new PrioritySelector(
                        CreateFollowBehavior()));
        }

        private Composite CreateCombatMovementAndFaceTarget()
        {
            return

                new PrioritySelector(

                    // Face target
                    new Decorator(ret => Setting.Instance.FaceTarget.OK() && !Misc.IsUserControllingMovement && !Me.IsMoving && !Me.IsSafelyFacing(CT),
                        new Sequence(
                            new Action(context => CT.Face())
                            )),

                    // Face target. Trying to ensure it faces faster when moving behind the target
                    new Decorator(ret => Setting.Instance.FaceTarget.OK() && !Misc.IsUserControllingMovement && Setting.Instance.AttackFromBehind.OK() && CT.CurrentTargetGuid != Me.Guid && Me.IsSafelyBehind(CT),
                        new Sequence(
                            new Action(context => CT.Face()),
                            new ActionAlwaysFail()
                            )),
                    
                    // Combat movement
                    new Decorator(ret => Setting.Instance.MoveToTarget.OK() && !Misc.IsUserControllingMovement && !Setting.Instance.CombatFollow.OK(),
                        new PrioritySelector(

                            // Position character behind the mob
                            new Decorator(ret => Setting.Instance.AttackFromBehind.OK() && CT.CurrentTargetGuid != Me.Guid && !Me.IsSafelyBehind(CT),
                                            new PrioritySelector(
                                                Movement.CreateMoveBehindTargetBehavior()
                                                )
                                ),

                            // Melee movement
                            new Decorator(ret => Helpers.Extensions.IsMeMeleeClass,
                                new PrioritySelector(
                                    Movement.CreateEnsureMovementStoppedWithinMelee(),
                                    Movement.CreateMoveToMeleeBehavior(true)
                                )),

                            // Caster movement
                            new Decorator(ret => !Helpers.Extensions.IsMeMeleeClass && Me.GotTarget,
                                new PrioritySelector(

                                    // LOS is all important
                                    Movement.CreateMoveToLosBehavior(),

                                    // If we're in melee range then REALLY stop moving
                                    Movement.CreateEnsureMovementStoppedWithinMelee(),

                                    // Move into range
                                    new Decorator(ret => CT.Distance >= 39,
                                        new PrioritySelector(
                                            Movement.CreateMoveToTargetBehavior(true, 35)
                                            )),

                                    // Stop moving if target < 35 yards
                                    new Decorator(ret => CT.Distance <= 39 && Me.IsMoving,
                                        new PrioritySelector(Movement.CreateEnsureMovementStoppedBehavior(35))
                                        )
                                ))
                        )
                    )

                



                );
        }


        #endregion

        #region Out of combat behavior
        private Composite CreateNotInCombatBehavior()
        {
            return
                new Decorator(ret => !Me.Combat,
                    new LockSelector(

                        // If our leader has a target then assist them
                        new Decorator(ret => Setting.Instance.Targeting.Contains("Assist") && Leader != null && Leader.GotTarget && Leader.Combat && (!Me.GotTarget || Me.GotTarget && CT.IsDead || Me.GotTarget && !CT.IsValidTarget() || Me.GotTarget && Leader.GotTarget && Leader.CurrentTargetGuid != CT.Guid),
                            new Sequence(
                                new Action(context => Leader.CurrentTarget.Target()),
                                new Action(context => Log.Info(string.Format("Assisting {0} with {1}", Leader.Name, Leader.CurrentTarget.Name), true)),
                                //new Action(context => StyxWoW.SleepForLagDuration()),
                                new Action(context => { if (!Me.GotTarget) return RunStatus.Running; return RunStatus.Success; }),
                                new Action(context => { if (!Me.IsMoving) CT.Face(); })
                                )
                            ),

                        // Pre combat buffs?
                        new Decorator(ret => Me.GotTarget && CT.IsAlive && CT.IsValidTarget(), RoutineManager.Current.PreCombatBuffBehavior),

                        
                        // Rest eat/drink?      
                        new PrioritySelector(RoutineManager.Current.RestBehavior),

                        // Force auto attack and move into range
                        CreateForceAutoAttack(),

                        // Do vendor repairs 
                        CreateVendorRepair(),

                        // Loot
                        CreateLootingBehavior(),

                        // Do questing 
                        CreateQuestBehavior(),

                        // Herb and Ore gathering
                        CreateHerbAndOreHarvesting(),

                        // Search for skinnable mobs
                        CreateSkinning(),

                        // Pickup quest objects if within 30 yards
                        CreateQuestObjectInteraction()


                        


                        ));
        }


        private bool TargetIsInteractable
        {
            get
            {
                WoWUnit target = null;
                if (Setting.Instance.FollowLeader.Contains("None"))
                {
                    if (Me.GotTarget) target = Me.CurrentTarget;
                }
                else if (Leader != null && Leader.GotTarget)
                {
                    target = Leader.CurrentTarget;
                }

                if (target == null) return false;

                if (target.QuestGiverStatus == QuestGiverStatus.TurnIn ||
                    target.QuestGiverStatus == QuestGiverStatus.Available ||
                    target.QuestGiverStatus == QuestGiverStatus.AvailableRepeatable ||
                    target.QuestGiverStatus == QuestGiverStatus.TurnInRepeatable ||
                    target.IsFlightMaster ||
                    target.IsBanker ||
                    target.IsInnkeeper ||
                    target.InteractType == WoWInteractType.TaxiPathAvailable)
                    return true;


                
                
                return false;
            }
        }

        private Composite CreateQuestBehavior()
        {
            return new Sequence(
                new Decorator(ret => TargetIsInteractable, new ActionAlwaysSucceed()),

                new Action(c => Leader.CurrentTarget.Target()),
                new WaitContinue(2,ret => StyxWoW.Me.CurrentTarget == Leader.CurrentTarget, new ActionAlwaysSucceed()),
                new Action(c =>
                        {
                            if (Me.CurrentTarget.WithinInteractRange) return RunStatus.Success;
                            
                            Navigator.MoveTo(Me.CurrentTarget.WorldLocation);
                            return RunStatus.Running;
                        }
                    ),
                new Action(c => Navigator.PlayerMover.MoveStop()),
                new Action(c => Me.CurrentTarget.Interact()),
                //new Action(c => StyxWoW.SleepForLagDuration()),

                //Most of the code here is done through events (Up above)
                new Wait(3, ret => false, new ActionAlwaysSucceed())
                );



        }

        private Composite CreateVendorRepair()
        {
            return

                new PrioritySelector(

                    // turn off vendoring (if its one)
                    new Decorator(ret => _vendoring, new Action(context => { _vendoring = false; })), 
                
                    // Vendor if we need to repair
                    new Decorator( ret =>Leader != null && Leader.GotTarget && Leader.CurrentTarget.IsRepairMerchant && Me.GetEstimatedRepairCost().TotalCoppers > 0,
                                      new Sequence(

                                          // Move to interact with vendor
                                          new DecoratorContinue(ret => !Leader.CurrentTarget.WithinInteractRange,
                                                                new Sequence(
                                                                    //new Action(context => QuestingAndInteraction.Repaired = false),
                                                                    new Action(context => _vendoring = true),
                                                                    new Action(context =>Log.Info("Moving to repair vendor", true)),
                                                                    new Action(context =>Navigator.MoveTo(WoWMovement.CalculatePointFrom(Leader.CurrentTarget.Location,(float) 0.70*Leader.CurrentTarget.InteractRange)))
                                                                    )),

                                          // Interact with vendor
                                          new Decorator(ret =>Leader.CurrentTarget.WithinInteractRange &&(Me.GetEstimatedRepairCost().TotalCoppers > 0),
                                              //(!Me.GotTarget || Me.GotTarget && CT.Guid != Leader.CurrentTarget.Guid),
                                              new Sequence(
                                                  new Action(context => Log.Info("Interacting with repair vendor", true)),
                                                  new Action(context => Leader.CurrentTarget.Interact()),
                                                  //new Action(context => StyxWoW.SleepForLagDuration()),
                                                  //new Action(context => Leader.CurrentTarget.Target()),

                                                  // Sleep for a while
                                                  new Action(context => StyxWoW.SleepForLagDuration()),
                                                  new Action(context => StyxWoW.SleepForLagDuration()),
                                                  new Action(context => StyxWoW.SleepForLagDuration()),

                                                  // repair from guild first, then from personal 
                                                  new Action(context => Lua.DoString("RepairAllItems(1)")),
                                                  new Action(context => Lua.DoString("RepairAllItems()")),
                                                  new Action(context => StyxWoW.SleepForLagDuration()),
                                                  new Action(context =>
                                                          {
                                                              if (Me.GetEstimatedRepairCost().TotalCoppers <= 0)
                                                                  _vendoring = false;
                                                          })
                                                  ))

                                          )
                                      ));
        }
        
        private Composite CreateForceAutoAttack()
        {
            return

                new PrioritySelector(

                    // Turn of force attacking if leader is not in combat. 
                    // This variable is used in the follow behavior to prevent it overriding our movment.
                    new Decorator(ret => _forceAttacking,
                                  new Decorator(ret => Setting.Instance.ForceAutoAttack.OK() && (Leader == null || !Leader.Combat || Me.Combat),
                                      new Action(context => _forceAttacking = false)
                                      )
                        ),

                    // Force the character into combat. This should help some other CCs where they don't want to engage if they are not attacked directly
                    new Decorator(ret => Setting.Instance.ForceAutoAttack.OK() && !Me.Combat && Leader != null && Leader.GotTarget && Leader.Combat && Me.GotTarget && (Helpers.Extensions.IsMeMeleeClass && !CT.IsWithinMeleeRange || !Helpers.Extensions.IsMeMeleeClass),// && !StyxWoW.Me.IsAutoAttacking && StyxWoW.Me.AutoRepeatingSpellId != 75),
                        new PrioritySelector(

                            // Melee
                            new Decorator(ret => Helpers.Extensions.IsMeMeleeClass && !CT.IsWithinMeleeRange,
                                          new Sequence(
                                              new DecoratorContinue(ret => !CT.IsWithinMeleeRange, // && !Me.IsMoving,
                                                new Sequence(
                                                    new Action(context => _forceAttacking = true),
                                                    new Action(context => TreeRoot.StatusText = "Forcing character into melee range"),
                                                    //new Action(context => Log.Info("Forcing autoattack, moving into melee range",true)),
                                                    new Action(context => Navigator.MoveTo(WoWMovement.CalculatePointFrom(CT.Location,(float) 0.5*CT.MeleeRange())))
                                                    )),

                                              //Movement.CreateMoveToMeleeBehavior(true),

                                              new Action(delegate { Lua.DoString("StartAttack()"); }),
                                              new Action(context => Navigator.Clear())
                                              )),

                            // Face the target as ranged has been presenting a few initial combat bugs.
                            new Decorator(ret=> !Helpers.Extensions.IsMeMeleeClass && !Me.IsSafelyFacing(CT) && !Me.IsMoving,
                                          new Action(context => CT.Face())
                                ),

                            // Ranged attackers just auto attack
                            new Decorator(ret => !Helpers.Extensions.IsMeMeleeClass && !Me.IsCasting && !Me.Combat,
                                          new Sequence(
                                              //new Action(context => Log.Info("Forcing autoattack, moving into caster range", true)),
                                              //new Action(context => Lua.DoString("StartAttack()")),
                                              new Action(context => Log.Info("Forcing CC into combat by casting the most basic spell")),
                                              new WaitContinue(5, ret => !Me.IsMoving, new ActionAlwaysSucceed()),
                                              new Action(c => Me.IsMoving ? RunStatus.Running : RunStatus.Success),
                                              new Action(context =>
                                                  {
                                                      switch (Me.Class)
                                                      {
                                                              case WoWClass.Priest:
                                                              SpellManager.Cast(Me.Specialization == WoWSpec.PriestShadow ? "Mind Blast" : "Smite");
                                                              break;

                                                              case WoWClass.Shaman:
                                                              SpellManager.Cast("Lightning Bolt");
                                                              break;

                                                              case WoWClass.Mage:
                                                              SpellManager.Cast("Frostfire Bolt");
                                                              break;

                                                              case WoWClass.Warlock:
                                                              SpellManager.Cast("Shadow Bolt");
                                                              break;

                                                              case WoWClass.Druid:
                                                              SpellManager.Cast("Wrath");
                                                              break;

                                                      }

                                                  }),
                                              new Action(context => Navigator.Clear())
                                              ))

                            )
                        ));

        }

        private Composite CreateQuestObjectInteraction()
        {
            return

                new Decorator(ret => Setting.Instance.QuestObjectInteract.OK() && 
                    !Me.IsCasting && !Me.IsChanneling &&
                    !Me.IsChanneling && !Me.Combat && QuestingAndInteraction.QuestItems(QuestingAndInteraction.DistanceForQuestItem).Count > 0,
                    new Sequence(
                        new Decorator(ret =>
                            {
                                if (QuestingAndInteraction.QuestItems(QuestingAndInteraction.DistanceForQuestItem).Count > 0)
                                {
                                    Log.Info("Quest object is visible: " + QuestingAndInteraction.QuestItems(QuestingAndInteraction.DistanceForQuestItem)[0].Name);
                                    QuestingAndInteraction.ObjectToGrab = QuestingAndInteraction.QuestItems(QuestingAndInteraction.DistanceForQuestItem)[0];
                                    return true;
                                }
                                
                                return false;
                            },

                                      
                            new ActionAlwaysSucceed()
                            ),


                            // Move closer to the mob
                            new DecoratorContinue(ret => QuestingAndInteraction.ObjectToGrab.Distance > QuestingAndInteraction.ObjectToGrab.InteractRange * 0.85f,
                                new Sequence(
                                //new Action(context => Log.Info("Moving closer ... ")),
                                Movement.CreateMoveToLocationBehavior(ret => QuestingAndInteraction.ObjectToGrab.Location, true, ret => QuestingAndInteraction.ObjectToGrab.InteractRange * 0.55f)
                                )),

                            // Wait a maximum of 10 seconds to get into range or if we get into combat
                            new WaitContinue(10, ret => (QuestingAndInteraction.ObjectToGrab.Distance < QuestingAndInteraction.ObjectToGrab.InteractRange * 0.80f || Me.Combat), new ActionAlwaysSucceed()),
                                        
                            // if we're moving then and we're in interact range then stop
                            new DecoratorContinue(ret => QuestingAndInteraction.ObjectToGrab.Distance <= QuestingAndInteraction.ObjectToGrab.InteractRange * 0.85f,
                                new PrioritySelector(
                                    Movement.CreateEnsureMovementStoppedBehavior(),
                                    // wait up to 2 seconds for us to register as stopped moving. WoW and HB tend to see this differently
                                    new WaitContinue(2, ret => !Me.IsMoving, new ActionAlwaysSucceed())
                                    )
                            ),
                                        
                                      
                            // Make sure we're not moving. Just in case
                            new WaitContinue(5, ret => !Me.IsMoving, new ActionAlwaysSucceed()),

                            // Finally interact with it
                            new Action(c =>
                                {
                                    _lootableGameObject = QuestingAndInteraction.ObjectToGrab;
                                    QuestingAndInteraction.ObjectToGrab.Interact();
                                }),
                
                            // wait for casting to start, max 2 seconds
                            new Wait(2, ret => Me.IsCasting, new ActionAlwaysSucceed()),
                            
                            // wait while we are casting. Max 10 seconds
                            new WaitContinue(10, ret => !Me.IsCasting, new ActionAlwaysSucceed()),

                            // Casting has finished, but wait 2 more seconds for looting to occur
                            new WaitContinue(1, ret => false, new ActionAlwaysSucceed()),
                            new Wait(1, ret => Me.Looting, new ActionAlwaysSucceed()),
                            new WaitContinue(5, ret => !Me.Looting, new ActionAlwaysSucceed()),

                            // Black list so we don't try to loot again for min 1 minute
                            new Action(c => Blacklist.Add(_lootableGameObject.Guid, BlacklistFlags.Loot, TimeSpan.FromMinutes(2))),
                            new Action(c => { _lootableGameObject = null; }),
                                        
                            // clear all navigation as we tend to jump after we've looted
                            new Action(c => { Navigator.Clear(); return RunStatus.Success;})




                        )
                    );
        }

        private WoWGameObject _lootableGameObject = null;
        private Composite CreateHerbAndOreHarvesting()
        {
            return

                new PrioritySelector(

                    // Harvest Ore - secondary to skinning
                      new Decorator(ret => Setting.Instance.InteractMining.OK() && !Me.IsCasting && !Me.IsChanneling && !Me.Combat && !Me.Looting && OreHarvestable != null,
                            new Sequence(

                                new Action(c => Log.Info("Ore found: " + OreHarvestable.Name)),

                                // Move closer to the mob
                                new DecoratorContinue(ret => OreHarvestable.Distance > OreHarvestable.InteractRange * 0.85f,
                                    new Sequence(

                                    Movement.CreateMoveToLocationBehavior(ret => OreHarvestable.Location, true, ret => OreHarvestable.InteractRange * 0.55f)
                                    )),

                            // Wait a maximum of 10 seconds to get into range or if we get into combat
                            new WaitContinue(10, ret => (OreHarvestable.Distance < OreHarvestable.InteractRange * 0.80f || Me.Combat), new ActionAlwaysSucceed()),

                            // if we're moving then and we're in interact range then stop
                            new DecoratorContinue(ret => OreHarvestable.Distance <= OreHarvestable.InteractRange * 0.85f,
                                new PrioritySelector(
                                    Movement.CreateEnsureMovementStoppedBehavior(),

                                    // wait up to 2 seconds for us to register as stopped moving. WoW and HB tend to see this differently
                                    new WaitContinue(2, ret => !Me.IsMoving, new ActionAlwaysSucceed())
                                    )
                            ),


                                     
                            // Make sure we're not moving. Just in case
                            new WaitContinue(5, ret => !Me.IsMoving, new ActionAlwaysSucceed()),

                            // Finally interact with it
                            new Action(c =>
                                {
                                    _lootableGameObject = OreHarvestable;
                                    OreHarvestable.Interact();
                                }),

                            // wait for casting to start, max 2 seconds
                            new Wait(2, ret => Me.IsCasting, new ActionAlwaysSucceed()),

                            // wait while we are casting. Max 10 seconds
                            new WaitContinue(10, ret => !Me.IsCasting, new ActionAlwaysSucceed()),

                            // Casting has finished, but wait 2 more seconds for looting to occur
                            new WaitContinue(1, ret => false, new ActionAlwaysSucceed()),
                            new Wait(1, ret => Me.Looting, new ActionAlwaysSucceed()),
                            new WaitContinue(5, ret => !Me.Looting, new ActionAlwaysSucceed()),

                            // Black list so we don't try to loot again for min 1 minute
                            new Action(c => Blacklist.Add(_lootableGameObject.Guid, BlacklistFlags.Loot, TimeSpan.FromMinutes(2))),
                            new Action(c => { _lootableGameObject = null; }),

                            // clear all navigation as we tend to jump after we've looted
                            new Action(c => { Navigator.Clear(); return RunStatus.Success; })


                            )),
                  

                // Harvest Herbs
                  new Decorator(ret => Setting.Instance.InteractHerbalism.OK() && !Me.IsCasting && !Me.IsChanneling && !Me.Combat && !Me.Looting && HerbHarvestable != null,
                            new Sequence(

                                new Action(c => Log.Info("Herb found: " + HerbHarvestable.Name)),

                                // Move closer to the mob
                                new DecoratorContinue(ret => HerbHarvestable.Distance > HerbHarvestable.InteractRange * 0.85f,
                                    new Sequence(

                                    Movement.CreateMoveToLocationBehavior(ret => HerbHarvestable.Location, true, ret => HerbHarvestable.InteractRange * 0.55f)
                                    )),

                            // Wait a maximum of 10 seconds to get into range or if we get into combat
                            new WaitContinue(10, ret => (HerbHarvestable.Distance < HerbHarvestable.InteractRange * 0.80f || Me.Combat), new ActionAlwaysSucceed()),

                            // if we're moving then and we're in interact range then stop
                            new DecoratorContinue(ret => HerbHarvestable.Distance <= HerbHarvestable.InteractRange * 0.85f,
                                new PrioritySelector(
                                    Movement.CreateEnsureMovementStoppedBehavior(),
                
                                    // wait up to 2 seconds for us to register as stopped moving. WoW and HB tend to see this differently
                                    new WaitContinue(2, ret => !Me.IsMoving, new ActionAlwaysSucceed())
                                    )
                            ),



                            
                                     
                            // Make sure we're not moving. Just in case
                            new WaitContinue(5, ret => !Me.IsMoving, new ActionAlwaysSucceed()),

                            // Finally interact with it
                            new Action(c =>
                                {
                                    _lootableGameObject = HerbHarvestable;
                                    HerbHarvestable.Interact();
                                }),

                            // wait for casting to start, max 2 seconds
                            new Wait(2, ret => Me.IsCasting, new ActionAlwaysSucceed()),

                            // wait while we are casting. Max 10 seconds
                            new WaitContinue(10, ret => !Me.IsCasting, new ActionAlwaysSucceed()),

                            // Casting has finished, but wait 2 more seconds for looting to occur
                            new WaitContinue(1, ret => false, new ActionAlwaysSucceed()),
                            new Wait(1, ret => Me.Looting, new ActionAlwaysSucceed()),
                            new WaitContinue(5, ret => !Me.Looting, new ActionAlwaysSucceed()),


                            // Black list so we don't try to loot again for min 1 minute
                            new Action(c => Blacklist.Add(_lootableGameObject.Guid, BlacklistFlags.Loot, TimeSpan.FromMinutes(2))),
                            new Action(c => { _lootableGameObject = null; }),

                            // clear all navigation as we tend to jump after we've looted
                            new Action(c => { Navigator.Clear(); return RunStatus.Success; })

                            ))




                    );
        }

        private WoWUnit _skinnableMob = null;
        private Composite CreateSkinning()
        {
            return
                new PrioritySelector(
                    
                    new Throttle(TimeSpan.FromSeconds(2),
                        new Decorator(ret => Setting.Instance.InteractSkinning.OK() && !Me.IsCasting && !Me.IsChanneling && !Me.Combat && !Me.Looting && SkinnableMob != null,
                            new Sequence(

                                new Action(c => Log.Info("Skinnable mob found: " + SkinnableMob.Name)),
                                         
                                // Move closer to the mob
                                new DecoratorContinue(ret => SkinnableMob.Distance > SkinnableMob.InteractRange * 0.85f,
                                    new Sequence(
                                    //new Action(context => Log.Info("Moving closer ... ")),
                                    Movement.CreateMoveToLocationBehavior(ret=> SkinnableMob.Location, true, ret=> SkinnableMob.InteractRange * 0.55f)
                                    )),

                            // Wait a maximum of 10 seconds to get into range or if we get into combat
                            new WaitContinue(10, ret => (SkinnableMob.Distance < SkinnableMob.InteractRange * 0.80f || Me.Combat), new ActionAlwaysSucceed()),
                                        
                            // if we're moving then and we're in interact range then stop
                            new DecoratorContinue(ret => SkinnableMob.Distance <= SkinnableMob.InteractRange * 0.85f,
                                new PrioritySelector(
                                    Movement.CreateEnsureMovementStoppedBehavior(),
                                    // wait up to 2 seconds for us to register as stopped moving. WoW and HB tend to see this differently
                                    new WaitContinue(2, ret => !Me.IsMoving, new ActionAlwaysSucceed())
                                    )
                            ),
                            
                                     
                            // Make sure we're not moving. Just in case
                            new WaitContinue(5, ret => !Me.IsMoving, new ActionAlwaysSucceed()),

                            // Finally interact with it
                            new Action(c => { _lootableObject = SkinnableMob; SkinnableMob.Interact(); }),

                           // wait for casting to start, max 2 seconds
                            new Wait(2, ret => Me.IsCasting, new ActionAlwaysSucceed()),

                            // wait while we are casting. Max 10 seconds
                            new WaitContinue(10, ret => !Me.IsCasting, new ActionAlwaysSucceed()),

                            // Casting has finished, but wait 2 more seconds for looting to occur
                            new WaitContinue(1, ret => false, new ActionAlwaysSucceed()),
                            new Wait(1, ret => Me.Looting, new ActionAlwaysSucceed()),
                            new WaitContinue(5, ret => !Me.Looting, new ActionAlwaysSucceed()),

                            // Black list so we don't try to loot again for min 1 minute
                            new Action(c => Blacklist.Add(_lootableObject.Guid, BlacklistFlags.Loot, TimeSpan.FromMinutes(2))),
                            new Action(c => { _lootableObject = null; }),

                            // clear all navigation as we tend to jump after we've looted
                            new Action(c => { Navigator.Clear(); return RunStatus.Success; })


                            ))
                        ));
        }

        //private WoWUnit _lootMob = null;

        private WoWUnit Lootable
        {
            get
            {
                if (Setting.Instance.Loot.OK())
                {
                    return ObjectManager.GetObjectsOfTypeFast<WoWUnit>().Where(o =>o.Distance < Setting.Instance.InteractMaxRange && o.Lootable && o.CanLoot &&!Blacklist.Contains(o.Guid, BlacklistFlags.Loot)).OrderBy(o => o.Distance).FirstOrDefault();
                }

                return null;
            }
        }

        private WoWUnit SkinnableMob
        {
            get
            {
                if (Setting.Instance.InteractSkinning.OK())
                {
                    return ObjectManager.GetObjectsOfTypeFast<WoWUnit>().Where(o => o.Distance < Setting.Instance.InteractMaxRange && o.Skinnable && o.CanSkin && !Blacklist.Contains(o.Guid, BlacklistFlags.Node)).OrderBy(o => o.Distance).FirstOrDefault();
                }

                return null;
            }
        }

        private WoWGameObject HerbHarvestable
        {
            get
            {
                if (Setting.Instance.InteractHerbalism.OK())
                {
                    return QuestingAndInteraction.HerbalismItems(Setting.Instance.InteractMaxRange).FirstOrDefault();
                }

                return null;
            }
        }

        private WoWGameObject OreHarvestable
        {
            get
            {
                if (Setting.Instance.InteractMining.OK())
                {
                    return QuestingAndInteraction.OreItems(Setting.Instance.InteractMaxRange).FirstOrDefault();
                }

                return null;
            }
        }

        private WoWUnit _lootableObject = null;
        private Composite CreateLootingBehavior()
        {
            return
                new PrioritySelector(

                    new Decorator(ret => Setting.Instance.Loot.OK() && !Me.IsCasting && !Me.IsChanneling && !Me.Combat && !Me.Looting && Lootable != null && Me.FreeNormalBagSlots <= 3,
                                  new PrioritySelector(
                                      new Action(c => Log.FailLog("Bags are full, no longer attempting to loot mobs."))
                                      )
                        ),

                    new Throttle(TimeSpan.FromSeconds(2),
                         new Decorator(ret => Setting.Instance.Loot.OK() && !Me.IsCasting && !Me.IsChanneling && !Me.Combat && !Me.Looting && Lootable != null && Me.FreeNormalBagSlots > 3,
                            new Sequence(

                                new Action(c => Log.Info("Lootable object found: " + Lootable.Name)),

                                // Move closer to the mob
                                new DecoratorContinue(ret => Lootable.Distance > Lootable.InteractRange * 0.85f,
                                    new Sequence(
                                    Movement.CreateMoveToLocationBehavior(ret => Lootable.Location, true, ret => Lootable.InteractRange * 0.55f)
                                    )),

                            // Wait a maximum of 10 seconds to get into range or if we get into combat
                            new WaitContinue(10, ret => (Lootable.Distance < Lootable.InteractRange * 0.80f || Me.Combat), new ActionAlwaysSucceed()),

                            // if we're moving then and we're in interact range then stop
                            new DecoratorContinue(ret => Lootable.Distance <= Lootable.InteractRange * 0.85f,
                                new PrioritySelector(
                                    Movement.CreateEnsureMovementStoppedBehavior(),
                
                                    // wait up to 2 seconds for us to register as stopped moving. WoW and HB tend to see this differently
                                    new WaitContinue(2, ret => !Me.IsMoving, new ActionAlwaysSucceed())
                                    )
                            ),

                            // One final check to ensure we've stopped moving
                            new Action(c => Me.IsMoving ? RunStatus.Running : RunStatus.Success),

                            // Finally interact with it
                            new Action(c => { _lootableObject = Lootable; Lootable.Interact(); }),

                            // wait for casting to start, max 2 seconds
                            new Wait(2, ret => Me.IsCasting, new ActionAlwaysSucceed()),

                            // wait while we are casting. Max 10 seconds
                            new WaitContinue(10, ret => !Me.IsCasting, new ActionAlwaysSucceed()),

                            // Casting has finished, but wait 2 more seconds for looting to occur
                            new WaitContinue(1, ret => false, new ActionAlwaysSucceed()),
                            new Wait(1, ret => Me.Looting, new ActionAlwaysSucceed()),
                            new WaitContinue(5, ret => !Me.Looting, new ActionAlwaysSucceed()),

                            // Black list so we don't try to loot again for min 1 minute
                            new Action(c => Blacklist.Add(_lootableObject.Guid, BlacklistFlags.Loot, TimeSpan.FromMinutes(2))),
                            new Action(c => { _lootableObject = null; }),

                            // clear all navigation as we tend to jump after we've looted
                            new Action(c => { Navigator.Clear(); return RunStatus.Success; })


                            ))
                        ));
        }
        
        #endregion

        #endregion

        #region Threat difference (from Singular)
        private static int GetAggroDifferenceFor(WoWUnit unit, IEnumerable<WoWPlayer> partyMembers)
        {
            uint myThreat = unit.ThreatInfo.ThreatValue;
            uint highestParty = (from p in partyMembers
                                 let tVal = unit.GetThreatInfoFor(p).ThreatValue
                                 orderby tVal descending
                                 select tVal).FirstOrDefault();

            int result = (int)myThreat - (int)highestParty;
            return result;
        }
        #endregion
        
        #region Botbase Start, Stop and Initialise
        public override void Start()
        {
            _root = null;

            Lua.Events.AttachEvent("PARTY_MEMBERS_CHANGED", HandlePartyMembersChanged);
            BotEvents.Player.OnMapChanged += Player_OnMapChanged;


            QuestingAndInteraction.AttachQuestingEvents();

            Lua.Events.AttachEvent("START_LOOT_ROLL", HandleLootRoll);
            Lua.Events.AttachEvent("CONFIRM_LOOT_ROLL", HandleConfirmLootRoll);
            Lua.Events.AttachEvent("CONFIRM_DISENCHANT_ROLL", HandleConfirmLootRoll);

            Lua.Events.AttachEvent("PARTY_INVITE_REQUEST", OnPartyInvite);
            Lua.Events.AttachEvent("LFG_ROLE_CHECK_SHOW", OnLfgRoleCheck);
            Lua.Events.AttachEvent("LFG_PROPOSAL_SHOW", OnLfgProposalShow);
            Lua.Events.AttachEvent("RESURRECT_REQUEST", OnRecurrectionHandler);
            Lua.Events.AttachEvent("READY_CHECK", OnReadyCheckHandler);

            // Set the number of ticks per second the botbase will function at
            _oldTps = TreeRoot.TicksPerSecond;
            TreeRoot.TicksPerSecond = (byte)Setting.Instance.TicksPerSecond;
            

            // Say who the leader is
            if (Leader != null)
            {
                Log.Info("Follow target set to " + Leader.Name);
            }

            // Register chat events
            if (Setting.Instance.AllowCommands.Contains("Always"))
            {
                Log.Info("Registering chat commands");
                //Lua.Events.AttachEvent("CHAT_MSG_PARTY", Handler);
                //Lua.Events.AttachEvent("CHAT_MSG_SAY", Handler);
                //Lua.Events.AttachEvent("CHAT_MSG_WHISPER", Handler);
                //Lua.Events.AttachEvent("CHAT_MSG_RAID", Handler);
                
                Chat.Say += NewCommand;
                Chat.Party += NewCommand;
                Chat.Whisper += NewCommand;
                Chat.Raid += NewCommand;
            }

            // Tell HB what it can and cannot pulse
            _pulseFlags = PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel | PulseFlags.Targeting;

            if (Setting.Instance.Plugins.OK())
            {
                _pulseFlags |= PulseFlags.Plugins;
            }

            RegisterHotkeys();
            GlobalSettings.Instance.LogoutForInactivity = false;
        }

        


        public override void Stop()
        {
            TreeRoot.TicksPerSecond = _oldTps;
            _isInitialized = false;

            QuestingAndInteraction.DetatchQuestingEvents();

            Lua.Events.DetachEvent("START_LOOT_ROLL", HandleLootRoll);
            Lua.Events.DetachEvent("CONFIRM_LOOT_ROLL", HandleConfirmLootRoll);
            Lua.Events.DetachEvent("CONFIRM_DISENCHANT_ROLL", HandleConfirmLootRoll);

            Lua.Events.DetachEvent("LFG_ROLE_CHECK_SHOW", OnLfgRoleCheck);
            Lua.Events.DetachEvent("LFG_PROPOSAL_SHOW", OnLfgProposalShow);
            Lua.Events.DetachEvent("RESURRECT_REQUEST", OnRecurrectionHandler);
            Lua.Events.DetachEvent("READY_CHECK", OnReadyCheckHandler);
            Lua.Events.DetachEvent("PARTY_MEMBERS_CHANGED", HandlePartyMembersChanged);

            BotEvents.Player.OnMapChanged -= Player_OnMapChanged;
            

            Log.Info("Unregistering chat commands");
            Chat.Say -= NewCommand;
            Chat.Whisper -= NewCommand;
            Chat.Party -= NewCommand;
            Chat.Raid -= NewCommand;
            
            //Lua.Events.DetachEvent("CHAT_MSG_PARTY", Handler);
            //Lua.Events.DetachEvent("CHAT_MSG_SAY", Handler);
            //Lua.Events.DetachEvent("CHAT_MSG_WHISPER", Handler);
            //Lua.Events.DetachEvent("CHAT_MSG_RAID", Handler);

            GlobalSettings.Instance.LogoutForInactivity = true;
            UnregisterHotkeys();
        }

        private static int ResultNumber(String settingName)
        {
            int result;

            string numFromString = Regex.Match(settingName, @"\d+").Value;
            return Int32.TryParse(numFromString, out result) ? result : 0;
        }

        public override void Initialize()
        {
            Setting.Instance.Load();
            ProfileManager.LoadEmpty();
            _isInitialized = false;

            Log.InitLog(Name + " [" + _version + "]");
            
            
            base.Initialize();
        }
        #endregion

        #region Lock Selector
        // perform all the parsed logic within a frame lock

        private class LockSelector : PrioritySelector
        {
            public LockSelector(params Composite[] children) : base(children){}

            public override RunStatus Tick(object context)
            {
                using (StyxWoW.Memory.AcquireFrame())
                {

                    try
                    {
                        return base.Tick(context);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                return RunStatus.Failure;
            }
        }

        #endregion

        #region Various event handlers

        /// <summary> Handles the 'CONFIRM_LOOT_ROLL' and 'CONFIRM_DISENCHANT_ROLL' event. Fires when you try to roll "need" or "greed" for an item which Binds on Pickup. </summary>
        private void HandleConfirmLootRoll(object sender, LuaEventArgs e)
        {
            if (Setting.Instance.LootRolling.Contains("Ignore")) return;

            double rollId = (double)e.Args[0];
            double rollType = (double)e.Args[1];

            Lua.DoString("ConfirmLootRoll({0},{1})", rollId, rollType);
        }

        private void HandleLootRoll(object sender, LuaEventArgs e)
        {
            if (Setting.Instance.LootRolling.Contains("Ignore")) return;

            string rollId = e.Args[0].ToString();

            switch (Setting.Instance.LootRolling)
            {
                case "Greed":
                    Log.Info("Rolling GREED on loot");
                    Lua.DoString(string.Format("RollOnLoot({0}, 2)", rollId));
                    break;

                case "Need":
                    Log.Info("Rolling NEED on loot");
                    Lua.DoString(string.Format("RollOnLoot({0}, 1)", rollId));
                    break;

                case "Disenchant":
                    Log.Info("Rolling DISENCHANT on loot");
                    bool canDisenchant = Lua.GetReturnVal<bool>(string.Format("return GetLootRollItemInfo({0})", rollId), 7);
                    if (canDisenchant)
                    {
                        Lua.DoString(string.Format("RollOnLoot({0}, 3)", rollId));
                    }
                    else
                    {
                        Log.Info("Unable to roll Disenchant on the loot, rolling Greed");
                        Lua.DoString(string.Format("RollOnLoot({0}, 2)", rollId));
                    }
                    break;

                case "Pass":
                    Log.Info("Rolling PASS on loot");
                    Lua.DoString(string.Format("RollOnLoot({0}, 0)", rollId));
                    break;

            }


        }


        private void OnPartyInvite(object sender, LuaEventArgs args)
        {
            if (Leader != null)
            {
                string inviteFrom = args.Args[0].ToString();
                
                if (Leader.Name == inviteFrom)
                {
                    Log.Info(string.Format("Accepting group invite from {0}", inviteFrom));
                    Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\")");
                }

            }
        }


        private void OnReadyCheckHandler(object sender, LuaEventArgs args)
        {
            if (Setting.Instance.AutomationReadyCheck.OK())
            {
                Log.Info("Ready check has been initiated by " + args.Args[0]);
                Lua.DoString("RunMacroText(\"/click ReadyCheckFrameYesButton\")");
            }
        }

        private void OnRecurrectionHandler(object sender, LuaEventArgs args)
        {

            if (Setting.Instance.AutomationRecurrection.OK())
            {
                Log.Info("Resurrection has been cast on you");
                Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\")");
            }


        }

        public static void OnLfgProposalShow(object obj, LuaEventArgs args)
        {
            Log.Info("OnLfgProposalShow .... ");
            Lua.DoString("AcceptProposal()");
        }

        public static void OnLfgRoleCheck(object obj, LuaEventArgs args)
        {
            Log.Info("OnLfgRoleCheck .... ");
            Lua.DoString("CompleteLFGRoleCheck(true)");
        }

        private void HandlePartyMembersChanged(object sender, LuaEventArgs args)
        {
            _leader = null;
        }

        private void Player_OnMapChanged(BotEvents.Player.MapChangedEventArgs args)
        {
            _root = null;
        }
        #endregion

        #region Follow Behavior


        static void _leader_OnInvalidate()
        {
            //_leader.OnInvalidate -= _leader_OnInvalidate;
            _leader = null;
        }
        private static WoWUnit _leader;
        private static string _followLeaderName = "";
        public static WoWUnit Leader
        {
            get
            {
                if (Setting.Instance.FollowLeader.Contains("Never")) return Me;
                //if (Setting.Instance.FollowLeader.Contains("None")) return null;
                // return our cached leader if its been less than 4 seconds
                //if (_leader != null && _leader.IsValid && _leader.IsAlive && !Timers.AutoExpire("Find a tank", 4000)) return _leader;


                // We've specifically identified a player to follow
                if (!Setting.Instance.FollowLeader.Contains("Never") && !Setting.Instance.FollowLeader.Contains("None") && !Setting.Instance.FollowLeader.Contains("Automatic"))
                {
                    // If the leader has not changed and they are valid return our old leader
                    if (_leader != null && _leader.IsValid && _leader.IsAlive && _leader.Name == Setting.Instance.FollowLeader) return _leader;

                    // Find a suitable leader
                    if (_leader == null || !_leader.IsValid || !_leader.IsAlive || _leader.Name != Setting.Instance.FollowLeader)
                    {
                        WoWUnit unit = ObjectManager.GetObjectsOfType<WoWPlayer>(false, false).FirstOrDefault(u => u.IsAlive && u.Distance < 100 && u.Name == Setting.Instance.FollowLeader);
                        if (unit != null)
                        {
                            TreeRoot.StatusText = "Leader choosen by name: " + unit.Name;
                            //_followLeaderName = unit.Name;
                            RaFHelper.SetLeader(unit.Guid);
                            _leader = unit;

                            //_leader.OnInvalidate += _leader_OnInvalidate;
                            return unit;
                        }
                    }
                }

                // We're here because we don't have a suitable leader. 
                if (Me.GroupInfo.IsInRaid && (!Setting.Instance.FollowLeader.Contains("Never") && !Setting.Instance.FollowLeader.Contains("None")))
                {
                    // If the leader has not changed and they are valid return our old leader
                    if (_leader != null && _leader.IsValid && _leader.IsAlive && !Timers.AutoExpire("Find a tank",4000)) return _leader;

                    for (int i = 1; i < Me.GroupInfo.RaidMembers.Count(); i++)
                    {
                        string role = Lua.GetReturnVal<string>(string.Format("return UnitGroupRolesAssigned('raid{0}')", i), 0);
                        if (role != "TANK") continue;

                        WoWUnit possibleLeader = ObjectManager.GetObjectByGuid<WoWPlayer>(Me.GetRaidMemberGuid(i - 1));

                        // Make sure they are valid and alive
                        if (possibleLeader != null && !possibleLeader.IsAlive) continue;
                        if (possibleLeader != null)
                        {
                            _leader = possibleLeader;
                            //_followLeaderName = possibleLeader.Name;
                            RaFHelper.SetLeader(_leader.Guid);
                            TreeRoot.StatusText = "Leader choosen by Raid tank role: " + possibleLeader.Name;

                            //_leader.OnInvalidate += _leader_OnInvalidate;
                            return _leader;
                        }
                        break;
                    }
                }

                if ((Me.GroupInfo.IsInParty || Me.GroupInfo.IsInLfgParty) && (!Setting.Instance.FollowLeader.Contains("Never") && !Setting.Instance.FollowLeader.Contains("None")))
                {
                    // If the leader has not changed and they are valid return our old leader
                    if (_leader != null && _leader.IsValid && _leader.IsAlive && !Timers.AutoExpire("Find a tank", 4000)) return _leader;

                    for (int i = 1; i < Me.GroupInfo.PartyMembers.Count(); i++)
                    {
                        string role = Lua.GetReturnVal<string>(string.Format("return UnitGroupRolesAssigned('party{0}')", i), 0);
                        if (role != "TANK") continue;

                        WoWUnit possibleLeader = ObjectManager.GetObjectByGuid<WoWPlayer>(Me.GetRaidMemberGuid(i - 1));

                        // Make sure they are valid and alive
                        if (possibleLeader != null && !possibleLeader.IsAlive) continue;
                        if (possibleLeader != null)
                        {
                            _leader = possibleLeader;
                            //_followLeaderName = possibleLeader.Name;
                            TreeRoot.StatusText = "Leader choosen by Party tank role: " + possibleLeader.Name;
                            RaFHelper.SetLeader(_leader.Guid);
                            _leader.OnInvalidate += _leader_OnInvalidate;
                            return _leader;
                        }
                        break;
                    }
                }

                // If we here then just take the first alive player we can find

                try
                {
                    WoWPlayer lastResortLeader = ObjectManager.GetObjectsOfType<WoWPlayer>(false, false).FirstOrDefault(p => p.IsAlive);
                    TreeRoot.StatusText = "Leader choosen by last resort: " + lastResortLeader.Name;
                    _leader = lastResortLeader;
                    RaFHelper.SetLeader(_leader.Guid);
                    return lastResortLeader;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return null;


/*
                return null;
*/
            }
        }

        [UsedImplicitly] private static bool _isInitialized;

        private static int FollowDistance
        {
            get
            {
                return Setting.Instance.CombatFollow.OK() ? Setting.Instance.CombatFollowDistance : Setting.Instance.FollowDistance;
            }
        }
        private static Composite CreateFollowBehavior()
        {
            return new PrioritySelector(

                // Reasons NOT to follow leader
                new Decorator(ret => Setting.Instance.FollowLeader.Contains("Never") || Setting.Instance.FollowLeader.Contains("None"), new ActionAlwaysSucceed()),
                new Decorator(ret => Misc.IsUserControllingMovement, new ActionAlwaysSucceed()),
                new Decorator(ret => WaitingHere, new ActionAlwaysSucceed()),
                new Decorator(ret => Leader == null, new ActionAlwaysSucceed()),
                new Decorator(ret => Me.IsDead || Me.IsGhost || Me.CurrentHealth <= 1, new ActionAlwaysSucceed()),
                
                // Mount up and Dismount as required
                new Decorator(ret => NeedToMount(), new Action(delegate { WaitForMount();})),
                new Decorator(ret => NeedToDismount(), new Action(delegate { WaitForDismount(); })),


                // Finally just move to our follow target
                //new Decorator(ret => (FollowTarget != null && FollowTarget.Distance > FollowDistance || (FollowTarget != null && !FollowTarget.InLineOfSpellSight)) && (Navigator.CanNavigateFully(Me.Location, FollowTarget.Location) || !Me.IsFlying || !Me.IsSwimming),
                new Decorator(ret => (Leader != null && Leader.Distance > FollowDistance || (Leader != null && !Leader.InLineOfSpellSight)) && (Navigator.CanNavigateFully(Me.Location, Leader.Location) || !Me.IsFlying || !Me.IsSwimming),
                    new Action(context =>
                        {
                            WoWPoint ftLocation = Leader.Location;
                            if (ftLocation.DistanceSqr(_lastDest) > 1 || !Me.IsMoving)
                            {
                                _lastDest = ftLocation;
                                WoWPoint newPoint = WoWMovement.CalculatePointFrom(ftLocation, (float) 0.85* FollowDistance);

                                if (Me.Mounted || Me.IsFlying || Me.IsSwimming && (Leader.IsFlying || Leader.IsSwimming))
                                {
                                    TreeRoot.StatusText = "Flying / Swimming to our leader";
                                    Flightor.MoveTo(newPoint);
                                }
                                else
                                {
                                    TreeRoot.StatusText = "Following our leader";
                                    Navigator.MoveTo(newPoint);
                                }

                                _botMovement = true;
                            }

                            return RunStatus.Success;
                        })),


                // Do we need to stop moving?
                new Decorator(ret => Me.IsMoving && _botMovement,
                    new Action(delegate
                    {
                        _botMovement = false;
                        while (IsGameStable() && Me.IsMoving)
                        {
                            TreeRoot.StatusText = "";
                            WoWMovement.MoveStop();
                            if (Me.IsMoving) { StyxWoW.SleepForLagDuration(); }
                        }

                        return RunStatus.Success;
                    }))



                );
        }

        public static bool IsGameStable()
        {
            return StyxWoW.IsInGame && Me != null && Me.IsValid;
        }
        

        #region Mounting

/*
        private static int _followDistance = 15;
*/
        /// <summary>
        /// Wait for us to stop moving. Max time is our ping
        /// </summary>
        public static void WaitForStop()
        {
            WoWMovement.MoveStop(WoWMovement.MovementDirection.All);
            WoWMovement.MoveStop();
            Navigator.PlayerMover.MoveStop();

            do { StyxWoW.SleepForLagDuration(); }
            while (IsGameStable() && Me.CurrentHealth > 1 && Me.IsMoving);
        }

        /// <summary>
        /// Wait for us to mount up. Max time is our ping
        /// </summary>
        public static void WaitForMount()
        {
            if (Me.Combat || Me.IsIndoors) return;

            WaitForStop();

            Stopwatch timeOut = new Stopwatch();
            timeOut.Start();

            if (!Mount.Mounts.Any() || !Mount.CanMount()) return;

            TreeRoot.StatusText = "Leader has mounted so we'll do the same";
            Mount.MountUp(LazyLocationRetriever);
            StyxWoW.SleepForLagDuration();

            //_followDistance = Setting.Instance.FollowDistance;
            //Setting.Instance.FollowDistance = 5;

            while (IsGameStable() && Me.CurrentHealth > 1 && Me.IsCasting)
            {
                Thread.Sleep(75);
            }

        }

        /// <summary>
        /// Wait for us to dismount. Max time is our ping
        /// </summary>
        public static void WaitForDismount()
        {
            while (IsGameStable() && Me.CurrentHealth > 1 && Me.Mounted)
            {
                TreeRoot.StatusText = "Dismounting";
                Lua.DoString("Dismount()");
                StyxWoW.SleepForLagDuration();
                //Setting.Instance.FollowDistance = _followDistance;
            }
        }

        public static WoWPoint LazyLocationRetriever()
        {
            return WoWPoint.Empty;
        }

        public static bool NeedToMount()
        {
            return !Me.Mounted && Leader != null && (Leader.Distance > 60 || Leader.Mounted) && Me.IsOutdoors && Mount.CanMount();
        }

        public static bool NeedToDismount()
        {
            return Me.Mounted && Leader != null && Leader.Distance <= 30 && !Leader.Mounted;
        }
        #endregion

        #endregion

        #region Chat Commands

        public static bool WaitingHere = false;
        private static void NewCommand(Chat.ChatLanguageSpecificEventArgs e)
        {
            if (Leader == null) return;
            if (e.Author != Leader.Name) return;

            bool validCommand = false;

            // don't follow
            string[] nofollowCommands = Setting.Instance.ChatNoFollowCommand.Split('|');
            foreach (string s in nofollowCommands.Where(s => e.Message.ToUpper() == s.ToUpper().Trim()))
            {
                _followLeaderName = Setting.Instance.FollowLeader;
                WaitingHere = true;
                Log.Info("Not following leader. You *MUST* type the follow command to re-enable this");
            }

            // slash follow 
            string[] justfollowCommands = Setting.Instance.ChatSlashFollowCommand.Split('|');
            foreach (string s in justfollowCommands.Where(s => e.Message.ToUpper() == s.ToUpper().Trim()))
            {
                _followLeaderName = Setting.Instance.FollowLeader;
                Lua.DoString(string.Format("RunMacroText(\"/follow {0} \")", _followLeaderName));
                //WaitingHere = true;
                //Log.Info("Not following leader. You *MUST* type the follow command to re-enable this");
            }

            // follow me
            string [] followCommands = Setting.Instance.ChatFollowCommand.Split('|');
            foreach (string s in followCommands.Where(s => e.Message.ToUpper() == s.ToUpper().Trim()))
            {
                WaitingHere = false;
                Setting.Instance.FollowLeader = _followLeaderName;
                Log.Info("Following leader");
                validCommand = true;
            }
            
            // turn off looting
            string[] nolootCommands = Setting.Instance.ChatNoLootCommand.Split('|');
            foreach (string s in nolootCommands.Where(s => e.Message.ToUpper() == s.ToUpper().Trim()))
            {
                Setting.Instance.Loot = "Never";
                Log.Info("Looting disabled");
                validCommand = true;
            }

            // turn on looting
            string[] lootCommands = Setting.Instance.ChatLootCommand.Split('|');
            foreach (string s in lootCommands.Where(s => e.Message.ToUpper() == s.ToUpper().Trim()))
            {
                Setting.Instance.Loot = "Always";
                Log.Info("Looting enabled");
                validCommand = true;
            }

            //todo add combat follow enable and disable

            // force assist me
            string[] assistCommands = Setting.Instance.ChatAssistCommand.Split('|');
            foreach (string s in assistCommands.Where(s => e.Message.ToUpper() == s.ToUpper().Trim()))
            {
                if (Leader != null && Leader.GotTarget)
                {
                    Leader.CurrentTarget.Target();
                    Log.Info("Assisting leader and targeting " + Leader.CurrentTarget.Name);
                }
            }

            // combat follow
            string[] combatFollowCommands = Setting.Instance.ChatCombatFollowCommand.Split('|');
            foreach (string s in combatFollowCommands.Where(s => e.Message.ToUpper() == s.ToUpper().Trim()))
            {
                Setting.Instance.CombatFollow = "Always";
                Log.Info("Combat follow enabled");
                validCommand = true;
            }

            // no combat follow
            string[] nocombatFollowCommands = Setting.Instance.ChatNoCombatFollowCommand.Split('|');
            foreach (string s in nocombatFollowCommands.Where(s => e.Message.ToUpper() == s.ToUpper().Trim()))
            {
                Setting.Instance.CombatFollow = "Never";
                Log.Info("Combat follow disabled");
                validCommand = true;
            }

            // no combat follow
            string[] movehereCommands = Setting.Instance.ChatMoveHereCommand.Split('|');
            foreach (string s in movehereCommands.Where(s => e.Message.ToUpper() == s.ToUpper().Trim()))
            {
                if (Leader != null)
                {
                    Log.Info("Moving to leaders position");
                    Navigator.MoveTo(Leader.Location);

                    while (Me.IsMoving)
                    {
                        Thread.Sleep(500);
                        ObjectManager.Update();
                    }
                }
            }


            // interact with this
            string[] clickThisCommands = Setting.Instance.ChatClickThisCommand.Split('|');
            foreach (string s in clickThisCommands.Where(s => e.Message.ToUpper().Contains(s.ToUpper().Trim())))
            {
                //Log.InitLog("--- step 2" + s);
                if (Leader != null)
                {
                    string partialName = e.Message.Replace(s, "").Trim().ToUpper();

                    WoWObject gameObject = ObjectManager.GetObjectsOfTypeFast<WoWObject>().Where(o => o.Name.ToUpper().Contains(partialName)).OrderBy(o => o.Location.Distance(Leader.Location)).FirstOrDefault();

                    if (gameObject != null)
                    {
                        // yeah yeah yeah, we're using thread.sleep. I know its REALLY bad
                        Log.DebugLog("Moving to interact with " + gameObject.Name);

                        if (!gameObject.WithinInteractRange)
                        {
                            Navigator.MoveTo(WoWMovement.CalculatePointFrom(gameObject.Location, (float) 0.70*gameObject.InteractRange));
                            //Navigator.MoveTo(gameObject.Location);
                            Thread.Sleep(500);
                            ObjectManager.Update();
                        }

                        while (Me.IsMoving)
                        {
                            Thread.Sleep(250);
                            ObjectManager.Update();
                            if (!gameObject.WithinInteractRange) continue;

                            StopMoving.Now();
                            break;
                        }

                        if (Me.IsMoving)
                        {
                            StopMoving.Now();
                            Thread.Sleep(1000);
                        }
                        Log.Info("Interacting with " + gameObject.Name);
                        gameObject.Interact();
                    }
                    else
                    {
                        Log.DebugLog("We're inside the Click This command but no object was found that matches the description");
                    }
                }
            }


            // use inventory item
            string[] inventoryItemCommands = Setting.Instance.ChatInventoryItemCommand.Split('|');
            foreach (string s in inventoryItemCommands.Where(s => e.Message.ToUpper().Contains(s.ToUpper().Trim())))
            {
                string partialName = e.Message.Replace(s, "").Trim().ToUpper();

                Log.DebugLog("Use item " + partialName);
                
                foreach (WoWItem item in Me.Inventory.Items)
                {
                    try
                    {
                        //Log.Info("... found " + item.Name);
                        if (item.Name.ToUpper().Contains(partialName))
                        {
                            Log.Info(string.Format("Attempting to use inventory item '{0}'", item.Name));
                            item.Use();
                            break;
                        }
                        {
                            Log.Info(string.Format("No inventory item found matching '{0}'", partialName));
                        }
                    }
                    catch (Exception exception) { Log.DebugLog("Exception finding and using inventory item: " + exception.Message); }
                }

                /*
                if (item != null)
                {
                    Log.Info("Using inventory item " + item.Name);
                    item.Interact();
                }
                 */
                
            }

            // follow distance
            string[] followDistanceCommands = Setting.Instance.ChatFollowDistanceCommand.Split('|');
            foreach (string s in followDistanceCommands.Where(s => e.Message.ToUpper() == s.ToUpper().Trim()))
            {
                int followDistance = ResultNumber(e.Message);

                if (followDistance <= 0)
                {
                    Log.Info("A follow distance of 1 or more must be used");
                    return;
                }

                Setting.Instance.FollowDistance = followDistance;
                Log.Info("Following distance set to " + Setting.Instance.FollowDistance);
                validCommand = true;
            }


            if (Me.Class == WoWClass.Hunter)
            {
                if (e.Message == "aop" || e.Message == "pack")
                {
                    Log.Info("Casting Aspect of the Pack");
                    SpellManager.Cast("Aspect of the Pack");
                }

                if (e.Message == "aoh" || e.Message == "aoth" || e.Message == "hawk")
                {
                    Log.Info("Casting Aspect of the Hawk");
                    SpellManager.Cast("Aspect of the Hawk");
                }
            }

            if (validCommand) Setting.Instance.Save();
        }
        #endregion

        #region Hotkeys


        private static Keys KeyBinding(string setting)
        {
            if (setting.Contains("None")) return Keys.None;

            if (Enum.IsDefined(typeof (Keys), setting))
            {
                return (Keys) Enum.Parse(typeof (Keys), setting);
            }

            // If all else fails just reutrn None
            return Keys.None;
        }

         private static ModifierKeys KeyBindingModifier(string setting)
        {
            if (setting.Contains("None") || setting.Contains("NoRepeat")) return ModifierKeys.NoRepeat;

            if (Enum.IsDefined(typeof (ModifierKeys), setting))
            {
                return (ModifierKeys) Enum.Parse(typeof (ModifierKeys), setting);
            }


            // If all else fails just reutrn None
            return ModifierKeys.NoRepeat;
        }

        protected void UnregisterHotkeys()
        {
            Log.InitLog("Unregistering all hotkeys");
            HotkeysManager.Unregister("FCB Movement Toggle");
            HotkeysManager.Unregister("FCB Pause Toggle");
            HotkeysManager.Unregister("FCB Looting Toggle");
            HotkeysManager.Unregister("FCB Assist Me");
        }

        protected void RegisterHotkeys()
        {
            Log.InitLog("Registering hotkeys");
            Log.InitLog(" ");
            Log.InitLog("Movement toggle, Key:" + Setting.Instance.KeyMoveToTarget + " Mod: " + Setting.Instance.KeyMoveToTargetMod);
            Log.InitLog("FPC Combat Bot toggle, Key:" + Setting.Instance.KeyPauseBotbase + " Mod: " + Setting.Instance.KeyPauseBotbaseMod);
            Log.InitLog("FPC Looting toggle, Key:" + Setting.Instance.KeyLooting + " Mod: " + Setting.Instance.KeyLootingMod);
            Log.InitLog("FPC Assist Me, Key:" + Setting.Instance.KeyAssist + " Mod: " + Setting.Instance.KeyAssistMod);
            
            HotkeysManager.Register("FCB Movement Toggle", KeyBinding(Setting.Instance.KeyMoveToTarget), KeyBindingModifier(Setting.Instance.KeyMoveToTargetMod), MovementToggle);
            HotkeysManager.Register("FCB Pause Toggle", KeyBinding(Setting.Instance.KeyPauseBotbase), KeyBindingModifier(Setting.Instance.KeyPauseBotbaseMod), PauseBotBase);
            HotkeysManager.Register("FCB Looting Toggle", KeyBinding(Setting.Instance.KeyLooting), KeyBindingModifier(Setting.Instance.KeyLootingMod), LootingToggle);
            HotkeysManager.Register("FCB Assist Me", KeyBinding(Setting.Instance.KeyAssist), KeyBindingModifier(Setting.Instance.KeyAssistMod), ForceAssistMe);
            HotkeysManager.Register("FCB Combat Follow", KeyBinding(Setting.Instance.KeyCombatFollow), KeyBindingModifier(Setting.Instance.KeyCombatFollowMod), CombatFollow);
        }

        private void PauseBotBase(Hotkey obj)
        {
            PauseBotbase = !PauseBotbase;

            Lua.DoString(PauseBotbase
                             ? @"print('FCBot \124cFFE61515 PAUSED!')"
                             : @"print('FCBot \124cFF15E61C ENABLED!')");
        }

        private void CombatFollow(Hotkey obj)
        {
            switch (Setting.Instance.CombatFollow)
            {
                case "Always":
                    Setting.Instance.CombatFollow = "Never";
                    Lua.DoString(@"print('FCB Combat Following \124cFFE61515 DISABLED!')");
                    break;

                case "Never":
                    Setting.Instance.CombatFollow = "Always";
                    Lua.DoString(@"print('FCB Combat Following \124cFF15E61C ENABLED!')");
                    break;
            }
        }

        private void LootingToggle(Hotkey obj)
        {
            switch (Setting.Instance.Loot)
            {
                case "Always":
                    Setting.Instance.Loot = "Never";
                    Lua.DoString(@"print('FCBot Looting \124cFFE61515 DISABLED!')");
                    break;

                case "Never":
                    Setting.Instance.Loot = "Always";
                    Lua.DoString(@"print('FCBot Looting \124cFF15E61C ENABLED!')");
                    break;
            }
        }

        private void ForceAssistMe(Hotkey obj)
        {
            if (Leader != null && Leader.GotTarget)
            {
                Leader.CurrentTarget.Target();
                Log.Info("Assisting leader and targeting " + Leader.CurrentTarget.Name);
            }
        }

        private static void MovementToggle(Hotkey obj)
        {
            switch (Setting.Instance.MoveToTarget)
            {
                case "Always":
                    Setting.Instance.MoveToTarget = "Never";
                    Lua.DoString(@"print('FCBot Movement \124cFFE61515 DISABLED!')");
                    break;

                case "Never":
                    Setting.Instance.MoveToTarget = "Always";
                    Lua.DoString(@"print('FCBot Movement \124cFF15E61C ENABLED!')");
                    break;
            }
        }

        #endregion
    }


}
