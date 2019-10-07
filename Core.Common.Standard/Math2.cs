using System;
using System.Linq;

namespace KY.Core
{
    public static class Math2
    {
        //public static DateTime Min(DateTime left, DateTime right)
        //{
        //    return right < left ? right : left;
        //}

        //public static DateTime Max(DateTime left, DateTime right)
        //{
        //    return right > left ? right : left;
        //}

        public static T Max<T>(params T[] values)
        {
            return values.Max();
        }

        public static T Min<T>(params T[] values)
        {
            return values.Min();
        }
    }
}