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
using Styx.CommonBot.Routines;
using Styx.Pathing;

namespace katzerle
{
    class RarekillerKiller
    {

        public static LocalPlayer Me = StyxWoW.Me;
        private static Stopwatch WaitAfterPullTimer = new Stopwatch();

        #region Landingpoints for Inhouse Pandaria Rares
        public static WoWPoint LandingPoint50817 = new WoWPoint(3805.414, 2307.001, 751.3418); // Ahone the Wanderer
        public static WoWPoint LandingPoint50822 = new WoWPoint(802.5736, 1461.719, 385.2514); // Ai-Ran the Shifting Cloud
        public static WoWPoint LandingPoint50768 = new WoWPoint(-1282.449, 1513.644, 13.70746); // Cournith Waterstrider
        //public static WoWPoint LandingPoint50739 = new WoWPoint(291.5115, 1772.154, 310.9289); // Gar'lok - evt mehrere nötig da mehrere Locations
        public static WoWPoint LandingPoint50836 = new WoWPoint(-939.2353, 3206.821, 73.79557); // IkIk the Nibble
        public static WoWPoint LandingPoint50782 = new WoWPoint(190.8846, -3087.344, 22.34637); // Sarnak
        //public static WoWPoint LandingPoint50816 = new WoWPoint(-1675.159, 1056.908, 21.96591); // Ruun Ghostpaw
        public static WoWPoint LandingPoint50831 = new WoWPoint(2982.228, 1911.914, 642.4153); // Scritch
        public static WoWPoint LandingPoint50832 = new WoWPoint(1800.888, 3203.81, 297.0952); // The Yowler
        public static WoWPoint LandingPoint50769 = new WoWPoint(2365.477, 202.3723, 480.2043); // Zai the Outcast
        public static WoWPoint LandingPoint50331 = new WoWPoint(-1007.527, 1143.013, 16.38882); // Go Kan
        public static WoWPoint LandingPoint51078 = new WoWPoint(1473.872, -2270.498, 154.341); // Ferdinand
        public static WoWPoint LandingPoint50749 = new WoWPoint(1001.682, 2123.806, 301.3187); // Kal'thik
        public static WoWPoint LandingPoint50334 = new WoWPoint(455.7283, 4783.266, 50.25636); // Dak the Breaker
        public static WoWPoint LandingPoint50347 = new WoWPoint(103.4087, 2322.122, 202.9273); //Karr der Verdunkler
        public static WoWPoint LandingPoint50811 = new WoWPoint(609.7339, -808.8943, 257.7697); //Nasra Spothide
        public static WoWPoint LandingPoint50821 = new WoWPoint(574.8134, 4256.733, 219.0302); //Ai-Li Skymirror 
        public static WoWPoint LandingPoint50808 = new WoWPoint(320.6452, -2527.102, 42.96289); //Urobi the Walker
        public static WoWPoint LandingPoint50820 = new WoWPoint(2191.15, 5223.58, 89.64538); //Yul Wildpaw

        #endregion

        #region Save Fighting Locations for Pandaria Rares
        public static WoWPoint SavePoint50768 = new WoWPoint(-1282.449, 1513.644, 13.70746); // Cournith Waterstrider
        public static WoWPoint SavePoint50832 = new WoWPoint(1749.258, 3209.693, 316.2377); // The Yowler
        #endregion

