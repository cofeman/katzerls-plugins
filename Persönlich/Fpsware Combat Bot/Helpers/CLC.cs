using System;
using System.Linq;
using System.Text.RegularExpressions;
using Styx;
using Styx.WoWInternals.WoWObjects;

namespace FCBot.Helpers
{
    static class CLC
    {
        internal static LocalPlayer Me { get { return StyxWoW.Me; } }
        internal static WoWUnit CT { get { return StyxWoW.Me.CurrentTarget; } }
        internal static WoWUnit Pet { get { return StyxWoW.Me.Pet; } }

        /// <summary>
        /// Returns TRUE is the setting is valid when checked by 'CLC logic'
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static bool OK(this string setting)
        {
            if (!string.IsNullOrEmpty(setting)) return setting.ResultOK();
            return false;
        }

        public static bool ResultOK(this string setting)
        {
            if (string.IsNullOrEmpty(setting)) return false;

            string s = setting.ToUpper();

            // Most common settings
            if (s.Contains("ALWAYS") || s.Contains("IMMEDIATE") || s.Contains("ENABLE")) return true;
            if (s.Contains("NEVER") || s.Contains("NONE")) return false;
            if (s.Contains("DISABLED")) return false;

            if (s.Contains("AUTOMATIC")) return true;

            // Not so common settings
            if (s.Contains("DON'T SHOW")) return false;
            if (s.Contains("+ ADDS") || s.Contains("+ HOSTILE")) return AddsOk(setting);
            if (s.Contains("+ PLAYERS")) return AddsOk(setting, true);

            if (s.Contains("BOSS ONLY") && CT != null && CT.IsBoss) return true;
            
            


            return false;
        }

        /// <summary>
        /// Returns TRUE if the number of aggro units (or players) >= the parsed quantity. Eg '3+ Adds' requires 3 or more mobs to be in combat with us, pet, party or raid members
        /// </summary>
        /// <param name="settingName">Setting string to be checked</param>
        /// <param name="playersOnly">Check for PLAYERS only</param>
        /// <returns></returns>
        private static bool AddsOk(string settingName, bool playersOnly = false)
        {
            int minCount = ResultNumber(settingName);

            int countOfAgroUnits = playersOnly
                                       ? Units.AttackableUnits.Count(
                                           u =>
                                           u.IsPlayer &&
                                           (u.IsTargetingMeOrPet || u.IsTargetingMyPartyMember ||
                                            u.IsTargetingMyRaidMember))

                                       : Units.AttackableUnits.Count(
                                           u =>
                                           u.IsTargetingMeOrPet || u.IsTargetingMyPartyMember ||
                                           u.IsTargetingMyRaidMember);
            
            return minCount > 0 && minCount >= countOfAgroUnits;
        }


        /// <summary>
        /// Returns TRUE if the setting [partially] CONTAINS the text
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="partialMatch"></param>
        /// <returns></returns>
        private static bool ResultOK(this String setting, string partialMatch)
        {
            return setting.ToUpper().Contains(partialMatch.ToUpper());
        }

        /// <summary>
        /// Returns the Integer found in the string. Eg '5+ Adds' would return 5 as an int32
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        private static int ResultNumber(this String settingName)
        {
            int result;

            string numFromString = Regex.Match(settingName, @"\d+").Value;
            return Int32.TryParse(numFromString, out result) ? result : 0;
        }
    }
}
