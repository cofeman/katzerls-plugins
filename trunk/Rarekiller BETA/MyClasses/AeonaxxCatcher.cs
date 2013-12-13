//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
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
    class RarekillerAeonaxx
    {
        #region Units for Aeonaxx
        /// <summary>
        /// returns the WoWUnit Aeonaxx Friendly ID 50062
        /// </summary>
        public WoWUnit AeonaxxFriendly
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => (u.Entry == 50062)).OrderBy(u => u.Distance).FirstOrDefault();
				// return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => (u.Entry == 42607)).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        /// <summary>
        /// returns the WoWUnit Aeonaxx Hostile ID 51236
        /// </summary>
        public WoWUnit AeonaxxHostile
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => (u.Entry == 51236)).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        /// <summary>
        /// returns the WoWUnit young Stone Drake ID 44038
        /// </summary>
        public WoWUnit youngStoneDrake
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => (u.Entry == 44038)).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }
        #endregion


        public static LocalPlayer Me = StyxWoW.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();

        /// <summary>
        /// Function to Find and Interact with NPCs
        /// </summary>
        public void catchAeonaxx()
        {

            float myXLocation = StyxWoW.Me.Location.X;
            float myYLocation = StyxWoW.Me.Location.Y;
            float myZLocation = StyxWoW.Me.Location.Z;

            if (AeonaxxFriendly != null && youngStoneDrake == null && !AeonaxxFriendly.WithinInteractRange && AeonaxxFriendly.IsAlive && !StyxWoW.Me.Combat)
            {
                float xLocation = AeonaxxFriendly.Location.X;
                float yLocation = AeonaxxFriendly.Location.Y;
                float zLocation = AeonaxxFriendly.Location.Z;

                Flightor.MoveTo(new WoWPoint(xLocation, yLocation, zLocation));
                Logging.Write(Colors.MediumPurple, "Rarekiller: Aeonaxx is valid, we're not in combat and aeonaxx is alive...  moving to Aeonaxx at" + " X: " + AeonaxxFriendly.Location.X + " Y: " + AeonaxxFriendly.Location.Y + " Z: " + AeonaxxFriendly.Location.Z);
            }
            else if (AeonaxxFriendly != null && youngStoneDrake == null && AeonaxxFriendly.WithinInteractRange && !StyxWoW.Me.Combat) // no check for isAlive because we're also using this as a secondary Looting Method
            {
                AeonaxxFriendly.Interact(); // Triple Interact attempt so it hopefully doesn't do the Interact and then stop (bottish and unreliable)
                AeonaxxFriendly.Interact(); // Will also be used as a secondary Looting Method incase first one fails or they didn't enable Loot Mobs
                AeonaxxFriendly.Interact(); // Since we parachute right next to Aeonaxx it shouldn't be an issue
                Logging.Write(Colors.MediumPurple, "Rarekiller: Aeonaxx is valid and within melee range, interacting...");
            }
            else if (AeonaxxHostile != null && youngStoneDrake == null && AeonaxxHostile.IsAlive && StyxWoW.Me.HealthPercent > 50)
            {
                AeonaxxHostile.Target();
                Logging.Write(Colors.MediumPurple, "Rarekiller: [Mounted] attacking Aeonaxx | [DEBUG] Mount Display ID: " + StyxWoW.Me.MountDisplayId);
            }
            else if (AeonaxxHostile != null && youngStoneDrake != null && youngStoneDrake.IsAlive && StyxWoW.Me.HealthPercent < 50)
            {
                youngStoneDrake.Target();
                Logging.Write(Colors.MediumPurple, "Rarekiller: [Mounted] attacking Young Stone Drakes until they are all slayed | [DEBUG] Mount Display ID: " + StyxWoW.Me.MountDisplayId);
            }
        }
    }
}
