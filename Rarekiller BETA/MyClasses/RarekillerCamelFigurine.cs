//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
//    Credits to highvoltz, bloodlove, SMcCloud, Lofi, ZapMan 
//                and all the brave Testers
//
//==================================================================
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Pathing;

namespace katzerle
{
    class RarekillerCamel
    {
        public static LocalPlayer Me = StyxWoW.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();
        public static WoWPoint ProblemCamel1 = new WoWPoint(-8906.634, 312.6967, 349.2024);
        public static WoWPoint ProblemCamel2 = new WoWPoint(-9900.116, 461.3653, 45.62226);
        public static WoWPoint ProblemCamel3 = new WoWPoint(-10697.69, 1045.757, 24.125);
        public static WoWPoint ProblemCamel4 = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint DormusPoint = new WoWPoint(-5726.439, 673.6976, 163.293);
        public static WoWPoint LandingPoint64227 = new WoWPoint(2359.229, 2484.88, 686.5128);

        public void findAndPickupObject()
        {
            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Scan for NPC to Interact");

            #region create a List with NPCs in reach
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (
                    ((o.Entry == 50409) && Rarekiller.Settings.Camel) || ((o.Entry == 50410) && Rarekiller.Settings.Camel) // 50409 might be the porting Camel Figurine
                    || (Rarekiller.AnotherMansTreasureList.ContainsKey(Convert.ToInt32(o.Entry)) && Rarekiller.Settings.AnotherMansTreasure && o.Entry < 200000)
                    || (Rarekiller.InteractNPCList.ContainsKey(Convert.ToInt32(o.Entry)) && Rarekiller.Settings.InteractNPC)
                    || ((o.Entry == 48959) && Rarekiller.Settings.TestFigurineInteract) //Testcase rostiger Amboss - Schnotzz Landing
                )))
                .OrderBy(o => o.Distance).ToList();
            #endregion

            List<WoWUnit> RareList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(r => ((r.CreatureRank == Styx.WoWUnitClassificationType.Rare) && r.Level > 85 && !r.IsDead)).OrderBy(r => r.Distance).ToList();

            foreach (WoWUnit o in objList)
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: Find {0} ID {1}", o.Name, o.Entry);
				Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: NPC Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                if (Rarekiller.Settings.LUAoutput)
                    Lua.DoString("print('NPCScan: Find {0} ID {1}')", o.Name, o.Entry);

                
                if (Me.IsIndoors)
                    Rarekiller.Settings.Forceground = true;

                #region Alert
                if (Rarekiller.Settings.Alert)
                {
                    if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                        new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play(); 
                    else if (File.Exists(Rarekiller.Soundfile))
                        new SoundPlayer(Rarekiller.Soundfile).Play();
                    else
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part NPC: playing Soundfile failes");
                }
                #endregion

                #region don't collect if ...
                // ----------------- Underground ----------
                //if not ID of Underground NPCs of Another Mans Treasure --> don't collect
                if (!(o.Entry == 64227))
                {
                    if (o.IsIndoors && Me.IsFlying && Me.IsOutdoors && (o.Location.Distance(Me.Location) > 30))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part NPC: Can't reach NPC because it is Indoors and I fly Outdoors {0}, Blacklist and Move on", o.Name);
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is Indoors')", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        return;
                    }
                }

