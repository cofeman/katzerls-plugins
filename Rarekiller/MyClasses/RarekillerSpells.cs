﻿//=================================================================
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
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Net;
using System.Globalization;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins;
using Styx.Pathing;

namespace katzerle
{
    class RarekillerSpells
    {
        public static LocalPlayer Me = StyxWoW.Me;

		// ------------ Spell Functions
        static public bool CastSafe(string spellName, WoWUnit Unit, bool wait)
        {
            bool SpellSuccess = false;
            if (Me.IsCasting)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Spells: I was already Casting");
                return false;
            }
            if (!SpellManager.HasSpell(spellName))
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Spells: I don't have Spell {0}", spellName);
                return false;
            }

            if (SpellManager.HasSpell(spellName) && !Me.IsCasting)
            {
                if (SpellManager.Spells[spellName].CastTime > 1)
                    WoWMovement.MoveStop();
                Unit.Target();
                Thread.Sleep(100);
                Unit.Face();
                Thread.Sleep(150);

                while (SpellManager.GlobalCooldown)
                {
                    Thread.Sleep(10);
                }

                if (!SpellManager.CanCast(spellName))
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Spells: cannot cast spell '{0}' yet - cd={1}, gcd={2}, casting={3} ",
                        SpellManager.Spells[spellName].Name,
                        SpellManager.Spells[spellName].Cooldown,
                        SpellManager.GlobalCooldown,
                        Me.IsCasting
                        );
                    return false;
                }

                SpellSuccess = SpellManager.Cast(spellName);
				
				Thread.Sleep(200);
				
				//if (StyxWoW.GlobalCooldown || Me.IsCasting)
				//	SpellSuccess = true;
				if (SpellSuccess)
                    Logging.Write(Colors.MediumPurple, "Rarekiller Part Spells: * {0}.", spellName);	
                if (wait)
                {
                    while (SpellManager.GlobalCooldown || Me.IsCasting)
                    {
                        Thread.Sleep(100);
                    }
                }

                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Spells: Spell successfull? {0}.", SpellSuccess);
                return SpellSuccess;
            }

            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Spells: Can't cast {0}.", spellName);
            return false;
        }

// Needs Work to update to MOP
        public string FastPullspell
        {
            get
            {
                if (SpellManager.HasSpell("Shadow Word: Pain"))
                    return "Shadow Word: Pain";
                else if (SpellManager.HasSpell("Fire Blast"))
                    return "Fire Blast";
                else if (SpellManager.HasSpell("Heroic Throw"))
                    return "Heroic Throw";
                else if (SpellManager.HasSpell("Arcane Shot"))
                    return "Arcane Shot";
                else if (SpellManager.HasSpell("Sinister Strike"))
                    return "Sinister Strike";
                else if (SpellManager.HasSpell("Icy Touch"))
                    return "Icy Touch";
                else if (SpellManager.HasSpell("Crusader Strike"))
                    return "Crusader Strike";
                else if (SpellManager.HasSpell("Earth Shock"))
                    return "Earth Shock";
                else if (SpellManager.HasSpell("Corruption"))
                    return "Corruption";
                else if (SpellManager.HasSpell("Moonfire"))
                    return "Moonfire";
                else if (SpellManager.HasSpell("Jab"))
                    return "Jab";
                else
                    return "no Spell";
            }
        }

        public string RangeCheck
        {
            get
            {
                if (SpellManager.HasSpell("Shadow Word: Pain"))
                    return "5";
                else if (SpellManager.HasSpell("Fire Blast"))
                    return "5";
                else if (SpellManager.HasSpell("Heroic Throw"))
                    return "5";
                else if (SpellManager.HasSpell("Arcane Shot"))
                    return "5";
                else if (SpellManager.HasSpell("Sinister Strike"))
                    return "3";
                else if (SpellManager.HasSpell("Icy Touch"))
                    return "5";
                else if (SpellManager.HasSpell("Crusader Strike"))
                    return "3";
                else if (SpellManager.HasSpell("Earth Shock"))
                    return "5";
                else if (SpellManager.HasSpell("Corruption"))
                    return "5";
                else if (SpellManager.HasSpell("Moonfire"))
                    return "5";
                else if (SpellManager.HasSpell("Jab"))
                    return "3";
                else
                    return "no Spell";
            }
        }
		
    }    
}
