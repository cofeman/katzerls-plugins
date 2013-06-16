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
    class RarekillerCollector
    {
		public static LocalPlayer Me = StyxWoW.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();

        //ToDo Landing Points for of Underground Objects
        public static WoWPoint IndoorNPC1LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC2LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC3LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC4LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC5LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC6LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC7LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC8LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC9LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC10LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC11LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC12LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC13LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC14LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC15LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC16LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC17LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint IndoorNPC18LandingPoint = new WoWPoint(-11066.67, -2100.342, 175.2816);

        public void findAndPickupObject()
        {

            bool ForceGround = false;
            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDiagnostic("Rarekiller: Scan for Objects to collect");
// ----------------- Generate a List with all wanted Nests found in Object Manager ---------------------		

			ObjectManager.Update();
            List<WoWGameObject> objList = ObjectManager.GetObjectsOfType<WoWGameObject>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (((o.Entry == 202082) && Rarekiller.Settings.TestRaptorNest)
                || ((o.Entry == 202080) && Rarekiller.Settings.TestRaptorNest)
                || ((o.Entry == 202083) && Rarekiller.Settings.TestRaptorNest)
                || ((o.Entry == 202081) && Rarekiller.Settings.TestRaptorNest)
                || (Rarekiller.AnotherMansTreasureList.ContainsKey(Convert.ToInt32(o.Entry)) && Rarekiller.Settings.AnotherMansTreasure && o.Entry > 200000)
				|| (Rarekiller.CollectObjectsList.ContainsKey(Convert.ToInt32(o.Entry)) && Rarekiller.Settings.ObjectsCollector)
                || ((o.Entry == 206195) && Rarekiller.Settings.TestRaptorNest) //Testcase Thundermar Ale Keg
                )))
                .OrderBy(o => o.Distance).ToList();

            List<WoWUnit> RareList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(r => ((r.CreatureRank == Styx.WoWUnitClassificationType.Rare) && r.Level > 85 && !r.IsDead)).OrderBy(r => r.Distance).ToList();

            foreach (WoWGameObject o in objList)
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: Find A Object to collect {0} ID {1}", o.Name, o.Entry);
				
				
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

				
// ----------------- Underground Object ----------
                //ToDo IDs of Underground NPCs
                //if (!(o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999
                //    || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999
                //    || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999))
                //{
                    if (o.IsIndoors && Me.IsFlying && Me.IsOutdoors && (o.Location.Distance(Me.Location) > 30))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part Collector: Can't reach Object because it is Indoors and I fly Outdoors {0}, Blacklist and Move on", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        return;
                    }
                //}
// ----------------- don't collect if Rare Pandaria Elite Around
                if (RareList != null)
                {
                    foreach (WoWUnit r in RareList)
                    {
                        if (r.Location.Distance(o.Location) < 30)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Collector: Can't reach Object because there's a Rare Elite around, Blacklist and move on", o.Name);
                            Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            return;
                        }
                    }
                }

                if (Me.Combat)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: ... but first I have to finish fighting another Mob.");
                    return;
                }
                if (Me.IsOnTransport)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: ... but I'm on a Transport.");
                    return;
                }
				
		        while (Me.IsCasting)
                {
                    Thread.Sleep(100);
                }

                //ToDo IDs of Underground NPCs
                if (!(o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999
                    || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999
                    || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999 || o.Entry == 99999))
                {
                    WoWPoint Helperpoint = null;
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Found a Underground NPC {0} so dismount and walk", o.Entry);
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC1LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC2LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC3LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC4LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC5LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC6LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC7LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC8LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC9LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC10LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC11LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC12LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC13LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC14LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC15LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC16LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC17LandingPoint;
                    if (o.Entry == 99999)
                        Helperpoint = IndoorNPC18LandingPoint;

                    while (o.Location.Distance(Helperpoint) > 5)
                    {
                        Flightor.MoveTo(Helperpoint);
                        Thread.Sleep(100);
                        if (Rarekiller.inCombat) return;
                    }
                    WoWMovement.MoveStop();
                    Thread.Sleep(1000);
                    //Descend to Land
                    WoWMovement.Move(WoWMovement.MovementDirection.Descend);
                    Thread.Sleep(2000);
                    WoWMovement.MoveStop();
                    //Dismount
                    if (Me.Auras.ContainsKey("Flight Form"))
                        Lua.DoString("CancelShapeshiftForm()");
                    else if (Me.Mounted)
                        Lua.DoString("Dismount()");

                    Thread.Sleep(300);
                    ForceGround = true;
                }



// ----------------- Move to Object Part ---------------------
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Move to Object");
                BlacklistTimer.Reset();
                BlacklistTimer.Start();



				while (o.Location.Distance(Me.Location) > 4)
				{
                    if (ForceGround)
                        Navigator.MoveTo(o.Location);
                    else
                        Flightor.MoveTo(o.Location);
					Thread.Sleep(100);
// ----------------- Security  ---------------------
					if (Rarekiller.inCombat) return;
                    if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part MoveTo: Can't reach Object {0}, Blacklist and Move on", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        BlacklistTimer.Reset();
                        WoWMovement.MoveStop();
                        return;
                    }
				}
                BlacklistTimer.Reset();
				Thread.Sleep(300);
				WoWMovement.MoveStop();	
// Collect Nest
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: Nest Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: My Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
                if (Me.Auras.ContainsKey("Flight Form"))
                    Lua.DoString("CancelShapeshiftForm()");
                else if (Me.Mounted)
                    Lua.DoString("Dismount()");

                //Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Take a Screen");
                //Lua.DoString("TakeScreenshot()"); 
				Thread.Sleep(1000);
                o.Interact();
                o.Interact();
                o.Interact();
				Thread.Sleep(2000);
				Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
				Thread.Sleep(4000);
                Logging.Write(Colors.MediumPurple, "Rarekiller Part Collector: Interact with {0} - ID {1}", o.Name, o.Entry);
                ForceGround = false;
            }
        }
    }
}
