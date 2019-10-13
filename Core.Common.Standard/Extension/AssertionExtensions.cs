using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace KY.Core
{
    public static class AssertionExtensions
    {
        private const string DefaultMessage = "[Message will be replaced in method body]";

        [DebuggerHidden]
        public static double AssertIsPositive(this double self, string argumentName = "", string message = "Has to be positive")
        {
            if (self <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, self, message);
            }

            return self;
        }

        [DebuggerHidden]
        public static double AssertIsPositive(this double? self, string argumentName = "", string message = "Has to be positive")
        {
            return self.AssertIsNotNull(argumentName, message).AssertIsPositive(argumentName, message);
        }

        [DebuggerHidden]
        public static double? AssertIsNullOrPositive(this double? self, string argumentName = "", string message = "Has to be null or positive")
        {
            self?.AssertIsPositive(argumentName, message);
            return self;
        }

        [DebuggerHidden]
        public static int AssertIsNotNegative(this int self, string argumentName = "", string message = "Has to be positive or 0")
        {
            if (self < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, self, message);
            }
            return self;
        }

        [DebuggerHidden]
        public static int AssertIsPositive(this int self, string argumentName = "", string message = "Has to be positive")
        {
            if (self <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, self, message);
            }
            return self;
        }

        [DebuggerHidden]
        public static long AssertIsNotNegative(this long self, string argumentName = "", string message = "Has to be positive or 0")
        {
            if (self < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, self, message);
            }
            return self;
        }

        [DebuggerHidden]
        public static long AssertIsPositive(this long self, string argumentName = "", string message = "Has to be positive")
        {
            if (self <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, self, message);
            }
            return self;
        }

        [DebuggerHidden]
        public static long AssertIsPositive(this long? self, string argumentName = "", string message = "Has to be positive")
        {
            return self.AssertIsNotNull(argumentName, message).AssertIsPositive(argumentName, message);
        }

        [DebuggerHidden]
        public static long? AssertIsNullOrPositive(this long? self, string argumentName = "", string message = "Has to be null or positive")
        {
            self?.AssertIsPositive(argumentName, message);
            return self;
        }

        [DebuggerHidden]
        public static string AssertIsNotLongerThan(this string reference, int maxLength, string parameterName = null, string message = DefaultMessage)
        {
            if (reference.Length > maxLength)
            {
                throw new ArgumentException(parameterName, message == DefaultMessage ? $"String is too long. Max: {maxLength} / Actual: {reference.Length}" : message);
            }
            return reference;
        }

        [DebuggerHidden]
        public static string AssertIsNotNullOrEmpty(this string reference, string parameterName = null, string message = "Cannot be null or empty")
        {
            if (string.IsNullOrEmpty(reference))
            {
                throw new ArgumentOutOfRangeException(parameterName, message);
            }
            return reference;
        }

        [DebuggerHidden]
        public static T AssertIsNotNull<T>(this T reference, string parameterName = null, string message = "Cannot be null")
            where T : class
        {
            if (reference == null)
            {
                throw new ArgumentNullException(parameterName ?? typeof(T).FullName, message);
            }
            return reference;
        }

        [DebuggerHidden]
        public static T AssertIsNotNull<T>(this T? reference, string parameterName = null, string message = "Cannot be null")
            where T : struct
        {
            if (reference == null)
            {
                throw new ArgumentNullException(parameterName ?? typeof(T).FullName, message);
            }
            return reference.Value;
        }

        [DebuggerHidden]
        public static IEnumerable<long> AssertAllElementsArePositive(this IEnumerable<long> self, string parameterName = null, string message = "All Elements have to be positive")
        {
            if (self.Any(l => l <= 0))
            {
                throw new ArgumentOutOfRangeException(parameterName, self, message);
            }
            return self;
        }

        [DebuggerHidden]
        public static IEnumerable<T> AssertContains<T>(this IEnumerable<T> collection, T item, string argumentName = "", string message = DefaultMessage)
        {
            collection.AssertIsNotNull(argumentName);
            if (collection.Contains(item))
            {
                throw new InvalidOperationException(message == DefaultMessage ? argumentName + " collection have to contains child" : message);
            }
            return collection;
        }

        [DebuggerHidden]
        public static IEnumerable<T> AssertIsNotNullOrEmpty<T>(this IEnumerable<T> collection, string argumentName = "", string message = DefaultMessage)
        {
            collection.AssertIsNotNull(argumentName);
            if (collection.Any())
            {
                throw new InvalidOperationException(message == DefaultMessage ? argumentName + " collection have to contain any child" : message);
            }
            return collection;
        }

        [DebuggerHidden]
        public static T AssertIs<T>(this T self, T expected, string argumentName = "", string message = DefaultMessage)
        {
            if (Equals(self, expected))
            {
                return self;
            }
            throw new ArgumentOutOfRangeException(argumentName, self, message == DefaultMessage ? "Has to be " + expected : message);
        }

        [DebuggerHidden]
        public static T AssertIsNot<T>(this T self, T notExpected, string argumentName = "", string message = DefaultMessage)
        {
            if (Equals(self, notExpected))
            {
                return self;
            }

            throw new ArgumentOutOfRangeException(argumentName, self, message == DefaultMessage ? "Mustn't be " + notExpected : message);
        }
    }
}