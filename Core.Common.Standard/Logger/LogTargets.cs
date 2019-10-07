using System.Collections.ObjectModel;
using System.Linq;

namespace KY.Core
{
    public class LogTargets : Collection<LogTarget>
    {
        protected override void InsertItem(int index, LogTarget item)
        {
            if (this.Contains(item))
                return;

            base.InsertItem(index, item);
        }

        public void Write(LogEntry entry)
        {
            this.ForEach(x => x.Write(entry));
        }

        public LogTargets Except(params LogTarget[] except)
        {
            LogTargets targets = new LogTargets();
            foreach (LogTarget target in this)
            {
                if (!except.Contains(target))
                {
                    targets.Add(target);
                }
            }
            return targets;
        }
    }
}