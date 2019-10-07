using System;

namespace KY.Core
{
    public static class LoggerConsoleExtension
    {
        public static void TraceJumpBack(this LoggerExtension extension, string message)
        {
            Logger.Trace(message);
            if (Logger.Console.IsConsoleAvailable)
            {
                Console.CursorTop--;
            }
        }

        public static void WarningJumpBack(this LoggerExtension extension, string message)
        {
            Logger.Warning(message);
            if (Logger.Console.IsConsoleAvailable)
            {
                Console.CursorTop--;
            }
        }
    }
}