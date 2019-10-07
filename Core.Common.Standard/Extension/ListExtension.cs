using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KY.Core
{
    public static class ListExtension
    {
        [DebuggerHidden]
        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                list.Add(item);
            }
            return list;
        }

        [DebuggerHidden]
        public static List<T> YieldList<T>(this T item)
        {
            return new List<T>(item.Yield());
        }

        [DebuggerHidden]
        public static IList<T> InsertRangeReverse<T>(this IList<T> list, int index, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                list.Insert(index, item);
            }
            return list;
        }

        [DebuggerHidden]
        public static IList<T> InsertRange<T>(this IList<T> list, int index, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                list.Insert(index++, item);
            }
            return list;
        }

        [DebuggerHidden]
        public static IList<T> RemoveRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                list.Remove(item);
            }
            return list;
        }

        [DebuggerHidden]
        public static IList<T> ReplaceRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            return list.RemoveRange(items).AddRange(items);
        }

        [DebuggerHidden]
        public static IList<T> Replace<T>(this IList<T> list, T replace, T with)
        {
            int index = list.IndexOf(replace);
            if (index < 0)
                throw new InvalidOperationException("Item to replace not found in list");

            list[index] = with;
            return list;
        }

        [DebuggerHidden]
        public static T Min<T>(this IList<T> source, Func<T, T> selector, T defaultValue)
        {
            if (source == null || source.Count == 0)
                return defaultValue;
            return source.Min(selector);
        }

        [DebuggerHidden]
        public static TSource MinBy<TSource, TKey>(this IList<TSource> source, Func<TSource, TKey> selector, TSource defaultValue)
        {
            if (source == null || source.Count == 0)
                return defaultValue;
            return source.MinBy(selector);
        }

        [DebuggerHidden]
        public static TKey MinValue<TSource, TKey>(this IList<TSource> source, Func<TSource, TKey> selector, TKey defaultValue)
        {
            if (source == null || source.Count == 0)
                return defaultValue;
            return selector(source.MinBy(selector));
        }

        [DebuggerHidden]
        public static TKey MaxValue<TSource, TKey>(this IList<TSource> source, Func<TSource, TKey> selector, TKey defaultValue)
        {
            if (source == null || source.Count == 0)
                return defaultValue;
            return selector(source.MaxBy(selector));
        }

        public static T Dequeue<T>(this IList<T> list)
        {
            T first = list.FirstOrDefault();
            list.Remove(first);
            return first;
        }

        public static IList<T> Unique<T>(this IList<T> source)
        {
            List<T> list = new List<T>();
            foreach (T entry in source)
            {
                if (!list.Contains(entry))
                {
                    list.Add(entry);
                }
            }
            return list;
        }
    }
}