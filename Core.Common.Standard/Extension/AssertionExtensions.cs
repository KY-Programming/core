using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KY.Core
{
    public static class AssertionExtensions
    {
        [DebuggerHidden]
        public static double AssertIsPositive(this double self, string argumentName = "")
        {
            if (self <= 0)
                throw new ArgumentOutOfRangeException(argumentName, self, "Has to be positive!");

            return self;
        }

        [DebuggerHidden]
        public static double AssertHasValueAndIsPositive(this double? self, string argumentName = "")
        {
            return self.AssertIsNotNull(argumentName).AssertIsPositive(argumentName);
        }

        [DebuggerHidden]
        public static double? AssertIsNullOrPositive(this double? self, string argumentName = "")
        {
            if (self.HasValue)
                self.Value.AssertIsPositive(argumentName);

            return self;
        }

        [DebuggerHidden]
        public static int AssertIsNotNegative(this int self, string argumentName = "")
        {
            if (self < 0)
                throw new ArgumentOutOfRangeException(argumentName, self, "Has to be positive or 0!");

            return self;
        }

        [DebuggerHidden]
        public static int AssertIsPositive(this int self, string argumentName = "")
        {
            if (self <= 0)
                throw new ArgumentOutOfRangeException(argumentName, self, "Has to be positive!");

            return self;
        }

        [DebuggerHidden]
        public static long AssertIsNotNegative(this long self, string argumentName = "")
        {
            if (self < 0)
                throw new ArgumentOutOfRangeException(argumentName, self, "Has to be positive or 0!");

            return self;
        }

        [DebuggerHidden]
        public static long AssertIsPositive(this long self, string argumentName = "")
        {
            if (self <= 0)
                throw new ArgumentOutOfRangeException(argumentName, self, "Has to be positive!");

            return self;
        }

        [DebuggerHidden]
        public static long AssertHasValueAndIsPositive(this long? self, string argumentName = "")
        {
            return self.AssertIsNotNull(argumentName).AssertIsPositive(argumentName);
        }

        [DebuggerHidden]
        public static long? AssertIsNullOrPositive(this long? self, string argumentName = "")
        {
            if (self.HasValue)
                self.Value.AssertIsPositive(argumentName);

            return self;
        }

        [DebuggerHidden]
        public static string AssertIsNotLongerThan(this string reference, int maxLength, string parameterName = null)
        {
            if (reference.Length > maxLength)
                throw new ArgumentException(parameterName, string.Format("String is too long. Max: {0} / Acctual: {1}", maxLength, reference.Length));
            return reference;
        }

        [DebuggerHidden]
        public static string AssertIsNotNullOrEmpty(this string reference, string parameterName = null)
        {
            reference.AssertIsNotNull(parameterName);
            if (string.IsNullOrEmpty(reference))
                throw new ArgumentOutOfRangeException(parameterName, "Cannot be empty.");
            return reference;
        }

        [DebuggerHidden]
        public static T AssertIsNotNull<T>(this T reference, string parameterName = null)
            where T : class
        {
            if (reference == null)
                throw new ArgumentNullException(parameterName ?? typeof(T).FullName, "Cannot be null");
            return reference;
        }

        [DebuggerHidden]
        public static T AssertIsNotNull<T>(this T? reference, string parameterName = null)
            where T : struct
        {
            if (reference == null)
                throw new ArgumentNullException(parameterName ?? typeof(T).FullName, "Cannot be null");
            return reference.Value;
        }

        [DebuggerHidden]
        public static IEnumerable<long> AssertAllElementsArePositive(this IEnumerable<long> self, string parameterName = null)
        {
            if (self.Any(l => l <= 0))
                throw new ArgumentOutOfRangeException(parameterName, self, "All Elements have to be positive!");

            return self;
        }

        [DebuggerHidden]
        public static IEnumerable<T> AssertContains<T>(this IEnumerable<T> collection, T item, string argumentName = "")
        {
            collection.AssertIsNotNull(argumentName);
            if (!collection.Contains(item))
                throw new InvalidOperationException(argumentName + " collection have to contains child");
            return collection;
        }

        [DebuggerHidden]
        public static IEnumerable<T> AssertIsNotNullOrEmpty<T>(this IEnumerable<T> collection, string argumentName = "")
        {
            collection.AssertIsNotNull(argumentName);
            if (!collection.Any())
                throw new InvalidOperationException(argumentName + " collection have to contain any child");
            return collection;
        }

        [DebuggerHidden]
        public static T AssertIs<T>(this T self, T expected, string argumentName = "")
        {
            if (Equals(self, expected))
                return self;

            throw new ArgumentOutOfRangeException(argumentName, self, "Has to be " + expected);
        }

        [DebuggerHidden]
        public static T AssertIsNot<T>(this T self, T notExpected, string argumentName = "")
        {
            if (!Equals(self, notExpected))
                return self;

            throw new ArgumentOutOfRangeException(argumentName, self, "Mustn't be " + notExpected);
        }
    }
}