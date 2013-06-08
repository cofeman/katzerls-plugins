using System.Windows.Media;
using Styx.Common;
using Styx.CommonBot;
using Styx.TreeSharp;

namespace FCBot.Helpers
{
    static class Log
    {
        private static string _lastCombatmsg;
        private const string MessagePrefix = "[FCB] ";

        public static void InitLog(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Color.FromRgb(73, 227, 220), string.Format("{0}{1}", MessagePrefix, string.Format(message, args)));
        }
        
        public static void FailLog(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.Red, "{0}", string.Format("{0}{1}", MessagePrefix, string.Format(message, args)));
        }

        public static void Info(string message, params object[] args)
        {
            Info(message, false, args);
        }

        public static void Info(string message, bool toStatusMessage = false, params object[] args)
        {
            if (message == null) return;
            if (toStatusMessage) { TreeRoot.StatusText = message; }
            Logging.Write(LogLevel.Normal, Color.FromRgb(255, 224, 0), "{0}{1}", MessagePrefix,string.Format(message, args));
        }

        public static void CombatLog(string message, params object[] args)
        {
            if (message != null && message == _lastCombatmsg) return;
            Logging.Write(LogLevel.Normal, Color.FromRgb(222, 0, 255), "{0}{1}", MessagePrefix,string.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void FocusLog(string message, params object[] args)
        {
            if (message != null && message == _lastCombatmsg) return;
            Logging.Write(LogLevel.Normal, Color.FromRgb(0, 255, 190), "{0}{1}", MessagePrefix,string.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void DebugLog(string message, params object[] args)
        {
            DebugLog(message, false, args);
        }

        public static void DebugLog(string message, bool toStatusMessage, params object[] args)
        {
            if (message != null && message == _lastCombatmsg) return;
            if (toStatusMessage) { TreeRoot.StatusText = message; }
            Logging.Write(LogLevel.Diagnostic, Color.FromRgb(255, 154, 154), "{0}{1}", MessagePrefix,string.Format(message, args));
        }

        
    }

    public class LogMessage : Action
    {
        private const string MessagePrefix = "[FCB] ";

        private readonly string _message;

        public LogMessage(string message)
        {
            _message = message;
        }

        protected override RunStatus Run(object context)
        {
            Logging.Write(Color.FromRgb(73, 227, 220), string.Format("{0}{1}", MessagePrefix, _message));

            if (Parent is Selector) return RunStatus.Failure;

            return RunStatus.Success;
        }
    }
}
