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
using Styx.CommonBot.Routines;
using Styx.Pathing;

namespace katzerle
{
    class RarekillerKiller
    {

        public static LocalPlayer Me = StyxWoW.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();
        public void findAndKillMob()
        {
            bool CastSuccess = false;
			int loothelper = 0;            

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Scan for Rare Mob");

            #region create List of Mobs in Reach
            // ----------------- Generate a List with all wanted Rares found in Object Manager ---------------------		
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (
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
                || (Rarekiller.Settings.MOP && (
                        (o.Entry == 50828 && Rarekiller.Settings.Bonobos50828) //
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
                        ))
                )))
                .OrderBy(o => o.Distance).ToList();
            #endregion

            foreach (WoWUnit o in objList)
            {
                if (!o.IsDead && !o.IsPet)
                {
                    #region MoP and Low Health

                    // Don't kill Pandaria Rares with Low Health
                    if ((o.Entry == 50828 || o.Entry == 50836 || o.Entry == 50840 || o.Entry == 50823 ||
                    o.Entry == 50831 || o.Entry == 50830 || o.Entry == 50832 || o.Entry == 50750 ||
                    o.Entry == 50768 || o.Entry == 50772 || o.Entry == 50766 || o.Entry == 50769 ||
                    o.Entry == 50780 || o.Entry == 50776 || o.Entry == 50739 || o.Entry == 50749 ||
                    o.Entry == 50734 || o.Entry == 50364 || o.Entry == 50363 || o.Entry == 50733 ||
                    o.Entry == 50388 || o.Entry == 50341 || o.Entry == 50349 || o.Entry == 50340 ||
                    o.Entry == 50347 || o.Entry == 50338 || o.Entry == 50344 || o.Entry == 50339 ||
                    o.Entry == 50354 || o.Entry == 50351 || o.Entry == 50355 || o.Entry == 50356 ||
                    o.Entry == 50350 || o.Entry == 50352 || o.Entry == 50359 || o.Entry == 50821 ||
                    o.Entry == 50817 || o.Entry == 50822 || o.Entry == 50816 || o.Entry == 50811 ||
                    o.Entry == 50808 || o.Entry == 50820 || o.Entry == 50787 || o.Entry == 50806 ||
                    o.Entry == 50789 || o.Entry == 50805 || o.Entry == 50783 || o.Entry == 50782 ||
                    o.Entry == 50791 || o.Entry == 51059 || o.Entry == 50334 || o.Entry == 51078 ||
                    o.Entry == 50331 || o.Entry == 50332 || o.Entry == 50333 || o.Entry == 50336)
                        && Me.HealthPercent < 90)
                    {
                        if (!Me.HasAura("Food") && !Me.IsFlying)
                        {
                            RoutineManager.Current.Rest();
                            Thread.Sleep(500);
                        }
                        return;
                    }
                    #endregion

                    Logging.Write(Colors.MediumPurple, "Rarekiller: Find a hunted Mob called {0} ID {1}", o.Name, o.Entry);
					Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Mob Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: Find {0} ID {1}')", o.Name, o.Entry);

                    #region don't kill if ...
                    // Don't kill Pandaria Rares with Level < 90
                    if ((o.Entry == 50828 || o.Entry == 50836 || o.Entry == 50840 || o.Entry == 50823 ||
                    o.Entry == 50831 || o.Entry == 50830 || o.Entry == 50832 || o.Entry == 50750 ||
                    o.Entry == 50768 || o.Entry == 50772 || o.Entry == 50766 || o.Entry == 50769 ||
                    o.Entry == 50780 || o.Entry == 50776 || o.Entry == 50739 || o.Entry == 50749 ||
                    o.Entry == 50734 || o.Entry == 50364 || o.Entry == 50363 || o.Entry == 50733 ||
                    o.Entry == 50388 || o.Entry == 50341 || o.Entry == 50349 || o.Entry == 50340 ||
                    o.Entry == 50347 || o.Entry == 50338 || o.Entry == 50344 || o.Entry == 50339 ||
                    o.Entry == 50354 || o.Entry == 50351 || o.Entry == 50355 || o.Entry == 50356 ||
                    o.Entry == 50350 || o.Entry == 50352 || o.Entry == 50359 || o.Entry == 50821 ||
                    o.Entry == 50817 || o.Entry == 50822 || o.Entry == 50816 || o.Entry == 50811 ||
                    o.Entry == 50808 || o.Entry == 50820 || o.Entry == 50787 || o.Entry == 50806 ||
                    o.Entry == 50789 || o.Entry == 50805 || o.Entry == 50783 || o.Entry == 50782 ||
                    o.Entry == 50791 || o.Entry == 51059 || o.Entry == 50334 || o.Entry == 51078 ||
                    o.Entry == 50331 || o.Entry == 50332 || o.Entry == 50333 || o.Entry == 50336)
                        && Me.Level < 90)
                    {
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Find {0}, but my level is to Low to kill him", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: My Level is to low')", o.Name);
                        return;
                    }
                    
                    // Don't kill the Rare if ...
                    if (o.TaggedByOther && !Rarekiller.TaggedMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                    {
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Find {0}, but he's tagged by another Player", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is tagged')", o.Name);
                        return;
                    }


                    if (Rarekiller.Settings.NotKillTameable && o.IsTameable) // ... I want to tame him :)
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Pulse Tamer");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is tameable, don't kill')", o.Name);
                        Rarekiller.Tamer.findAndTameMob();
					}

                    if (Me.IsFlying && Me.IsOutdoors && o.IsIndoors)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Mob is Indoors and I fly Outdoors, so blacklist him to prevent Problems");
                        Logging.Write(Colors.MediumPurple, "Rarekiller: You have to place me next to the Spawnpoint, if you want me to hunt this Mob.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is Indoors')", o.Name);
                        return;
                    }
                    
					if (o.Level > (Me.Level + 4)) // ... 4 Levels higher them me
					{
                        Logging.Write(Colors.MediumPurple, "Rarekiller: His Level is 5 over mine, better not to kill him.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
						return;
					}
					if (o.IsFriendly) // ... is Friendly
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Find {0}, but he's friendly", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is friendly')", o.Name);
						return;
					}
					if ((o.Entry == 32630) && !Rarekiller.Settings.Vyragosa) // ... my Settings say don't kill Vyragosa
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Config says: don't kill Vyragosa.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: don't kill NPC')", o.Name);
						return;
					}
					if ((o.Entry == 50057) && !Rarekiller.Settings.Blazewing) // ... my Settings say don't kill Blazewing
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Config says: don't kill Blazewing.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: don't kill NPC')", o.Name);
						return;
					}
					if ((o.Entry == 596) || (o.Entry == 599) || Me.IsInInstance)
					{
					// ... Instance Mobs, don't run wild in Instances
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Don't run wild because of RareMobs in Instances.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
						return;
					}

					if(Rarekiller.BlacklistMobsList.ContainsKey(Convert.ToInt32(o.Entry))) 
					// ... Mob is Blacklisted in Rarekiller/config/BlacklistedMobs.xml
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: {0} is Member of the BlacklistedMobs.xml", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 15 Minutes.");
						return;
                    }
                    

                    if (Me.Combat) // ... I'm in combat
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: ... but first I have to finish fighting another one.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: First finish combat')");
						return;
					}
					if (Me.IsOnTransport) // ... I'm on transport
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: ... but I'm on a Transport.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: I'm on Transport')");
						return;
                    }
                    #endregion

                    #region Alert
