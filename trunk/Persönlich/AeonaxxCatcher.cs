using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using System.Globalization;
using System.Windows.Media;
using System.Media;

using Styx;
using Styx.Common;
using Styx.Pathing;
using Styx.Plugins;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace AeonaxxCatcher
{
    public class AeonaxxCatcher : HBPlugin
    {

		public static bool Alert = true;
        public override void Pulse()
        {
            if (StyxWoW.IsInGame)
            {
                if (StyxWoW.Me.IsAlive)
                {
                    _searchforAeonaxx();
					
                }
            }
        }

        public void _searchforAeonaxx()
        {
            List<WoWUnit> mobList = ObjectManager.GetObjectsOfType<WoWUnit>();

            uint Aeonaxx = 50062;
			// uint Aeonaxx = 64480; only for Test
            uint AeonaxxMounted = 51236;
            uint YoungStoneDrake = 44038;

            var aeonaxx = mobList.Find(x => x.Entry == Aeonaxx);
            var youngStoneDrake = mobList.Find(x => x.Entry == YoungStoneDrake);
            var aeonaxxCombat = mobList.Find(x => x.Entry == AeonaxxMounted);

            foreach (WoWUnit mob in mobList)
            {
                if (aeonaxx == null && aeonaxxCombat == null && youngStoneDrake == null)
                {
					Alert = true;
					return;
                }
                else if (aeonaxx != null || aeonaxxCombat != null || youngStoneDrake != null)
                {
					_aeonaxx();
                }
            }
        }

        public void _aeonaxx()
        {
            List<WoWUnit> mobList = ObjectManager.GetObjectsOfType<WoWUnit>();
            const uint Aeonaxx = 50062;
			// const uint Aeonaxx = 64480; Only for Test
            const uint AeonaxxMounted = 51236;
            const uint YoungStoneDrake = 44038;

            var aeonaxx = mobList.Find(x => x.Entry == Aeonaxx);
            var aeonaxxCombat = mobList.Find(x => x.Entry == AeonaxxMounted);
            var youngStoneDrake = mobList.Find(x => x.Entry == YoungStoneDrake);

            float myXLocation = StyxWoW.Me.Location.X;
            float myYLocation = StyxWoW.Me.Location.Y;
            float myZLocation = StyxWoW.Me.Location.Z;

            foreach (WoWUnit mob in mobList)
            {
				if (aeonaxx != null && youngStoneDrake == null && !aeonaxx.WithinInteractRange && aeonaxx.IsAlive && !StyxWoW.Me.Combat)
                {
// ----------------- Alert ---------------------
					if (Alert)
					{
						Logging.Write("Aeonaxx Catcher: Make Noise");
						if (File.Exists(Soundfile))
							new SoundPlayer(Soundfile).Play();
						else
							Logging.Write("Aeonaxx Catcher: playing Soundfile failes");
						Alert = false;
					}
					
					float xLocation = aeonaxx.Location.X;
                    float yLocation = aeonaxx.Location.Y;
                    float zLocation = aeonaxx.Location.Z;

                    Flightor.MoveTo(new WoWPoint(xLocation, yLocation, zLocation));
                    Logging.Write("[AeonaxxCatcher] Aeonaxx is valid, we're not in combat and aeonaxx is alive...  moving to Aeonaxx at" + " X: " + aeonaxx.Location.X + " Y: " + aeonaxx.Location.Y + " Z: " + aeonaxx.Location.Z);
                }
                else if (aeonaxx != null && youngStoneDrake == null && aeonaxx.WithinInteractRange && !StyxWoW.Me.Combat) // no check for isAlive because we're also using this as a secondary Looting Method
                {
// ----------------- Alert ---------------------
					if (Alert)
					{
						Logging.Write("Aeonaxx Catcher: Make Noise");
						if (File.Exists(Soundfile))
							new SoundPlayer(Soundfile).Play();
						else
							Logging.Write("Aeonaxx Catcher: playing Soundfile failes");
						Alert = false;
					}					
					aeonaxx.Interact(); // Triple Interact attempt so it hopefully doesn't do the Interact and then stop (bottish and unreliable)
                    aeonaxx.Interact(); // Will also be used as a secondary Looting Method incase first one fails or they didn't enable Loot Mobs
                    aeonaxx.Interact(); // Since we parachute right next to Aeonaxx it shouldn't be an issue
                    Logging.Write("[AeonaxxCatcher] Aeonaxx is valid and within melee range, interacting...");
                }
                else if (aeonaxxCombat != null && youngStoneDrake == null && aeonaxxCombat.IsAlive && StyxWoW.Me.HealthPercent > 50)
                {
                    aeonaxxCombat.Target();
                    Logging.Write("[AeonaxxCatcher][Mounted] attacking Aeonaxx | [DEBUG] Mount Display ID: " + StyxWoW.Me.MountDisplayId);
                }
                else if (aeonaxxCombat != null && youngStoneDrake != null && youngStoneDrake.IsAlive && StyxWoW.Me.HealthPercent < 50)
                {
                    youngStoneDrake.Target();
                    Logging.Write("[AeonaxxCatcher][Mounted] attacking Young Stone Drakes until they are all slayed | [DEBUG] Mount Display ID: " + StyxWoW.Me.MountDisplayId);
                }
            }
        }
		
		static public string Soundfile
        {
            get
            {
                string sPath = Process.GetCurrentProcess().MainModule.FileName;
                sPath = Path.GetDirectoryName(sPath);
                sPath = Path.Combine(sPath, "Plugins\\AeonaxxCatcher\\siren1.wav");
                return sPath;
            }
        }
		
		public override void Initialize()
        {
            Logging.Write("AeonaxxCatcher - Loaded Version " + Version);
        }

        private static LocalPlayer intMe { get { return StyxWoW.Me; } }
        public override string Name { get { return "AeonaxxCatcher"; } }
        public override string Author { get { return "Giwin"; } }
        public override Version Version { get { return new Version(2, 7); } }
        public override bool WantButton { get { return false; } }
    }
}