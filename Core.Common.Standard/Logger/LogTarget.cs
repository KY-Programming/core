using System;

namespace KY.Core
{
    public abstract class LogTarget
    {
        public abstract void Write(LogEntry entry);

        public void Trace(string message)
        {
            this.Write(LogEntry.Trace(message));
        }

        public void Warning(string message)
        {
            this.Write(LogEntry.Warning(message));
        }

        public void Error(Exception exception)
        {
            this.Write(LogEntry.Error(exception));
        }

        public void Error(string message)
        {
            this.Write(LogEntry.Error(message));
        }
    }
}