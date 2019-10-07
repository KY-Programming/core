using System;
using System.Diagnostics;

namespace KY.Core
{
    public class EventLogTarget : LogTarget
    {
        private readonly string eventLogSource;
        private readonly string eventLogName;
        private readonly object eventLogeMutex = new object();

        public EventLogTarget(string eventLogSource, string eventLogName)
        {
            this.eventLogSource = eventLogSource ?? throw new ArgumentNullException(nameof(eventLogSource));
            this.eventLogName = eventLogName ?? throw new ArgumentNullException(nameof(eventLogName));
        }

        public override void Write(LogEntry entry)
        {
#if !NETSTANDARD2_0
            try
            {
                lock (this.eventLogeMutex)
                {
                    if (!EventLog.SourceExists(this.eventLogSource))
                    {
                        EventLog.CreateEventSource(this.eventLogSource, this.eventLogName);
                    }
                    using (EventLog log = new EventLog())
                    {
                        log.Source = this.eventLogSource;
                        log.WriteEntry(entry.Message, EventLogEntryType.Error);
                    }
                }
            }
            catch
            { }
#endif
        }
    }
}