using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace FCBot.Helpers
{
    public static class Groups
    {
        /// <summary>
        /// All ALIVE group members in my Party OR Raid
        /// </summary>
        internal static IEnumerable<WoWPlayer> GroupMembers
        {
            get { return ObjectManager.GetObjectsOfType<WoWPlayer>(true, true).Where(u => u.CanSelect && !u.IsDead && u.IsInMyPartyOrRaid); }
        }

        public static bool MeIsTank
        {
            get
            {
                bool all = Tanks.All(p => !p.IsAlive);
                return (StyxWoW.Me.Role & WoWPartyMember.GroupRole.Tank) != 0 || all && StyxWoW.Me.HasAura("Bear Form");
            }
        }

        public static bool MeIsHealer
        {
            get { return (StyxWoW.Me.Role & WoWPartyMember.GroupRole.Healer) != 0; }
        }

        public static List<WoWPlayer> Tanks
        {
            get
            {
                if (!StyxWoW.Me.GroupInfo.IsInParty) return new List<WoWPlayer>();

                return StyxWoW.Me.GroupInfo.RaidMembers.Where(p => p.HasRole(WoWPartyMember.GroupRole.Tank)).Select(p => p.ToPlayer()).Where(p => p != null).ToList();
            }
        }

        public static List<WoWPlayer> Healers
        {
            get
            {
                if (!StyxWoW.Me.GroupInfo.IsInParty) return new List<WoWPlayer>();

                return StyxWoW.Me.GroupInfo.RaidMembers.Where(p => p.HasRole(WoWPartyMember.GroupRole.Healer)).Select(p => p.ToPlayer()).Where(p => p != null).ToList();
            }
        }
    }
}
