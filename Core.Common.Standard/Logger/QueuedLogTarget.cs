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
                    this.WorkEntries(this.queue.DequeueAll());
                    this.resetEvent.Set();
                }
            }
            this.sleepTimer.Interval = this.sleepTime.TotalMilliseconds;
            this.sleepTimer.Start();
        }

        protected abstract void WorkEntries(List<LogEntry> entry);

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