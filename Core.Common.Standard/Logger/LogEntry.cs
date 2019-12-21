using System;
using System.Text;
using KY.Core.Properties;

namespace KY.Core
{
    [Serializable]
    public class LogEntry
    {
        public LogType Type { get; }
        public DateTime Timestamp { get; }
        public string Message { get; }
        public string CustomType { get; }
        public string Source { get; set; }
        public bool Shortable { get; private set; }

        public LogEntry(LogType type, DateTime timestamp, string message, string source = null, bool shortable = true, string customType = null)
        {
            this.Type = type;
            this.Timestamp = timestamp;
            this.Message = message;
            this.Source = source ?? Logger.Source;
            this.Shortable = shortable;
            this.CustomType = customType ?? this.Type.ToString();
        }

        public static LogEntry Trace(string message)
        {
            return new LogEntry(LogType.Trace, DateTime.Now, message);
        }

        public static LogEntry Warning(string message)
        {
            return new LogEntry(LogType.Warning, DateTime.Now, message);
        }

        public static LogEntry Error(Exception exception)
        {
            if (exception == null)
                return Error(Resources.EmptyException);
            return new LogEntry(LogType.Error, DateTime.Now, Logger.Extension.Format(exception), customType: exception.GetType().ToString());
        }

        public static LogEntry Error(string message, string customType = null, string source = null)
        {
            return new LogEntry(LogType.Error, DateTime.Now, message, source, false, customType);
        }

        public LogEntry Unshortable()
        {
            this.Shortable = false;
            return this;
        }
    }
}