        #region Enemys to kill in Reach
        /// <summary>
        /// returns the WoWUnit to kill
        /// </summary>
        public WoWUnit killableEnemy
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && !o.IsPet && (
                    (Rarekiller.Settings.CATA && (Rarekiller.CataRaresList.ContainsKey(Convert.ToInt32(o.Entry))))
                    || (Rarekiller.Settings.Poseidus && ((o.Entry == 50005)	// Poseidus
                            || (o.Entry == 9999999)))			// Platzhalter
                    || (Rarekiller.Settings.TLPD && ((o.Entry == 32491)	// Timelost Protodrake
                            || (o.Entry == 32630)))			// Vyragosa
                    || (Rarekiller.Settings.WOTLK && (Rarekiller.FrostbittenList.ContainsKey(Convert.ToInt32(o.Entry))))
                    || (Rarekiller.Settings.BC && (Rarekiller.BloodyRareList.ContainsKey(Convert.ToInt32(o.Entry))))
                    || (Rarekiller.Settings.KillList && (Rarekiller.KillMobsList.ContainsKey(Convert.ToInt32(o.Entry)))) //Kill Mobs from List
                    || (Rarekiller.Settings.KillList && o.TaggedByOther && (Rarekiller.TaggedMobsList.ContainsKey(Convert.ToInt32(o.Entry)))) //Kill Tagged Mobs from List
                    || ((o.Level < Rarekiller.Settings.Level) && Rarekiller.Settings.LowRAR && (o.CreatureRank == Styx.WoWUnitClassificationType.Rare)) // every single Rare Mob < Level 61 is hunted	
                    || (Rarekiller.Settings.HUNTbyID && (o.Entry == Convert.ToInt64(Rarekiller.Settings.MobID)))				// Hunt special IDs 
                        // Pandaria Rares
                    || (o.Entry == 50828 && Rarekiller.Settings.Bonobos50828) //
                    || (o.Entry == 50836 && Rarekiller.Settings.IkIk50836) //
                    || (o.Entry == 50840 && Rarekiller.Settings.Nanners50840) //
                    || (o.Entry == 50823 && Rarekiller.Settings.Ferocious50823) //
                    || (o.Entry == 50831 && Rarekiller.Settings.Scritch50831) // 
                    || (o.Entry == 50830 && Rarekiller.Settings.Spriggin50830) // 
                    || (o.Entry == 50832 && Rarekiller.Settings.Yowler50832) // 
                    || (o.Entry == 50750 && Rarekiller.Settings.Aethis50750) //          
                    || (o.Entry == 50768 && Rarekiller.Settings.Cournith50768) // 
                    || (o.Entry == 50772 && Rarekiller.Settings.Eshelon50772) // 
                    || (o.Entry == 50766 && Rarekiller.Settings.Selena50766) // 
                    || (o.Entry == 50769 && Rarekiller.Settings.Zai50769) //                    
                    || (o.Entry == 50780 && Rarekiller.Settings.Sahn50780) // 
                    || (o.Entry == 50776 && Rarekiller.Settings.Nalash50776) // 
                    || (o.Entry == 50739 && Rarekiller.Settings.Garlok50739) // 
                    || (o.Entry == 50749 && Rarekiller.Settings.Kaltik50749) //                     
                    || (o.Entry == 50734 && Rarekiller.Settings.Lithik50734) // 
                    || (o.Entry == 50364 && Rarekiller.Settings.Nallak50364) // 
                    || (o.Entry == 50363 && Rarekiller.Settings.Kraxik50363) // 
                    || (o.Entry == 50733 && Rarekiller.Settings.Skithik50733) //                   
                    || (o.Entry == 50388 && Rarekiller.Settings.Torik50388) //
                    || (o.Entry == 50341 && Rarekiller.Settings.Borginn50341) // 
                    || (o.Entry == 50349 && Rarekiller.Settings.Kang50349) // 
                    || (o.Entry == 50340 && Rarekiller.Settings.Gaarn50340) //                     
                    || (o.Entry == 50347 && Rarekiller.Settings.Karr50347) // 
                    || (o.Entry == 50338 && Rarekiller.Settings.Kornas50338) // 
                    || (o.Entry == 50344 && Rarekiller.Settings.Norlaxx50344) // 
                    || (o.Entry == 50339 && Rarekiller.Settings.Sulikshor50339) //                    
                    || (o.Entry == 50354 && Rarekiller.Settings.Havak50354) // 
                    || (o.Entry == 50351 && Rarekiller.Settings.JonnDar50351) // 
                    || (o.Entry == 50355 && Rarekiller.Settings.Kahtir50355) // 
                    || (o.Entry == 50356 && Rarekiller.Settings.Krol50356) //                    
                    || (o.Entry == 50350 && Rarekiller.Settings.Morgrinn50350) // 
                    || (o.Entry == 50352 && Rarekiller.Settings.Qunas50352) // 
                    || (o.Entry == 50359 && Rarekiller.Settings.Urgolax50359) // 
                    || (o.Entry == 50821 && Rarekiller.Settings.AiLi50821) //                     
                    || (o.Entry == 50817 && Rarekiller.Settings.Ahone50817) // 
                    || (o.Entry == 50822 && Rarekiller.Settings.AiRan50822) // 
                    || (o.Entry == 50816 && Rarekiller.Settings.Ruun50816) // 
                    || (o.Entry == 50811 && Rarekiller.Settings.Nasra50811) //                    
                    || (o.Entry == 50808 && Rarekiller.Settings.Urobi50808) // 
                    || (o.Entry == 50820 && Rarekiller.Settings.Yul50820) // 
                    || (o.Entry == 50787 && Rarekiller.Settings.Arness50787) // 
                    || (o.Entry == 50806 && Rarekiller.Settings.Moldo50806) //                     
                    || (o.Entry == 50789 && Rarekiller.Settings.Nessos50789) // 
                    || (o.Entry == 50805 && Rarekiller.Settings.Omnis50805) // 
                    || (o.Entry == 50783 && Rarekiller.Settings.Salyin50783) // 
                    || (o.Entry == 50782 && Rarekiller.Settings.Sarnak50782) //                    
                    || (o.Entry == 50791 && Rarekiller.Settings.Siltriss50791) // 
                    || (o.Entry == 51059 && Rarekiller.Settings.Blackhoof51059) // 
                    || (o.Entry == 50334 && Rarekiller.Settings.Dak50334) // 
                    || (o.Entry == 51078 && Rarekiller.Settings.Ferdinand51078) //                     
                    || (o.Entry == 50331 && Rarekiller.Settings.GoKan50331) // 
                    || (o.Entry == 50332 && Rarekiller.Settings.Korda50332) // 
                    || (o.Entry == 50333 && Rarekiller.Settings.Lon50333) // 
                    || (o.Entry == 50336 && Rarekiller.Settings.Yorik50336) //
                    )))
                    .OrderBy(o => o.Distance).FirstOrDefault();
            }
        }
        #endregion

        /// <summary>
        /// Function to Find and Kill Mobs
        /// </summary>
        public void findAndKillMob(WoWUnit Enemy)
        {
            bool CastSuccess = false;

            if (!Enemy.IsDead)
            {
                #region don't kill if
                // Don't kill Pandaria Rares with Low Health
                if ((Enemy.Entry == 50828 || Enemy.Entry == 50836 || Enemy.Entry == 50840 || Enemy.Entry == 50823 ||
                Enemy.Entry == 50831 || Enemy.Entry == 50830 || Enemy.Entry == 50832 || Enemy.Entry == 50750 ||
                Enemy.Entry == 50768 || Enemy.Entry == 50772 || Enemy.Entry == 50766 || Enemy.Entry == 50769 ||
                Enemy.Entry == 50780 || Enemy.Entry == 50776 || Enemy.Entry == 50739 || Enemy.Entry == 50749 ||
                Enemy.Entry == 50734 || Enemy.Entry == 50364 || Enemy.Entry == 50363 || Enemy.Entry == 50733 ||
                Enemy.Entry == 50388 || Enemy.Entry == 50341 || Enemy.Entry == 50349 || Enemy.Entry == 50340 ||
                Enemy.Entry == 50347 || Enemy.Entry == 50338 || Enemy.Entry == 50344 || Enemy.Entry == 50339 ||
                Enemy.Entry == 50354 || Enemy.Entry == 50351 || Enemy.Entry == 50355 || Enemy.Entry == 50356 ||
                Enemy.Entry == 50350 || Enemy.Entry == 50352 || Enemy.Entry == 50359 || Enemy.Entry == 50821 ||
                Enemy.Entry == 50817 || Enemy.Entry == 50822 || Enemy.Entry == 50816 || Enemy.Entry == 50811 ||
                Enemy.Entry == 50808 || Enemy.Entry == 50820 || Enemy.Entry == 50787 || Enemy.Entry == 50806 ||
                Enemy.Entry == 50789 || Enemy.Entry == 50805 || Enemy.Entry == 50783 || Enemy.Entry == 50782 ||
                Enemy.Entry == 50791 || Enemy.Entry == 51059 || Enemy.Entry == 50334 || Enemy.Entry == 51078 ||
                Enemy.Entry == 50331 || Enemy.Entry == 50332 || Enemy.Entry == 50333 || Enemy.Entry == 50336)
                    && Me.HealthPercent < 80)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Find a hunted Mob called {0} ID {1}, but don't Kill Pandaria Mob with Health < 80%", Enemy.Name, Enemy.Entry);
                    return;
                }
                if (Rarekiller.Settings.NotKillTameable && Enemy.IsTameable) // ... I want to tame him :)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Find a hunted Mob called {0} ID {1}, but kill tameable", Enemy.Name, Enemy.Entry);
                    return;
                }
                #endregion

                Logging.WriteQuiet(Colors.MediumPurple, "Rarekiller: Find a hunted Mob called {0} ID {1}", Enemy.Name, Enemy.Entry);
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Mob Location: {0} / {1} / {2}", Convert.ToString(Enemy.X), Convert.ToString(Enemy.Y), Convert.ToString(Enemy.Z));
                if (Rarekiller.Settings.LUAoutput)
                    Lua.DoString("print('NPCScan: Find {0} ID {1}')", Enemy.Name, Enemy.Entry);

                #region don't kill if ...
                if (Rarekiller.Settings.PullCounter >= Rarekiller.Settings.MaxPullCounter)
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Find {0}, but I pulled him now {1} Times", Enemy.Name, Rarekiller.Settings.PullCounter);
                    Blacklist.Add(Enemy.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 15 Minutes, MoP Rares will be deactivated");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: I pulled now {0} times, blacklist')", Rarekiller.Settings.PullCounter);
                    Rarekiller.Settings.PullCounter = 0;
                    Rarekiller.Settings.GuidCurrentPull = 0;
                    Rarekiller.Settings.BlacklistCounter++;
                    Rarekiller.Settings.DeactivateMoPRare(Enemy);
                    return;
                }
                // Don't kill Pandaria Rares with Level < 90
                if ((Enemy.Entry == 50828 || Enemy.Entry == 50836 || Enemy.Entry == 50840 || Enemy.Entry == 50823 ||
                Enemy.Entry == 50831 || Enemy.Entry == 50830 || Enemy.Entry == 50832 || Enemy.Entry == 50750 ||
                Enemy.Entry == 50768 || Enemy.Entry == 50772 || Enemy.Entry == 50766 || Enemy.Entry == 50769 ||
                Enemy.Entry == 50780 || Enemy.Entry == 50776 || Enemy.Entry == 50739 || Enemy.Entry == 50749 ||
                Enemy.Entry == 50734 || Enemy.Entry == 50364 || Enemy.Entry == 50363 || Enemy.Entry == 50733 ||
                Enemy.Entry == 50388 || Enemy.Entry == 50341 || Enemy.Entry == 50349 || Enemy.Entry == 50340 ||
                Enemy.Entry == 50347 || Enemy.Entry == 50338 || Enemy.Entry == 50344 || Enemy.Entry == 50339 ||
                Enemy.Entry == 50354 || Enemy.Entry == 50351 || Enemy.Entry == 50355 || Enemy.Entry == 50356 ||
                Enemy.Entry == 50350 || Enemy.Entry == 50352 || Enemy.Entry == 50359 || Enemy.Entry == 50821 ||
                Enemy.Entry == 50817 || Enemy.Entry == 50822 || Enemy.Entry == 50816 || Enemy.Entry == 50811 ||
                Enemy.Entry == 50808 || Enemy.Entry == 50820 || Enemy.Entry == 50787 || Enemy.Entry == 50806 ||
                Enemy.Entry == 50789 || Enemy.Entry == 50805 || Enemy.Entry == 50783 || Enemy.Entry == 50782 ||
                Enemy.Entry == 50791 || Enemy.Entry == 51059 || Enemy.Entry == 50334 || Enemy.Entry == 51078 ||
                Enemy.Entry == 50331 || Enemy.Entry == 50332 || Enemy.Entry == 50333 || Enemy.Entry == 50336)
                    && Me.Level < 90)
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Find {0}, but my level is to Low to kill him", Enemy.Name);
                    Blacklist.Add(Enemy.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: My Level is to low')", Enemy.Name);
                    return;
                }
                    
                if (Enemy.TaggedByOther && !Rarekiller.TaggedMobsList.ContainsKey(Convert.ToInt32(Enemy.Entry)))
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Find {0}, but he's tagged by another Player", Enemy.Name);
                    Blacklist.Add(Enemy.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: NPC is tagged')", Enemy.Name);
                    return;
                }
                //if not known ID of Inhouse Pandaria Rare --> don't kill 
                if (!(Enemy.Entry == 50817 || Enemy.Entry == 50768
                    || Enemy.Entry == 50836 || Enemy.Entry == 50782 || Enemy.Entry == 50331 || Enemy.Entry == 51078
                    || Enemy.Entry == 50822 || Enemy.Entry == 50831 || Enemy.Entry == 50832 || Enemy.Entry == 50769))
                {
                    if (Me.IsFlying && Me.IsOutdoors && Enemy.IsIndoors)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Mob is Indoors and I fly Outdoors, so blacklist him to prevent Problems");
                        Logging.Write(Colors.MediumPurple, "Rarekiller: You have to place me next to the Spawnpoint, if you want me to hunt this Mob.");
                        Blacklist.Add(Enemy.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is Indoors')", Enemy.Name);
                        return;
                    }
                }
				if (Enemy.Level > (Me.Level + 4)) // ... 4 Levels higher them me
				{
                    Logging.Write(Colors.MediumPurple, "Rarekiller: His Level is 5 over mine, better not to kill him.");
                    Blacklist.Add(Enemy.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
					return;
				}
				if (Enemy.IsFriendly) // ... is Friendly
				{
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Find {0}, but he's friendly", Enemy.Name);
                    Blacklist.Add(Enemy.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: NPC is friendly')", Enemy.Name);
					return;
				}
				if ((Enemy.Entry == 32630) && !Rarekiller.Settings.Vyragosa) // ... my Settings say don't kill Vyragosa
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Config says: don't kill Vyragosa.");
                    Blacklist.Add(Enemy.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: don't kill NPC')", Enemy.Name);
					return;
				}
				if ((Enemy.Entry == 50057) && !Rarekiller.Settings.Blazewing) // ... my Settings say don't kill Blazewing
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Config says: don't kill Blazewing.");
                    Blacklist.Add(Enemy.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: don't kill NPC')", Enemy.Name);
					return;
				}
				if ((Enemy.Entry == 596) || (Enemy.Entry == 599) || Me.IsInInstance)
				{
				// ... Instance Mobs, don't run wild in Instances
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Don't run wild because of RareMobs in Instances.");
                    Blacklist.Add(Enemy.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
					return;
				}
				if(Rarekiller.BlacklistMobsList.ContainsKey(Convert.ToInt32(Enemy.Entry))) 
				// ... Mob is Blacklisted in Rarekiller/config/BlacklistedMobs.xml
				{
                    Logging.Write(Colors.MediumPurple, "Rarekiller: {0} is Member of the BlacklistedMobs.xml", Enemy.Name);
                    Blacklist.Add(Enemy.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 15 Minutes.");
					return;
                }
                if (Rarekiller.DontInteract) return;
                #endregion

                if (Rarekiller.Settings.Alert)
                    Rarekiller.Alert();
                    
                #region fly to Helperpoint for some Pandaria Rares
                if (Me.IsFlying && (Enemy.Entry == 50817 || Enemy.Entry == 50768
                    || Enemy.Entry == 50836 || Enemy.Entry == 50782 || Enemy.Entry == 50331 || Enemy.Entry == 51078 
                    || Enemy.Entry == 50822 || Enemy.Entry == 50831 || Enemy.Entry == 50832 || Enemy.Entry == 50769
                    || Enemy.Entry == 50749 || Enemy.Entry == 50334 || Enemy.Entry == 50347 || Enemy.Entry == 50811
                    || Enemy.Entry == 50821 || Enemy.Entry == 50808 || Enemy.Entry == 50820))
                {
                    WoWPoint Helperpoint = Enemy.Location;
                    if (Enemy.Entry == 50817)
                        Helperpoint = LandingPoint50817;
                    if (Enemy.Entry == 50768)
                        Helperpoint = LandingPoint50768;
                    if (Enemy.Entry == 50836)
                        Helperpoint = LandingPoint50836;
                    if (Enemy.Entry == 50782)
                        Helperpoint = LandingPoint50782;
                    if (Enemy.Entry == 50831)
                        Helperpoint = LandingPoint50831;
                    if (Enemy.Entry == 50832)
                        Helperpoint = LandingPoint50832;
                    if (Enemy.Entry == 50769)
                        Helperpoint = LandingPoint50769;
                    if (Enemy.Entry == 50822)
                        Helperpoint = LandingPoint50822;
                    if (Enemy.Entry == 50331)
                        Helperpoint = LandingPoint50331;
                    if (Enemy.Entry == 51078)
                        Helperpoint = LandingPoint51078;
                    if (Enemy.Entry == 50749)
                        Helperpoint = LandingPoint50749;
                    if (Enemy.Entry == 50334)
                        Helperpoint = LandingPoint50334;
                    if (Enemy.Entry == 50347)
                        Helperpoint = LandingPoint50347;
                    if (Enemy.Entry == 50811)
                        Helperpoint = LandingPoint50811;
                    if (Enemy.Entry == 50821)
                        Helperpoint = LandingPoint50821;
                    if (Enemy.Entry == 50808)
                        Helperpoint = LandingPoint50808;
                    if (Enemy.Entry == 50820)
                        Helperpoint = LandingPoint50820;

                    if (!Rarekiller.MoveTo(Helperpoint, Enemy, 5, false)) return;
                    Rarekiller.Dismount();
                }
                #endregion

                #region Check PullRange
                    
                string UsedPullRange = Rarekiller.Settings.Range;

                // Range to 27 because of Inhouse Pandaria Mob
                if (SpellManager.HasSpell(Rarekiller.Spells.RangedPullspell) && (
                    Enemy.Entry == 50836 || Enemy.Entry == 50782 || Enemy.Entry == 50331 || Enemy.Entry == 51078
                    || Enemy.Entry == 50822 || Enemy.Entry == 50831 || Enemy.Entry == 50832 || Enemy.Entry == 50768
                    || Enemy.Entry == 50749 || Enemy.Entry == 50334 || Enemy.Entry == 50347 || Enemy.Entry == 50811
                    || Enemy.Entry == 50821 || Enemy.Entry == 50808 || Enemy.Entry == 50820))
                {
                    UsedPullRange = "27";
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Set Range to 27 because of Inhouse Pandaria Rare");
                }
                // Range to 3 for other Pandaria Mobs
                //else if (o.Entry == 50828 || o.Entry == 50836 || o.Entry == 50840 || o.Entry == 50823 ||
                //o.Entry == 50831 || o.Entry == 50830 || o.Entry == 50832 || o.Entry == 50750 ||
                //o.Entry == 50768 || o.Entry == 50772 || o.Entry == 50766 || o.Entry == 50769 ||
                //o.Entry == 50780 || o.Entry == 50776 || o.Entry == 50739 || o.Entry == 50749 ||
                //o.Entry == 50734 || o.Entry == 50364 || o.Entry == 50363 || o.Entry == 50733 ||
                //o.Entry == 50388 || o.Entry == 50341 || o.Entry == 50349 || o.Entry == 50340 ||
                //o.Entry == 50347 || o.Entry == 50338 || o.Entry == 50344 || o.Entry == 50339 ||
                //o.Entry == 50354 || o.Entry == 50351 || o.Entry == 50355 || o.Entry == 50356 ||
                //o.Entry == 50350 || o.Entry == 50352 || o.Entry == 50359 || o.Entry == 50821 ||
                //o.Entry == 50817 || o.Entry == 50822 || o.Entry == 50816 || o.Entry == 50811 ||
                //o.Entry == 50808 || o.Entry == 50820 || o.Entry == 50787 || o.Entry == 50806 ||
                //o.Entry == 50789 || o.Entry == 50805 || o.Entry == 50783 || o.Entry == 50782 ||
                //o.Entry == 50791 || o.Entry == 51059 || o.Entry == 50334 || o.Entry == 51078 ||
                //o.Entry == 50331 || o.Entry == 50332 || o.Entry == 50333 || o.Entry == 50336)
                //{
                //    UsedPullRange = "3";
                //    Logging.Write(Colors.MediumPurple, "Rarekiller: Set Range to 3 because of other Pandaria Rares");
                //}

                else if (!Rarekiller.Settings.DefaultPull && (Convert.ToInt64(UsedPullRange) > Convert.ToInt64(Rarekiller.Spells.RangeCheck(Rarekiller.Settings.Pull))))
                {
                    UsedPullRange = Rarekiller.Spells.RangeCheck(Rarekiller.Settings.Pull);
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Set Range to {0} because of Low-Ranged Customized Spell", Rarekiller.Spells.RangeCheck(Rarekiller.Settings.Pull));
                }

                else if (Rarekiller.Settings.DefaultPull && (Convert.ToInt64(UsedPullRange) > Convert.ToInt64(Rarekiller.Spells.RangeCheck(Rarekiller.Spells.FastPullspell))))
                {
                    UsedPullRange = Rarekiller.Spells.RangeCheck(Rarekiller.Spells.FastPullspell);
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Set Range to {0} because of Low-Ranged Default Spell", Rarekiller.Spells.RangeCheck(Rarekiller.Spells.FastPullspell));
                }
                #endregion

                #region Move To Mob
                if (Enemy.Entry == 50817 || Enemy.Entry == 50768
                    || Enemy.Entry == 50836 || Enemy.Entry == 50782 || Enemy.Entry == 50331 || Enemy.Entry == 51078
                    || Enemy.Entry == 50822 || Enemy.Entry == 50831 || Enemy.Entry == 50832 || Enemy.Entry == 50769
                    || Enemy.Entry == 50749 || Enemy.Entry == 50334 || Enemy.Entry == 50347 || Enemy.Entry == 50811
                    || Enemy.Entry == 50821 || Enemy.Entry == 50808 || Enemy.Entry == 50820)
                { if (!Rarekiller.MoveTo(Enemy, Convert.ToInt64(UsedPullRange), true)) return; }
                else
                { if (!Rarekiller.MoveTo(Enemy, Convert.ToInt64(UsedPullRange), false)) return; }
                #endregion

                #region Special Behavior Nal'lak
                //if (Me.Combat && o.Entry == 50364) //Nal'lak
                //{
                //    WoWMovement.MoveStop();
                //    Thread.Sleep(100);
                //    if (Me.IsFlying)
                //    {
                //        WoWUnit GroundUnit = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => (!u.IsFlying)).OrderBy(u => u.Distance).FirstOrDefault(); ;
                //        if (!Rarekiller.DescendToLand(GroundUnit)) return;
                //    }
                //    Rarekiller.Dismount();
                //}
                #endregion

                #region Special Behavior Clean up Area
                if (Enemy.Entry == 50749 || Enemy.Entry == 50334 || Enemy.Entry == 50347)
                {
                    List<WoWUnit> AddList = ObjectManager.GetObjectsOfType<WoWUnit>().Where(Add => !Add.IsDead && Add.IsHostile && (Add != Enemy) && Add.Location.Distance(Enemy.Location) < 27 && Add.Location.Distance(Me.Location) < 27).OrderBy(Add => Add.Distance).ToList();
                    foreach (WoWUnit Add in AddList)
                    {
                        if (SpellManager.HasSpell(Rarekiller.Spells.RangedPullspell))
                        {
                            CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Spells.RangedPullspell, Add, true);
                            Logging.Write(Colors.MediumPurple, "Rarekiller: Clean up Area, pull Add: {0}", Add.Name);
                            return;
                        }
                    }
                }
                #endregion

                #region Pull Mob
                Logging.Write(Colors.MediumPurple, "Rarekiller: Pull at Mob Location: {0} / {1} / {2}", Convert.ToString(Enemy.X), Convert.ToString(Enemy.Y), Convert.ToString(Enemy.Z));
                Logging.Write(Colors.MediumPurple, "Rarekiller: ... my Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Target is Flying - {0}", Enemy.IsFlying);

                Enemy.Target();
                if (SpellManager.HasSpell(Rarekiller.Spells.RangedPullspell) &&
                    (Enemy.Entry == 50817 || Enemy.Entry == 50768
                    || Enemy.Entry == 50836 || Enemy.Entry == 50782 || Enemy.Entry == 50331 || Enemy.Entry == 51078
                    || Enemy.Entry == 50822 || Enemy.Entry == 50831 || Enemy.Entry == 50832 || Enemy.Entry == 50769
                    || Enemy.Entry == 50749 || Enemy.Entry == 50334 || Enemy.Entry == 50347 || Enemy.Entry == 50811
                    || Enemy.Entry == 50821 || Enemy.Entry == 50808 || Enemy.Entry == 50820))
                    CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Spells.RangedPullspell, Enemy, true);

                else if (!(Rarekiller.Settings.DefaultPull) && SpellManager.HasSpell(Rarekiller.Settings.Pull))
                    CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Settings.Pull, Enemy, false);
                else if (SpellManager.HasSpell(Rarekiller.Spells.FastPullspell))
                    CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Spells.FastPullspell, Enemy, false);
                else
                    Logging.Write(Colors.MediumPurple, "Rarekiller: I have no valid Pullspell - sorry");

                if (Me.CurrentTarget != Enemy)
                    Enemy.Target();
                #endregion

                #region Quick Slowfall for known flying Mobs
                if (Rarekiller.Settings.UseSlowfall && ((Enemy.Entry == 29753) || (Enemy.Entry == 32491) || (Enemy.Entry == 32630) || (Enemy.Entry == 33687) || (Enemy.Entry == 50364)))
                {
                    Thread.Sleep(500);
                    if(Me.IsFalling)
                        Rarekiller.Slowfall.HelpFalling();
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Use Quick Slowfall: {0} Mob: {1}", Me.IsFalling, Enemy.Name);
                #endregion

                #region Pulltimer and Pullcounter
                WaitAfterPullTimer.Reset();
                WaitAfterPullTimer.Start();
                while (WaitAfterPullTimer.IsRunning && !Rarekiller.ToonInvalidCombat && WaitAfterPullTimer.ElapsedMilliseconds < 2000)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Wait for Combat now {0} ms", WaitAfterPullTimer.ElapsedMilliseconds);
                    Thread.Sleep(100);
                }

                if (CastSuccess && Me.Combat)
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Successfully pulled {0}", Enemy.Name);

                if (Enemy.Entry != 32491 && Enemy.Entry != 50005 && (Rarekiller.Settings.GuidCurrentPull != Enemy.Guid) && Me.Combat)
                {
                    Rarekiller.Settings.PullCounter = 1;
                    Rarekiller.Settings.GuidCurrentPull = Enemy.Guid;
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Pulled {0} now first time", Enemy.Name);
                }
                else if (Me.Combat)
                {
                    Rarekiller.Settings.PullCounter++;
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Pulled {0} now {1} times", Enemy.Name, Rarekiller.Settings.PullCounter);
						
					if ((Enemy.Entry == 50828 || Enemy.Entry == 50836 || Enemy.Entry == 50840 || Enemy.Entry == 50823 ||
					Enemy.Entry == 50831 || Enemy.Entry == 50830 || Enemy.Entry == 50832 || Enemy.Entry == 50750 ||
					Enemy.Entry == 50768 || Enemy.Entry == 50772 || Enemy.Entry == 50766 || Enemy.Entry == 50769 ||
					Enemy.Entry == 50780 || Enemy.Entry == 50776 || Enemy.Entry == 50739 || Enemy.Entry == 50749 ||
					Enemy.Entry == 50734 || Enemy.Entry == 50364 || Enemy.Entry == 50363 || Enemy.Entry == 50733 ||
					Enemy.Entry == 50388 || Enemy.Entry == 50341 || Enemy.Entry == 50349 || Enemy.Entry == 50340 ||
					Enemy.Entry == 50347 || Enemy.Entry == 50338 || Enemy.Entry == 50344 || Enemy.Entry == 50339 ||
					Enemy.Entry == 50354 || Enemy.Entry == 50351 || Enemy.Entry == 50355 || Enemy.Entry == 50356 ||
					Enemy.Entry == 50350 || Enemy.Entry == 50352 || Enemy.Entry == 50359 || Enemy.Entry == 50821 ||
					Enemy.Entry == 50817 || Enemy.Entry == 50822 || Enemy.Entry == 50816 || Enemy.Entry == 50811 ||
					Enemy.Entry == 50808 || Enemy.Entry == 50820 || Enemy.Entry == 50787 || Enemy.Entry == 50806 ||
					Enemy.Entry == 50789 || Enemy.Entry == 50805 || Enemy.Entry == 50783 || Enemy.Entry == 50782 ||
					Enemy.Entry == 50791 || Enemy.Entry == 51059 || Enemy.Entry == 50334 || Enemy.Entry == 51078 ||
					Enemy.Entry == 50331 || Enemy.Entry == 50332 || Enemy.Entry == 50333 || Enemy.Entry == 50336)
						&& Me.Level < 90)
					{
						RoutineManager.Current.Combat();
					}
                }
                #endregion

                #region Move to a Save Fighting Area after Pull for some Pandaria Rares
                if ((Enemy.Entry == 50768 || Enemy.Entry == 50832) && Me.Combat)
                {
                    WoWPoint SaveHelperpoint = Enemy.Location;
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Move to Save Fighting Point", Enemy.Entry);
                       
                    if (Enemy.Entry == 50768)
                        SaveHelperpoint = SavePoint50768;
                        
                    if (Enemy.Entry == 50832)
                        SaveHelperpoint = SavePoint50832;

                    if (Navigator.CanNavigateFully(Me.Location, SaveHelperpoint))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Move to Save Fighting Point");
                        while (Me.Location.Distance(SaveHelperpoint) > 5)
                        {
                            if (Me.IsSwimming)
                                WoWMovement.ClickToMove(SaveHelperpoint);
                            else
                                Navigator.MoveTo(SaveHelperpoint);
                            Thread.Sleep(100);
                            if (Rarekiller.ToonInvalid) return;
                        }
                    }
                    else
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Wasn't able to Move to Save Fighting Point", Enemy.Entry);
                }
                #endregion

                return;					
            }
            else
            {
                if (Enemy.Guid == Rarekiller.Settings.GuidCurrentPull)
                {
                    Rarekiller.Settings.PullCounter = 0;
                    Rarekiller.Settings.GuidCurrentPull = 0;
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Set PullCounter to {0}", Rarekiller.Settings.PullCounter);
                }
                    
                if (Enemy.CanLoot)
                    if(!Rarekiller.Loothelper(Enemy)) return;
                        
                if (!Blacklist.Contains(Enemy.Guid, Rarekiller.Settings.Flags))
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Find {0}, but sadly he's dead", Enemy.Name);
                    Blacklist.Add(Enemy.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
                }
            }
        }
    }
}
