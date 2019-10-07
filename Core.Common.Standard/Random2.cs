using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KY.Core
{
    public static class Random2
    {
        private static readonly Random random = new Random();
        private static string defaultAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-!?@";

        public static string NextString(int minLength = 5, int maxLength = 25, string alphabet = null)
        {
            alphabet = alphabet ?? defaultAlphabet;
            int length = random.Next(minLength, maxLength);
            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(alphabet.Length - 1);
                builder.Append(alphabet[index]);
            }
            return builder.ToString();
        }

        public static int Next(int minValue = 0, int maxValue = int.MaxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public static TimeSpan Next(TimeSpan? minValue, TimeSpan? maxValue = null)
        {
            double min = minValue?.TotalMilliseconds ?? TimeSpan.MinValue.TotalMilliseconds;
            double max = maxValue?.TotalMilliseconds ?? TimeSpan.MaxValue.TotalMilliseconds;
            return TimeSpan.FromMilliseconds(Next(min, max));
        }

        public static double Next(double min, double max = Double.MaxValue)
        {
            double range = max - min;
            return min + random.NextDouble() * range;
        }

        public static T Next<T>(IList<T> list)
        {
            return list.Count == 0 ? default(T) : list[Next(0, list.Count)];
        }

        public static KeyValuePair<TKey, TValue> Next<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            return Next(dictionary.ToList());
        }

        public static T Dequeue<T>(IList<T> list)
        {
            T next = Next(list);
            list.Remove(next);
            return next;
        }

        public static IList<T> Unorder<T>(IList<T> list)
        {
            Dictionary<T, int> order = list.ToDictionary(x => x, x => Next());
            return list.OrderBy(x => order[x]).ToList();
        }
    }
}