// -----------------  don't collect if Rare Pandaria Elite Around
                if (RareList != null)
                {
                    foreach (WoWUnit r in RareList)
                    {
                        if (r.Location.Distance(o.Location) < 30)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part NPC: Can't reach Object because there's a Rare Elite around, Blacklist and move on", o.Name);
                            if (Rarekiller.Settings.LUAoutput)
                                Lua.DoString("print('NPCScan: NPC Elite Rare around')", o.Name);
                            Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            return;
                        }
                    }
                }

                if (Rarekiller.Settings.PlayerScan && RarekillerSecurity.PlayerAround(o))
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part NPC: There are other Players around, so move on");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: Other Players around')");
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    return;
                }
				
                if (Me.Combat)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part NPC: ... but first I have to finish fighting another Mob.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: First finish combat')");
                    return;
                }
                if (Me.IsOnTransport)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part NPC: ... but I'm on a Transport.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: I'm on Transport')");
                    return;
                }
                #endregion

                if (Me.IsCasting)
                {
                    SpellManager.StopCasting();
                    Thread.Sleep(100);
                }

                #region Move to Helperpoint if found known Figurine under a Tent
                if (!Rarekiller.Settings.Forceground && ((o.Location.Distance(ProblemCamel1) < 10) || (o.Location.Distance(ProblemCamel2) < 10) || 
						(o.Location.Distance(ProblemCamel3) < 10) || (o.Location.Distance(ProblemCamel4) < 20)))
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Found a Problem Figurine {0} so dismount and walk", o.Entry);
					while (o.Location.Distance(Me.Location) > 15)
					{
						Flightor.MoveTo(o.Location);
						Thread.Sleep(100);
						if (Rarekiller.inCombat) return;
					}
					WoWMovement.MoveStop();
					Thread.Sleep(500);
					//Descend to Land
					WoWMovement.Move(WoWMovement.MovementDirection.Descend);
					Thread.Sleep(1000);
					WoWMovement.MoveStop();
					//Dismount
                    if (Me.Auras.ContainsKey("Flight Form"))
                        Lua.DoString("CancelShapeshiftForm()");
                    else if (Me.Mounted)
                        Lua.DoString("Dismount()");

					Thread.Sleep(300);
                    Rarekiller.Settings.Forceground = true;
                }
                #endregion

                #region Move to Helperpoint if known underground NPC
                //ToDo IDs of Underground NPCs
                if (!Rarekiller.Settings.Forceground && o.Entry == 64227)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Found a Underground NPC {0} so dismount and walk", o.Entry);

                    while (Me.Location.Distance(LandingPoint64227) > 5)
                    {
                        Flightor.MoveTo(LandingPoint64227);
                        Thread.Sleep(100);
                        if (Rarekiller.inCombat) return;
                    }

                    WoWMovement.MoveStop();
                    //Dismount
                    if (Me.Auras.ContainsKey("Flight Form"))
                        Lua.DoString("CancelShapeshiftForm()");
                    else if (Me.Mounted)
                        Lua.DoString("Dismount()");

                    Thread.Sleep(300);
                    Rarekiller.Settings.Forceground = true;
                }
                #endregion


                Logging.Write(Colors.MediumPurple, "Rarekiller Part MoveTo: Move to target");

                BlacklistTimer.Reset();
                BlacklistTimer.Start();

                #region Move to NPC
                while (o.Location.Distance(Me.Location) > 4)
				{
                    if (Rarekiller.Settings.Forceground)
						Navigator.MoveTo(o.Location);
					else
						Flightor.MoveTo(o.Location);
					Thread.Sleep(100);
					if (Rarekiller.inCombat) return;
					if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
					{
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part NPC: Blacklist NPC for 15 Minutes.");
						BlacklistTimer.Reset();
						WoWMovement.MoveStop();
                        Rarekiller.Settings.Forceground = false;
						return;
					}
				}

                BlacklistTimer.Reset();
                Thread.Sleep(500);
                WoWMovement.MoveStop();

                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part NPC: NPC Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part NPC: My Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
                if (Me.Auras.ContainsKey("Flight Form"))
                    Lua.DoString("CancelShapeshiftForm()");
                else if (Me.Mounted)
                    Lua.DoString("Dismount()");
                #endregion
 
				Thread.Sleep(500);
                o.Interact();
                o.Interact();
                o.Interact();
                Logging.Write(Colors.MediumPurple, "Rarekiller Part NPC: Interact with NPC - ID {0}", o.Entry);
                Thread.Sleep(300);

                Rarekiller.Settings.Forceground = false;
				
                //64143 = Test
				if (o.Entry == 65552 || o.Entry == 64272 || o.Entry == 64004 || o.Entry == 64191 || o.Entry == 64227)
				{
                    Thread.Sleep(500);
                    Lua.DoString("RunMacroText(\"/click GossipTitleButton1\");");
					Thread.Sleep(500);
				}
            }
        }

        public void findAndKillDormus()
        {
            bool CastSuccess = false;
			//Testcases --> 51756 = Blutelfenjunge --> 52008 = Resortangestellter --> 50245 = Dormus
            int IDDormus = 50245;
            //Testcases --> 6346 = Fear Ward --> 974 = Earthsschild --> Dormus' Rage = 93269
            int IDDormusAura = 93269;

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Scan for Dormus");
            
            ObjectManager.Update();
            
            WoWUnit Dormus = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == IDDormus).OrderBy(u => u.Distance).FirstOrDefault();

            #region Move to Dormus Helperpoint
            while (Me.HasAura(IDDormusAura) && Dormus == null && !Me.Combat && Me.Location.Distance(DormusPoint) > 5 && Me.Location.Distance(DormusPoint) < 500)
            {
                WoWMovement.ClickToMove(DormusPoint);
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Dormus not in sight, Swim to Dormus Helper Point");
                Thread.Sleep(500);

                ObjectManager.Update();
                Dormus = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == IDDormus).OrderBy(u => u.Distance).FirstOrDefault();
            }
            #endregion

            if (Dormus != null)
            {
                if (!Dormus.IsDead)
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: Find {0}", Dormus.Name);

                    #region Alert
                    if (Rarekiller.Settings.Alert)
                    {
                        if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                            new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
                        else if (File.Exists(Rarekiller.Soundfile))
                            new SoundPlayer(Rarekiller.Soundfile).Play();
                        else
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: playing Soundfile failes");
                    }
                    #endregion

                    Dormus.Target();
					
					#region Move to Dormus
					// ------------- Move to Dormus with Klick to Move -------------------		

                    while (DormusPoint.Distance(Me.Location) > 5 && Me.HasAura(IDDormusAura) && !Dormus.IsDead && !Rarekiller.Settings.ReachedDormusHelperpoint)
					{
						WoWMovement.ClickToMove(DormusPoint);
						Thread.Sleep(100);
						Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: Move out of Water to Helperpoint");
                        if (Rarekiller.inCombat) return;
					}
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: Reached Dormus Helperpoint");
                    Rarekiller.Settings.ReachedDormusHelperpoint = true;

                    while (Dormus.Location.Distance(Me.Location) > 3 && Me.HasAura(IDDormusAura) && !Dormus.IsDead)
					{
                        if (Navigator.CanNavigateFully(Me.Location, Dormus.Location))
                            Navigator.MoveTo(Dormus.Location);
                        else
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: Navigation Issue, use Click to Move");
                            WoWMovement.ClickToMove(Dormus.Location);
                        }
						Thread.Sleep(100);
						Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: Move to Dormus");
                        if (Rarekiller.inCombat) return;
					}
					WoWMovement.MoveStop();
					#endregion

					#region Pull Dormus
					// ------------- pull Dormus  -------------------					
					Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: Distance: {0}", Dormus.Location.Distance(Me.Location));
					Dormus.Target();
					Dormus.Face();
					Thread.Sleep(100);


					if (!(Rarekiller.Settings.DefaultPull) && SpellManager.HasSpell(Rarekiller.Settings.Pull))
						CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Settings.Pull, Dormus, true);
					else if (SpellManager.HasSpell(Rarekiller.Spells.FastPullspell))
						CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Spells.FastPullspell, Dormus, false);
					else
						Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: I have no Pullspell");
					if (!CastSuccess && SpellManager.HasSpell("Shoot"))
						CastSuccess = RarekillerSpells.CastSafe("Shoot", Dormus, true);
					if (CastSuccess)
					{
						Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: successfully pulled Dormus");
						Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: Pull Distance: {0}", Dormus.Location.Distance(Me.Location));
						return;
					}
					else if (!CastSuccess && Me.Combat)
						Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: got Aggro");
					else
					{
						Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: Pull Fails --> next try");
					}
					#endregion
                }
                else if (!Blacklist.Contains(Dormus.Guid, Rarekiller.Settings.Flags))
                {
                    // ----------------- Loot Helper ---------------------
                    if (Dormus.CanLoot)
                    {
                        #region Loothelper
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: Found lootable {0}, move to him", Dormus.Name);
                        if (Me.Auras.ContainsKey("Flight Form"))
                            Lua.DoString("CancelShapeshiftForm()");
                        else if (Me.Mounted)
                            Lua.DoString("Dismount()");

                        Dormus.Target();
                        // ------------- Move to Corpse -------------------					
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: ... move to him to loot");
                        while (Dormus.Location.Distance(Me.Location) > 3 && Me.HasAura(IDDormusAura))
						{
							Navigator.MoveTo(Dormus.Location);
							Thread.Sleep(100);
						}
						
                        Thread.Sleep(500);
                        WoWMovement.MoveStop();
                        Dormus.Interact();
                        Thread.Sleep(2000);
                        Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
                        Thread.Sleep(4000);
                        if (!Dormus.CanLoot)
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: successfully looted");
                        else
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: Loot failed, try again");
                        #endregion
                    }
                    else
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Find {0}, but sadly he's dead", Dormus.Name);
                        Blacklist.Add(Dormus.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
                    }
                }
            }
        }

        public void AvoidSpit(WoWUnit Unit)
        {
            if (!StyxWoW.Me.IsFacing(Unit))
            { Unit.Face(); Thread.Sleep(300); }
			Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: Avoid Camel Spit");
            //94967 = Aura Spit
            while (Me.HasAura(94967))
                WoWMovement.Move(WoWMovement.MovementDirection.StrafeRight);
            WoWMovement.MoveStop();
            Unit.Face();
			Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: successfully avoided Camel Spit");
        }
    }
}