// ----------------- Alert ---------------------
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Make Noise");
					if (Rarekiller.Settings.Alert)
					{
						if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
							new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
						else if (File.Exists(Rarekiller.Soundfile))
							new SoundPlayer(Rarekiller.Soundfile).Play();
						else
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: playing Soundfile failes");
                    }
                    #endregion

                    if (Me.IsCasting)
                    {
                        SpellManager.StopCasting();
                        Thread.Sleep(100);
                    }

                    #region Move to Mob
                    // ----------------- Move to Mob Part ---------------------	

                    if (!(Me.Pet == null) && ((Me.Class == WoWClass.Hunter) || (Me.Class == WoWClass.Warlock)))
                        Lua.DoString(string.Format("RunMacroText(\"/petpassive\")"), 0);

                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Move to target");
                    BlacklistTimer.Reset();
                    BlacklistTimer.Start();
					

                    // Spellrange Test Default Pull Spell
                    if (o.Entry == 50828 || o.Entry == 50836 || o.Entry == 50840 || o.Entry == 50823 ||
                    o.Entry == 50831 || o.Entry == 50830 || o.Entry == 50832 || o.Entry == 50750 || 
                    o.Entry == 50768 || o.Entry == 50772 || o.Entry == 50766 || 
                    o.Entry == 50780 || o.Entry == 50776 || o.Entry == 50739 || o.Entry == 50749 ||
                    o.Entry == 50734 || o.Entry == 50363 || o.Entry == 50733 ||
                    o.Entry == 50388 || o.Entry == 50341 || o.Entry == 50349 || o.Entry == 50340 || 
                    o.Entry == 50347 || o.Entry == 50338 || o.Entry == 50344 || o.Entry == 50339 ||
                    o.Entry == 50354 || o.Entry == 50351 || o.Entry == 50355 || o.Entry == 50356 ||
                    o.Entry == 50350 || o.Entry == 50352 || o.Entry == 50359 || o.Entry == 50821 || 
                    o.Entry == 50817 || o.Entry == 50822 || o.Entry == 50816 || o.Entry == 50811 ||
                    o.Entry == 50808 || o.Entry == 50820 || o.Entry == 50787 || o.Entry == 50806 || 
                    o.Entry == 50789 || o.Entry == 50805 || o.Entry == 50783 || o.Entry == 50782 ||
                    o.Entry == 50791 || o.Entry == 51059 || o.Entry == 50334 || o.Entry == 51078 || 
                    o.Entry == 50331 || o.Entry == 50332 || o.Entry == 50333 || o.Entry == 50336)
                    {
                        Rarekiller.Settings.Range = "5";
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Set Range to 5 because of Pandaria Rare");
                    }
                    
                    if (Rarekiller.Settings.DefaultPull && (Convert.ToInt64(Rarekiller.Settings.Range) > Convert.ToInt64(Rarekiller.Spells.RangeCheck(Rarekiller.Spells.FastPullspell))))
                    {
                        Rarekiller.Settings.Range = Rarekiller.Spells.RangeCheck(Rarekiller.Spells.FastPullspell);
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Set Range to {0} because of Low-Ranged Default Spell", Rarekiller.Spells.RangeCheck(Rarekiller.Spells.FastPullspell));
                    }

                    // Spellrange Test Customized Pull Spell
                    if (!Rarekiller.Settings.DefaultPull && (Convert.ToInt64(Rarekiller.Settings.Range) > Convert.ToInt64(Rarekiller.Spells.RangeCheck(Rarekiller.Settings.Pull))))
                    {
                        Rarekiller.Settings.Range = Rarekiller.Spells.RangeCheck(Rarekiller.Settings.Pull);
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Set Range to {0} because of Low-Ranged Customized Spell", Rarekiller.Spells.RangeCheck(Rarekiller.Settings.Pull));
                    }

                    while ((o.Location.Distance(Me.Location) > Convert.ToInt64(Rarekiller.Settings.Range)) && !o.IsDead)
					{
                        if (o.Entry == 49822 || Me.IsIndoors)
							Navigator.MoveTo(o.Location);
						else
							Flightor.MoveTo(o.Location);
						Thread.Sleep(50);

                        if (o.TaggedByOther && !Rarekiller.TaggedMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Find {0}, but he's tagged by another Player", o.Name);
                            Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                            if (Rarekiller.Settings.LUAoutput)
                                Lua.DoString("print('NPCScan: NPC {0} is tagged')", o.Name);
                            BlacklistTimer.Reset();
                            WoWMovement.MoveStop();
                            return;
                        }

						// ----------------- Security  ---------------------
						if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
						{
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part MoveTo: Can't reach Mob {0}, Blacklist and Move on", o.Name);
                            Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
							BlacklistTimer.Reset();
							WoWMovement.MoveStop();
							return;
						}
					}
					BlacklistTimer.Reset();
                    
                    if (o.IsDead && !o.CanLoot)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Mob was killed by another Player");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
                        return;
                    }

                    o.Target();
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Pull at Mob Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: ... my Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Target is Flying - {0}", o.IsFlying);
                    #endregion

                    #region Pull Mob
                    // ----------------- Pull Part --------------------
					WoWMovement.MoveStop();

                    if (!(Rarekiller.Settings.DefaultPull) && SpellManager.HasSpell(Rarekiller.Settings.Pull))
                        CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Settings.Pull, o, false);
                    else if (SpellManager.HasSpell(Rarekiller.Spells.FastPullspell))
                        CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Spells.FastPullspell, o, false);
                    else
                    {
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: I have no valid Pullspell - set Range to 3 for next try");
                        Rarekiller.Settings.Range = "3";
                        findAndKillMob();
                    }

                    if (CastSuccess)
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Successfully pulled {0}", o.Name);
                    else
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Pull fails - set Range to 3 for next try to initiate Aggro", o.Name);
                        Rarekiller.Settings.Range = "3";
                    }

                    if (!(Me.Pet == null) && ((Me.Class == WoWClass.Hunter) || (Me.Class == WoWClass.Warlock)))
                        Lua.DoString(string.Format("RunMacroText(\"/petdefensive\")"), 0);
                    #endregion

                    Thread.Sleep(100);
					WoWMovement.MoveStop();

                    #region Quick Slowfall for known flying Mobs
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Use Quick Slowfall: {0} Mob: {1}", Me.IsFalling, o.Name);
                    if (Me.IsFalling && Rarekiller.Settings.UseSlowfall && ((o.Entry == 29753) || (o.Entry == 32491) || (o.Entry == 32630) || (o.Entry == 33687)))
					{
						Thread.Sleep(500);
						Rarekiller.Slowfall.HelpFalling();
					}
					if(Me.CurrentTarget != o)
						o.Target();
					o.Face();
                    #endregion

                    return;					
                }
                else if (o.IsDead)
                {
                    if (o.CanLoot)
                    {
                        #region Loothelper
                        // ----------------- Loot Helper for all killed Rare Mobs ---------------------
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Found lootable corpse, move to him");

                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Take Screen and Move to target");
                        Lua.DoString("TakeScreenshot()");
                        Thread.Sleep(300);
                        while (o.Location.Distance(Me.Location) > 5)
						{
                            if (o.Entry == 49822 || o.IsIndoors)
								Navigator.MoveTo(o.Location);
							else
								Flightor.MoveTo(o.Location);
							Thread.Sleep(100);
							if (Rarekiller.inCombat) return;
						}
						WoWMovement.MoveStop();

                        if (Me.Auras.ContainsKey("Flight Form"))
                            Lua.DoString("CancelShapeshiftForm()");
                        else if (Me.Mounted)
                            Lua.DoString("Dismount()");

                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Take another Screen");
                        Lua.DoString("TakeScreenshot()");
                        Thread.Sleep(300); 
                        while (loothelper < 3)
                        {
                            Thread.Sleep(500);
                            WoWMovement.MoveStop();
                            o.Interact();
                            Thread.Sleep(2000);
                            Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
                            Thread.Sleep(4000);
                            if (!o.CanLoot)
                            {
                                Logging.Write(Colors.MediumPurple, "Rarekiller: successfully looted");
                                Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                                Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
                                return;
                            }
                            else
                            {
                                Logging.Write(Colors.MediumPurple, "Rarekiller: Loot failed, try again");
                                loothelper = loothelper + 1;
                            }
                            
                        }
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Loot failed 3 Times");
                        #endregion
                    }
                        
                    if (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Find {0}, but sadly he's dead", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
                    }
                }
            }
        }
    }
}
