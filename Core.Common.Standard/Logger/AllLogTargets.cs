using System.Collections;
using System.Collections.Generic;

namespace KY.Core
{
    public class AllLogTargets : LogTarget, IEnumerable<LogTarget>
    {
        private readonly LogTargets[] targets;

        public AllLogTargets(params LogTargets[] targets)
        {
            this.targets = targets;
        }

        public void Clear()
        {
            this.targets.ForEach(x => x.Clear());
        }

        public void Add(LogTarget target)
        {
            this.targets.ForEach(x => x.Add(target));
        }

        public void Remove(LogTarget target)
        {
            this.targets.ForEach(x => x.Remove(target));
        }

        public override void Write(LogEntry entry)
        {
            this.targets.ForEach(x => x.Write(entry));
        }

        public IEnumerator<LogTarget> GetEnumerator()
        {
            List<LogTarget> list = new List<LogTarget>();
            foreach (LogTargets logTargets in this.targets)
            {
                foreach (LogTarget target in logTargets)
                {
                    if (!list.Contains(target))
                    {
                        list.Add(target);
                    }
                }
            }
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}