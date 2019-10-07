using System;

namespace KY.Core
{
    public class EventArgs<T> : EventArgs
    {
        public T Value { get; private set; }

        public EventArgs(T value)
        {
            this.Value = value;
        }
    }

    public class EventArgs<T1, T2> : EventArgs
    {
        public T1 Value1 { get; private set; }

        public T2 Value2 { get; private set; }

        public EventArgs(T1 value1, T2 value2)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }
    }

    public class EventArgs<T1, T2, T3> : EventArgs<T1, T2>
    {
        public T3 Value3 { get; private set; }

        public EventArgs(T1 value1, T2 value2, T3 value3)
            : base(value1, value2)
        {
            this.Value3 = value3;
        }
    }

    public class EventArgs<T1, T2, T3, T4> : EventArgs<T1, T2, T3>
    {
        public T4 Value4 { get; private set; }

        public EventArgs(T1 value1, T2 value2, T3 value3, T4 value4)
            : base(value1, value2, value3)
        {
            this.Value4 = value4;
        }
    }
}