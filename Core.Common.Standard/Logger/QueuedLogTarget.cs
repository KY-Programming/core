using System;
using System.Collections.Generic;
using System.Threading;
using Timer = System.Timers.Timer;

namespace KY.Core
{
    public abstract class QueuedLogTarget : LogTarget
    {
        private readonly TimeSpan waitTime;
        private readonly TimeSpan sleepTime;
        private readonly Queue<LogEntry> queue;
        private readonly ManualResetEvent resetEvent;
        private readonly Timer sleepTimer;

        protected QueuedLogTarget(TimeSpan? waitTime = null, TimeSpan? sleepTime = null)
        {
            this.waitTime = waitTime ?? TimeSpan.FromMinutes(1);
            this.sleepTime = sleepTime ?? TimeSpan.FromMinutes(15);
            this.queue = new Queue<LogEntry>();
            this.resetEvent = new ManualResetEvent(true);
            this.sleepTimer = new Timer();
            this.sleepTimer.AutoReset = false;
            this.sleepTimer.Elapsed += (sender, args) => this.WorkQueue();
        }

        public sealed override void Write(LogEntry entry)
        {
            lock (this.queue)
            {
                this.resetEvent.Reset();
                this.queue.Enqueue(entry);
                if (!this.sleepTimer.Enabled)
                {
                    this.sleepTimer.Interval = this.waitTime.TotalMilliseconds;
                    this.sleepTimer.Start();
                }
            }
        }

        private void WorkQueue()
        {
            lock (this.queue)
            {
                if (this.queue.Count > 0)
                {
                    List<LogEntry> entries = this.queue.DequeueAll();
                    if (!this.WorkEntries(entries))
                    {
                        // Nothing can have been queued meanwhile (Write waits for the lock), so they keep their order
                        // and go out with the next run. No limit: a receiver that is down for good is noticed anyway.
                        entries.ForEach(this.queue.Enqueue);
                    }
                    // Also when they stay queued: Wait (on shutdown) must not block on a receiver that is down
                    this.resetEvent.Set();
                }
            }
            this.sleepTimer.Interval = this.sleepTime.TotalMilliseconds;
            this.sleepTimer.Start();
        }

        /// <returns>False when the entries could not be handled and should be tried again with the next run.</returns>
        protected abstract bool WorkEntries(List<LogEntry> entries);

        public void Wait()
        {
            if (this.sleepTimer.Enabled)
            {
                this.sleepTimer.Stop();
                this.WorkQueue();
            }
            this.resetEvent.WaitOne();
        }
    }
}