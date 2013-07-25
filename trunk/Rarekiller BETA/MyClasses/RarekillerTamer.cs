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
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Pathing;


namespace katzerle
{
    class RarekillerTamer
    {
        public static LocalPlayer Me = StyxWoW.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();
        private static Stopwatch Alerttimer = new Stopwatch();
        
        public void findAndTameMob()
        {
            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Scan for Tamer");
            if (Me.Class != WoWClass.Hunter && !Rarekiller.Settings.TestcaseTamer)
                return;
// ----------------- Generate a List with all wanted Rares found in Object Manager ---------------------		
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (
                ((Rarekiller.Settings.TameDefault && Rarekiller.TameableMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                || (Rarekiller.Settings.TameByID && (o.Entry == Convert.ToInt64(Rarekiller.Settings.TameMobID))))
                && !o.IsPet && o.IsTameable)))
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWUnit o in objList)
            {
                if (!o.IsDead)
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Found a new Pet {0} ID {1}", o.Name, o.Entry);
					Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Tamer: Mob Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: Find {0} ID {1}')", o.Name, o.Entry);

                    #region don't tame if
                    // Don't tame the Rare if ...
                    if (Me.Combat)
                    {
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Tamer: ... but I'm in another Combat :( !!!");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: First finish combat')");
                        return;
                    }

