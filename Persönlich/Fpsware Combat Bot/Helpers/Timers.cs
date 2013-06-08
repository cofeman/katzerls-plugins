using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace FCBot.Helpers
{
    public static class Timers
    {
        private static Dictionary<string, Stopwatch> _timers = new Dictionary<string, Stopwatch>();

        public static void Add(string timerName)
        {
            if (_timers.ContainsKey(timerName))
            {
                return;
            }

            var stw = new Stopwatch();
            stw.Start();
            _timers.Add(timerName, stw);
        }

        public static bool Expired(string timerName, long maximumMilliseconds)
        {
            if (!TimerExists(timerName))
            {
                return false;
            }
            bool result = _timers[timerName].ElapsedMilliseconds > maximumMilliseconds;

            // If it has expired then reset the timer. Solves some issues further down the track
            if (result) Reset(timerName);
            return result;
        }


        public static void Reset(string timerName)
        {
            if (!TimerExists(timerName))
            {
                Add(timerName);
            }

            _timers[timerName].Reset();
            _timers[timerName].Start();
        }

        public static bool Exists(string timerName)
        {
            return _timers.ContainsKey(timerName);
        }

        /// <summary>
        /// Internal check for the Timers class
        /// </summary>
        /// <param name="timerName"></param>
        /// <returns></returns>
        private static bool TimerExists(string timerName)
        {
            if (!_timers.ContainsKey(timerName))
            {
                //Utils.Log("Timer '" + timerName + "' does not exist in the colleciton. For the sake of the CC this has now been added!");
                Add(timerName);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if the spell has been cast and a minimum of 2000ms has passed
        /// </summary>
        /// <param name="spellName"></param>
        /// <returns></returns>
        public static bool SpellOkToCast(string spellName)
        {
            return SpellOkToCast(spellName, 2000);
        }

        public static bool Invalidate(string spellName)
        {
            if (Exists(spellName))
            {
                _timers.Remove(spellName);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check is the spell has been cast and a minimum of X milliseconds has passed
        /// </summary>
        /// <param name="spellName">Spell Name</param>
        /// <param name="millisecondsExpired">Milliseconds</param>
        /// <returns></returns>
        public static bool SpellOkToCast(string spellName, int millisecondsExpired)
        {
            if (!Exists(spellName))
            {
                Add(spellName);
                return true;
            }
            return Expired(spellName, millisecondsExpired);
        }

        public static string Elappsed(string s)
        {
            return !TimerExists(s) ? "timer does not exist" : _timers[s].ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns TRUE if the timer has expired or TRUE if the timer does not yet exist
        /// If the timer has expired it is reset
        /// </summary>
        /// <param name="timerName"></param>
        /// <param name="millisecondsExpired"></param>
        /// <returns></returns>
        public static bool AutoExpire(string timerName, int millisecondsExpired)
        {
            if (!Exists(timerName))
            {
                Add(timerName);
                return true;
            }

            if (Expired(timerName, millisecondsExpired))
            {
                Reset(timerName);
                return true;
            }

            return false;
        }
    }
}
