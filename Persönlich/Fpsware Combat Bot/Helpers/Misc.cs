using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Styx;

namespace FCBot.Helpers
{
    static class Misc
    {
        public static bool IsUserControllingMovement
        {
            get
            {

                /*
                if (KeyboardPolling.IsKeyDown(Keys.Delete))
                {
                    Log.Info("======== DELETE KEY PRESSED");
                }
                 */
                // if the current WoW window does not have focus then ignore the [global] keydown overrides
                if (StyxWoW.Memory.Process.Id != KeyboardPolling.ForegroundWindowPID) return false;

                if (KeyboardPolling.IsKeyDown(Keys.W)) return true;
                if (KeyboardPolling.IsKeyDown(Keys.S)) return true;
                if (KeyboardPolling.IsKeyDown(Keys.A)) return true;
                if (KeyboardPolling.IsKeyDown(Keys.D)) return true;
                if (KeyboardPolling.IsKeyDown(Keys.Q)) return true;
                if (KeyboardPolling.IsKeyDown(Keys.E)) return true;

                if (KeyboardPolling.IsKeyDown(Keys.Left)) return true;
                if (KeyboardPolling.IsKeyDown(Keys.Right)) return true;
                if (KeyboardPolling.IsKeyDown(Keys.Up)) return true;
                if (KeyboardPolling.IsKeyDown(Keys.Down)) return true;

                if (KeyboardPolling.IsKeyDown(Keys.LButton) && KeyboardPolling.IsKeyDown(Keys.RButton)) return true;

                return false;
            }
        }
    }

    static class KeyboardPolling
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);
        
        public static bool IsKeyDown(Keys key)
        {
            return (GetAsyncKeyState(key)) != 0;
        }
        
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public static int ForegroundWindowPID
        {
            get
            {
                int processID = 0;
                int threadID = GetWindowThreadProcessId(GetForegroundWindow(), out processID);
                return processID;
            }
        }

    }
}
