using KY.Core.Properties;

namespace KY.Core
{
    public class VisualStudioOutputTarget : LogTarget
    {
        public override void Write(LogEntry entry)
        {
            string formatedMessage;
            if (entry.Type == LogType.Error)
            {
                formatedMessage = string.Format(Resources.ConsoleErrorFormat, entry.Timestamp, entry.CustomType, entry.Message);
            }
            else
            {
                formatedMessage = string.Format(Resources.ConsoleTraceFormat, entry.Timestamp, entry.Message);
            }
            System.Diagnostics.Trace.WriteLine(formatedMessage);
        }
    }
}