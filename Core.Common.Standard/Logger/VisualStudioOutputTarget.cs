using System;
using KY.Core.Properties;

namespace KY.Core
{
    public class VisualStudioOutputTarget : LogTarget
    {
        public override void Write(LogEntry entry)
        {
            if (Logger.Console.IsConsoleAvailable)
            {
                return;
            }
            string formattedMessage;
            if (entry.Type == LogType.Error)
            {
                formattedMessage = string.Format(Resources.ConsoleErrorFormat, entry.Timestamp, entry.CustomType, entry.Message);
            }
            else
            {
                formattedMessage = string.Format(Resources.ConsoleTraceFormat, entry.Timestamp, entry.Message);
            }
            Console.WriteLine(formattedMessage);
        }
    }
}