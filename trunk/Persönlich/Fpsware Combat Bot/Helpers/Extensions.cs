using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace FCBot.Helpers
{
    public static class Extensions
    {

        #region Local declarations
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static WoWUnit CT { get { return StyxWoW.Me.CurrentTarget; } }
        private static WoWUnit Pet { get { return StyxWoW.Me.Pet; } }
        #endregion

        public static bool HasMyAura(this WoWUnit unit, string aura)
        {
            return HasMyAura(unit, aura, 0);
        }

        public static bool HasMyAura(this WoWUnit unit, string aura, int stacks)
        {
            return HasAura(unit, aura, stacks, StyxWoW.Me);
        }

        private static bool HasAura(this WoWUnit unit, string aura, int stacks, WoWUnit creator)
        {
            return unit.GetAllAuras().Any(a => a.Name == aura && a.StackCount >= stacks && (creator == null || a.CreatorGuid == creator.Guid));
        }

        public static IEnumerable<T> GetAllItems<T>(this Enum value)
        {
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                yield return (T)item;
            }
        }




        private static string Right(string s, int c)
        {
            return s.Substring(c > s.Length ? 0 : s.Length - c);
        }
        public static string UnitID(ulong guid)
        {
            return Right(String.Format("{0:X4}", guid), 4);
        }

        public static string CamelToSpaced(this string str)
        {
            var sb = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsUpper(c))
                {
                    sb.Append(' ');
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static bool HasAura(this WoWUnit unit, string aura, bool isMyAura = false, int msLeft = 1000)
        {
            WoWAura result = unit.GetAuraByName(aura);
            if (result == null) return false;
            if (isMyAura && result.CreatorGuid != StyxWoW.Me.Guid) return false;

            return isMyAura ? unit.GetAllAuras().Any(a => a.Name == aura && a.CreatorGuid == StyxWoW.Me.Guid && a.TimeLeft.TotalMilliseconds >= msLeft) : unit.GetAllAuras().Any(a => a.Name == aura && a.TimeLeft.TotalMilliseconds >= msLeft);
        }

        public static string SafeName(this WoWObject obj)
        {
            if (obj.IsMe) { return "Me"; }
            if (obj.Name.Contains("Dummy")) { return "Training Dummy"; }

            string name;
            WoWPlayer player = obj as WoWPlayer;
            if (player != null)
            {
                if (RaFHelper.Leader == obj) return "Tank";
                name = player.Class.ToString();
            }
            else if (obj is WoWUnit && obj.ToUnit().IsPet) { name = "Pet"; }
            else { name = obj.Name; }

            return name;
        }

        public static bool HasAnyAura(this WoWUnit unit, params string[] auraNames)
        {
            WoWAuraCollection auras = unit.GetAllAuras();
            HashSet<string> hashes = new HashSet<string>(auraNames);
            return auras.Any(a => hashes.Contains(a.Name));
        }

        /// <summary>
        /// get the  combined base spell and hitbox range of <c>unit</c>.
        /// </summary>
        /// <param name="unit">unit</param>
        /// <param name="baseSpellRange"></param>
        /// <param name="other">Me if null, otherwise second unit</param>
        /// <returns></returns>
        public static float SpellRange(this WoWUnit unit, float baseSpellRange, WoWUnit other = null)
        {
            // abort if mob null
            if (unit == null)
                return 0;

            // optional arg implying Me, then make sure not Mob also
            if (other == null)
                other = StyxWoW.Me;
            return baseSpellRange + other.CombatReach + unit.CombatReach;
        }

        public static float MeleeDistance(this WoWUnit unit)
        {
            return Me.MeleeDistance(unit);
        }

        public static float MeleeRange(this WoWUnit unit)
        {
            return unit.MeleeDistance();
        }

        /// <summary>
        /// get melee distance between two units
        /// </summary>
        /// <param name="unit">unit</param>
        /// <param name="other">Me if null, otherwise second unit</param>
        /// <returns></returns>
        public static float MeleeDistance(this WoWUnit unit, WoWUnit other = null)
        {
            // abort if mob null
            if (unit == null)
                return 0;

            if (other == null)
            {
                if (unit.IsMe)
                    return 0;
                other = StyxWoW.Me;
            }

            // pvp, then keep it close
            if (unit.IsPlayer && other.IsPlayer)
                return 3.5f;

            return Math.Max(5f, other.CombatReach + 1.3333334f + unit.CombatReach);
        }

        public static bool IsMeMeleeClass
        {
            get
            {
                switch (Me.Class)
                {
                    case WoWClass.DeathKnight:
                    case WoWClass.Rogue:
                    case WoWClass.Warrior:
                    case WoWClass.Monk:
                        return true;

                    case WoWClass.Druid:
                        if (Me.Specialization == WoWSpec.DruidFeral || Me.Specialization == WoWSpec.DruidGuardian)
                            return true;
                        break;

                    case WoWClass.Paladin:
                        if (Me.Specialization != WoWSpec.PaladinHoly)
                            return true;
                        break;
                    
                    case WoWClass.Shaman:
                        if (Me.Specialization == WoWSpec.ShamanEnhancement)
                            return true;
                        break;
                }

                return false;
            }
        }

     
    }
}
