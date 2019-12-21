using System;
using KY.Core.Properties;

namespace KY.Core
{
    public class MsBuildOutputTarget : LogTarget
    {
        public override void Write(LogEntry entry)
        {
            try
            {
                string formattedMessage;
                if (entry.Type == LogType.Error)
                {
                    if (entry.CustomType != "Error")
                    {
                        formattedMessage = string.Format(Resources.MsBuildErrorFormat, entry.CustomType, entry.Message);
                    }
                    else
                    {
                        formattedMessage = string.Format(Resources.MsBuildErrorShortFormat, entry.Message);
                    }
                }
                else if (entry.Type == LogType.Warning)
                {
                    formattedMessage = string.Format(Resources.MsBuildWarningFormat, entry.Message);
                }
                else
                {
                    formattedMessage = string.Format(Resources.MsBuildTraceFormat, entry.Message);
                }
                Console.WriteLine(formattedMessage);
            }
            catch
            {
                // Ignore all log related errors
            }
        }
    }
}