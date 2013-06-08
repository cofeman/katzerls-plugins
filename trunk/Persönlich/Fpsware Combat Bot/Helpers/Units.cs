using System;
using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace FCBot.Helpers
{
    public static class Units
    {
        #region Local declarations
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static WoWUnit CT { get { return StyxWoW.Me.CurrentTarget; } }
        private static WoWUnit Pet { get { return StyxWoW.Me.Pet; } }
        #endregion

        internal static IEnumerable<WoWUnit> AttackableUnits
        {
            get { return ObjectManager.GetObjectsOfType<WoWUnit>(true, false).Where(u => u.Attackable && u.CanSelect && !u.IsFriendly && !u.IsDead && !u.IsNonCombatPet && !u.IsCritter && u.Distance <= 60 && (u.IsTargetingMyRaidMember|| u.IsTargetingMeOrPet || u.IsTargetingMyPartyMember )); }
        }

        public static IEnumerable<WoWUnit> NearbyUnfriendlyMobs(double maxDistanceFromUnit, WoWUnit unit)
        {
            return ObjectManager.GetObjectsOfType<WoWUnit>(false, false).Where(p => IsValidTarget(p) && unit.Location.Distance(p.Location) < maxDistanceFromUnit).ToList();
        }

        internal static IEnumerable<WoWUnit> AttackableMeleeUnits
        {
            get { return AttackableUnits.Where(u => u.IsWithinMeleeRange); }
        }

        public static IEnumerable<WoWPlayer> NearbyUnfriendlyPlayers(double maxDistance)
        {
            return ObjectManager.GetObjectsOfType<WoWPlayer>(false, false).Where(p => IsValidTarget(p) && p.DistanceSqr <= maxDistance * maxDistance).ToList();
        }

        /// <summary>
        /// All ALIVE players within (default) 40 yards with a maximum of (default) 98% HP
        /// </summary>
        /// <param name="minHealth"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        public static IEnumerable<WoWPlayer> NearbyFriendlyPlayers(int minHealth = 98, int maxDistance = 40)
        {
            return ObjectManager.GetObjectsOfType<WoWPlayer>(false, true).Where(p => p.IsAlive && p.HealthPercent <= minHealth && p.Distance <= maxDistance).ToList();
        }


        public static bool HasActiveAura(this WoWUnit unit, string aura)
        {
            return unit.ActiveAuras.ContainsKey(aura);
        }

        public static bool HasAuraWithMechanic(this WoWUnit unit, params WoWSpellMechanic[] mechanics)
        {
            var auras = unit.GetAllAuras();
            return auras.Any(a => mechanics.Contains(a.Spell.Mechanic));
        }

        public static int AuraStackCount(this WoWUnit unit, string aura)
        {
            return !unit.HasAura(aura) ? 0 : (int)unit.Auras[aura].StackCount;
        }


        /// <summary>
        /// Returns TRUE is the target has an enraged spell mechanic present
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static bool IsRooted(this WoWUnit unit)
        {
            return unit.Auras.Any(x => x.Value.Spell.Mechanic == WoWSpellMechanic.Rooted);
        }

        public static bool IsSilenced(this WoWUnit unit)
        {
            return unit.Auras.Any(x => x.Value.Spell.Mechanic == WoWSpellMechanic.Silenced);
        }

        public static bool IsSlowed(this WoWUnit unit)
        {
            return unit.Auras.Any(x => x.Value.Spell.Mechanic == WoWSpellMechanic.Slowed);
        }

        public static bool IsDazed(this WoWUnit unit)
        {
            return unit.Auras.Any(x => x.Value.Spell.Mechanic == WoWSpellMechanic.Dazed);
        }

        public static bool IsEnraged(this WoWUnit unit)
        {
            return unit.Auras.Any(x => x.Value.Spell.Mechanic == WoWSpellMechanic.Enraged);
        }

        public static bool IsFocusValid()
        {
            return StyxWoW.Me.FocusedUnit != null && StyxWoW.Me.FocusedUnit.IsValid && StyxWoW.Me.CurrentTargetGuid != StyxWoW.Me.FocusedUnitGuid;
        }

        /// <summary>
        /// Is ... attackable, selectable, !friendly, !dead, !critter, !vanitypet
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsValidTarget(this WoWUnit p)
        {
            if (!p.CanSelect || !p.Attackable) return false;
            if (p.IsFriendly) return false;
            if (p.IsDead) return false;
            if (p.IsPet && p.OwnedByUnit.IsPlayer || p.OwnedByRoot != null) return false;
            if (p.IsNonCombatPet || p.IsCritter) return false;

            return p.Name.Contains("Dummy") || true;
        }

    }
}
