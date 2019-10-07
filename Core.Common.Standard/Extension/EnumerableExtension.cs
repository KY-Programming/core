using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace KY.Core
{
    public static class EnumerableExtension
    {
        [DebuggerHidden]
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
        
        [DebuggerHidden]
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }
        
        [DebuggerHidden]
        public static ReadOnlyCollection<T> ToReadOnlyList<T>(this IEnumerable<T> items)
        {
            return new ReadOnlyCollection<T>(items?.ToList() ?? new List<T>());
        }

        [DebuggerHidden]
        public static TKey MaxValue<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, TKey defaultValue)
        {
            if (source == null || !source.Any())
                return defaultValue;
            return selector(source.MaxBy(selector));
        }

        [DebuggerHidden]
        public static TKey MinValue<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, TKey defaultValue)
        {
            if (source == null || !source.Any())
                return defaultValue;
            return selector(source.MinBy(selector));
        }
        
        [DebuggerHidden]
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }
        
        [DebuggerHidden]
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence was empty");
                }
                TSource min = sourceIterator.Current;
                TKey minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }
        
        [DebuggerHidden]
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }
        
        [DebuggerHidden]
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements");

                TSource max = sourceIterator.Current;
                TKey maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }
        
        [DebuggerHidden]
        public static TSource MaxByOrDefault<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxByOrDefault(selector, Comparer<TKey>.Default);
        }
        
        [DebuggerHidden]
        public static TSource MaxByOrDefault<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    return default(TSource);

                TSource max = sourceIterator.Current;
                TKey maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }

        public static IEnumerable<T> Unique<T>(this IEnumerable<T> source)
        {
            List<T> found = new List<T>();
            foreach (T entry in source)
            {
                if (!found.Contains(entry))
                {
                    found.Add(entry);
                    yield return entry;
                }
            }
        }

        public static TimeSpan Sum<T>(this IEnumerable<T> source, Func<T, TimeSpan> action)
        {
            TimeSpan result = default(TimeSpan);
            foreach (T entry in source)
            {
                result += action(entry);
            }
            return result;
        }

        public static T Second<T>(this IEnumerable<T> source)
        {
            return source.Skip(1).First();
        }

        public static T Second<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Skip(1).First(predicate);
        }

        public static T SecondOrDefault<T>(this IEnumerable<T> source)
        {
            return source.Skip(1).FirstOrDefault();
        }

        public static T SecondOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Skip(1).FirstOrDefault(predicate);
        }
    }
}