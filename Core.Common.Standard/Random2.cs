using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KY.Core
{
    public static class Random2
    {
        public const string DefaultAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-!?@";
        public const string Base64Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+/";
        public const string Base32Alphabet = "abcdefghijklmnopqrstuvwxyz234567";
        public const string Base36Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        public const string UrlSafeAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

#if NET6_0_OR_GREATER
        // Random.Shared and the static RandomNumberGenerator members are thread-safe,
        // so no shared mutable state is required on modern targets.
        private static Random Rng => Random.Shared;
#else
        // System.Random is not thread-safe, so give every thread its own instance.
        // Seeding from a Guid avoids the correlated sequences that nearby tick-based
        // seeds produce when several threads create a Random within the same tick.
        [ThreadStatic]
        private static Random threadRandom;
        private static Random Rng => threadRandom ??= new Random(Guid.NewGuid().GetHashCode());

        // RandomNumberGenerator.GetBytes on the shared instance is thread-safe.
        private static readonly RandomNumberGenerator secureRng = RandomNumberGenerator.Create();
#endif

        /// <summary>
        /// Returns a random string with a length in [<paramref name="minLength"/>, <paramref name="maxLength"/>).
        /// The length and every character are drawn from a cryptographically secure random number generator,
        /// so the result is safe to use for tokens, secrets and signing keys.
        /// </summary>
        public static string NextString(int minLength = 5, int maxLength = 25, string alphabet = null)
        {
            alphabet ??= DefaultAlphabet;
            if (alphabet.Length == 0)
            {
                return string.Empty;
            }
            int length = NextSecureInt(minLength, maxLength);
            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int index = NextSecureInt(0, alphabet.Length);
                builder.Append(alphabet[index]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns a uniformly distributed, unbiased integer in [<paramref name="minInclusive"/>, <paramref name="maxExclusive"/>)
        /// drawn from a cryptographic random number generator.
        /// </summary>
        private static int NextSecureInt(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
            {
                return minInclusive;
            }
#if NET6_0_OR_GREATER
            return RandomNumberGenerator.GetInt32(minInclusive, maxExclusive);
#else
            // Rejection sampling to avoid modulo bias, for targets without RandomNumberGenerator.GetInt32.
            uint range = (uint)(maxExclusive - minInclusive);
            // Largest multiple of range within [0, 2^32); reject values at or above it so that "value % range" stays unbiased.
            ulong limit = (ulong)uint.MaxValue + 1UL;
            limit -= limit % range;
            byte[] buffer = new byte[4];
            uint value;
            do
            {
                secureRng.GetBytes(buffer);
                value = BitConverter.ToUInt32(buffer, 0);
            }
            while (value >= limit);
            return (int)(minInclusive + value % range);
#endif
        }

        public static int Next(int minValue = 0, int maxValue = int.MaxValue)
        {
            return Rng.Next(minValue, maxValue);
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
            return min + Rng.NextDouble() * range;
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
