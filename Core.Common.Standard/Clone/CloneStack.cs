using System;
using System.Collections.Generic;

namespace KY.Core.Clone
{
    internal class CloneStack
    {
        private static CloneStack openStack;
        private static int openStackCount = 0;
        public Dictionary<object, object> Mapping { get; } = new();
        internal List<DelayedSetter> DelayedSetters { get; } = new();

        public void Open()
        {
            if (openStack != null && openStack != this)
            {
                throw new InvalidOperationException("Can not open clone stack. Another clone operation already running.");
            }
            openStackCount++;
            openStack = this;
        }

        public void Close()
        {
            openStackCount--;
            if (openStackCount <= 0)
            {
                openStack = null;
            }
        }

        public static CloneStack Continue()
        {
            return openStack;
        }
    }
}
