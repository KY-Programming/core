using System;

namespace KY.Core
{
    public class EventTarget : LogTarget
    {
        public event EventHandler<EventArgs<LogEntry>> Added;

        public override void Write(LogEntry entry)
        {
            this.Added?.Invoke(null, new EventArgs<LogEntry>(entry));
        }
    }
}