                    if (Me.IsFlying && Me.IsOutdoors && o.IsIndoors)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: Mob is Indoors and I fly Outdoors, so blacklist him to prevent Problems");
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: You have to place me next to the Spawnpoint, if you want me to hunt this Mob.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.Write(Colors.MediumPurple, " Part TamerRarekiller: Blacklist Mob for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is Indoors')", o.Name);
                        return;
                    }

                    if (Me.Level < o.Level)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: Mob Level is higher then mine, can't tame the Mob.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: Blacklist Mob for 60 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC Level is to high to tame')", o.Name);
                        return;
                    }
                    if (Me.IsOnTransport)
                    {
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Tamer: ... but I'm on a Transport.");
                        return;
                    }

                    if (Rarekiller.BlacklistMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                    {
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Tamer: {0} is Member of the BlacklistedMobs.xml", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Tamer: Blacklist Mob for 15 Minutes.");
                        return;
                    }
                    #endregion

                    if (Me.IsCasting)
                    {
                        SpellManager.StopCasting();
                        Thread.Sleep(100);
                    }

                    //Dismiss Pet
                    if (Me.Class == WoWClass.Hunter)
                    {
                        SpellManager.Cast("Dismiss Pet");
                        Thread.Sleep(3000);
                    }

                    #region Alert
                    // ----------------- Alert ---------------------
                    if (Rarekiller.Settings.Alert)
                    {
                        if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                            new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
                        else if (File.Exists(Rarekiller.Soundfile))
                            new SoundPlayer(Rarekiller.Soundfile).Play();
                        else
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Tamer: playing Soundfile failes");
                    }
                    #endregion

                    #region Move to tameable Mob
                    // ----------------- Move to Mob Part ---------------------	

                    WoWPoint newPoint = WoWMovement.CalculatePointFrom(o.Location, (float)Rarekiller.Settings.Tamedistance);

                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Move to target");
                    BlacklistTimer.Reset();
                    BlacklistTimer.Start();
                    while (newPoint.Distance(Me.Location) > Rarekiller.Settings.Tamedistance)
                    {
                        if (o.Entry == 49822 || o.IsIndoors)
                            Navigator.MoveTo(newPoint);
                        else
                            Flightor.MoveTo(newPoint);
                        Thread.Sleep(100);
                        // ----------------- Security  ---------------------
                        if (Rarekiller.inCombat) return;
                        if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: Can't reach Mob {0}, Blacklist and Move on", o.Name);
                            Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            BlacklistTimer.Reset();
                            WoWMovement.MoveStop();
                            return;
                        }
                    }
                    BlacklistTimer.Reset();
                    Thread.Sleep(300);
                    WoWMovement.MoveStop();

                    if (Me.IsFlying)
                    {
                        //Descend to Land
                        WoWMovement.Move(WoWMovement.MovementDirection.Descend);
                        Thread.Sleep(1000);
                        if (Me.IsFlying && !Rarekiller.inCombat)
                            Thread.Sleep(1000);
                        WoWMovement.MoveStop();

                    }
                    //Dismount
                    if (Me.Auras.ContainsKey("Flight Form"))
                        Lua.DoString("CancelShapeshiftForm()");
                    else if (Me.Mounted)
                        Lua.DoString("Dismount()");
                    #endregion

                    Thread.Sleep(150);
                    o.Target();
                    if (Me.Class == WoWClass.Hunter)
                    {
                        #region Tame
                        // Tame it
                        while (!o.IsPet)
                        {
                            if (o.IsDead)
                            {
                                Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: Mob was dying, I'm so Sorry !!! ");
                                return;
                            }
                            if (Me.HealthPercent < 10)
                            {
                                Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: Health < 10% , Use Feign Death !!! ");
                                SpellManager.Cast("Feign Death");
                                return;
                            }

                            if (!Me.IsCasting)
                            {
                                WoWMovement.MoveStop();
                                SpellManager.Cast("Tame Beast");
                                Logging.Write(Colors.MediumPurple, "Plugin Part The Tamer: Try to tame Beast {0}", o.Name);
                                Thread.Sleep(1500);
                            }
                        }
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: Sucessfully tamed Beast {0}", o.Name);
                        #endregion
                    }
                     
                   else
                    {
                        #region Testcase for my Shammi
                        while (!o.IsDead)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer Test: Try to tame Beast", o.Name);
                            Thread.Sleep(1500);
                        }
                        #endregion
                    }
                    
                }
                else if (o.IsPet)
                    return;
                else if (Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags))
                    return;
                else
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: Find a Mob, but sadly he's dead or not tameable: {0}", o.Name);
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: Blacklist Mob for 5 Minutes.");
                    return;
                }
            }
        }

        public void findandfollowFootsteps()
        {
            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Scan for Tamer");
            if (Me.Class != WoWClass.Hunter && !Rarekiller.Settings.TestcaseTamer)
                return;

            #region create a List with Objects in Reach
            // ----------------- Generate a List with all wanted Nests found in Object Manager ---------------------		
            ObjectManager.Update();
            WoWGameObject Footprint = ObjectManager.GetObjectsOfType<WoWGameObject>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (
                    (o.Entry == 214430) //patrannache footprint
                    || (o.Entry == 214429) //rockhide the immovable footprint
                    || (o.Entry == 214431) //bloodtooth footprint
                    || (o.Entry == 214432) //hexapos footprint
                    || (o.Entry == 214433) //stompy footprint
                    || (o.Entry == 214434) //bristlespine footprint
                    || (o.Entry == 214435) //portent footprint
                    || (o.Entry == 214436) //savage footprint
                    || (o.Entry == 214437) //glimmer footprint
                    || ((o.Entry == 210565) && Rarekiller.Settings.TestcaseTamer) //Testcase for Developer
                )))
                .OrderBy(o => o.Distance).FirstOrDefault();
            #endregion

            List<WoWUnit> RareList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(r => ((r.CreatureRank == Styx.WoWUnitClassificationType.Rare) && r.Level > 85 && !r.IsDead)).OrderBy(r => r.Distance).ToList();

            Logging.Write(Colors.MediumPurple, "Rarekiller: Found a Footprint {0} ID {1}", Footprint.Name, Footprint.Entry);
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Tamer: Mob Location: {0} / {1} / {2}", Convert.ToString(Footprint.X), Convert.ToString(Footprint.Y), Convert.ToString(Footprint.Z));
            if (Rarekiller.Settings.LUAoutput)
                Lua.DoString("print('NPCScan: Find {0} ID {1}')", Footprint.Name, Footprint.Entry);

            #region Alert
            // ----------------- Alert ---------------------
            if (Rarekiller.Settings.Alert)
            {
                if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                    new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
                else if (File.Exists(Rarekiller.Soundfile))
                    new SoundPlayer(Rarekiller.Soundfile).Play();
                else
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: playing Soundfile failes");
            }
            #endregion

            #region don't follow, if ...
            // ----------------- don't collect if Rare Pandaria Elite Around
            if (RareList != null)
            {
                foreach (WoWUnit r in RareList)
                {
                    if (r.Location.Distance(Footprint.Location) < 30)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part Collector: Can't reach Object because there's a Rare Elite around, Blacklist and move on", Footprint.Name);
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: Object Elite Rare around')", Footprint.Name);
                        Blacklist.Add(Footprint.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        return;
                    }
                }
            }

            if (Me.Combat)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: ... but first I have to finish fighting another Mob.");
                if (Rarekiller.Settings.LUAoutput)
                    Lua.DoString("print('NPCScan: First finish combat')");
                return;
            }
            if (Me.IsOnTransport)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: ... but I'm on a Transport.");
                if (Rarekiller.Settings.LUAoutput)
                    Lua.DoString("print('NPCScan: I'm on Transport')");
                return;
            }
            #endregion

            while (Me.IsCasting)
            {
                Thread.Sleep(100);
            }

            while (Footprint != null)
            {
                if (Rarekiller.inCombat) return;
                
                #region Move to Object
                // ----------------- Move to Object Part ---------------------
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Move to Object");
                BlacklistTimer.Reset();
                BlacklistTimer.Start();

                while (Footprint.Location.Distance(Me.Location) > 10)
                {
                    Flightor.MoveTo(Footprint.Location);
                    Thread.Sleep(100);
                    // ----------------- Security  ---------------------
                    if (Rarekiller.inCombat) return;
                    if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part MoveTo: Can't reach Object {0}, Blacklist and Move on", Footprint.Name);
                        Blacklist.Add(Footprint.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        BlacklistTimer.Reset();
                        WoWMovement.MoveStop();
                        Rarekiller.Settings.Forceground = false;
                        return;
                    }
                }
                BlacklistTimer.Reset();
                Thread.Sleep(300);
                WoWMovement.MoveStop();
                #endregion
                    
                #region Alert
                if (!Alerttimer.IsRunning)
                {
                    Alerttimer.Reset();
                    Alerttimer.Start();
                }
                // ----------------- Alert ---------------------
                if (Rarekiller.Settings.Alert && Alerttimer.Elapsed.TotalSeconds > 20)
                {
                    if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                        new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
                    else if (File.Exists(Rarekiller.Soundfile))
                        new SoundPlayer(Rarekiller.Soundfile).Play();
                    else
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: playing Soundfile failes");
                    Alerttimer.Reset();
                    Alerttimer.Start();
                }
                #endregion

                #region create a List with Objects in Reach
                // ----------------- Generate a List with all wanted Nests found in Object Manager ---------------------		
                ObjectManager.Update();
                Footprint = ObjectManager.GetObjectsOfType<WoWGameObject>()
                    .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (
                        (o.Entry == 214430) //patrannache footprint
                        || (o.Entry == 214429) //rockhide the immovable footprint
                        || (o.Entry == 214431) //bloodtooth footprint
                        || (o.Entry == 214432) //hexapos footprint
                        || (o.Entry == 214433) //stompy footprint
                        || (o.Entry == 214434) //bristlespine footprint
                        || (o.Entry == 214435) //portent footprint
                        || (o.Entry == 214436) //savage footprint
                        || (o.Entry == 214437) //glimmer footprint
                        || ((o.Entry == 210565) && Rarekiller.Settings.TestcaseTamer) //Testcase for Developer
                    )))
                    .OrderBy(o => o.Distance).FirstOrDefault();
                #endregion 
            }
        }
    }
}
