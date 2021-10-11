using System;
using System.Text;

namespace KY.Core
{
    public static class StringExtension
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static string SubstringSafe(this string value, int startIndex)
        {
            if (value == null)
                throw new NullReferenceException();

            if (startIndex > value.Length)
                return string.Empty;
            return value.Substring(startIndex);
        }

        public static string SubstringSafe(this string value, int startIndex, int length)
        {
            if (value == null)
                throw new NullReferenceException();

            if (startIndex > value.Length)
                return string.Empty;

            length = Math.Min(value.Length - startIndex, length);
            return value.Substring(startIndex, length);
        }

        public static string Format(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string Trim(this string value, string trim)
        {
            return value.TrimStart(trim).TrimEnd(trim);
        }

        public static string TrimEnd(this string value, string trim)
        {
            while (value.EndsWith(trim))
            {
                value = value.Substring(0, value.Length - trim.Length);
            }
            return value;
        }

        public static string TrimStart(this string value, string trim)
        {
            while (value.StartsWith(trim))
            {
                value = value.Substring(trim.Length);
            }
            return value;
        }

        public static string Remove(this string value, string toRemove)
        {
            return value?.Replace(toRemove, string.Empty);
        }

        public static string Repeate(this string value, int length)
        {
            while (value.Length < length)
            {
                value += value;
            }
            return value.Substring(0, length);
        }

        public static string FirstCharToLower(this string value)
        {
            if (!string.IsNullOrEmpty(value) && value != value.ToUpper())
            {
                value = value[0].ToString().ToLower() + value.Substring(1);
            }
            return value;
        }

        public static string FirstCharToUpper(this string value)
        {
            return string.IsNullOrEmpty(value) ? value : value[0].ToString().ToUpper() + value.Substring(1);
        }

        public static string Fallback(this string value, string fallback)
        {
            return string.IsNullOrEmpty(value) ? fallback : value;
        }

        public static string PadLeft(this string value, int totalWidth, string text = " ", bool exact = false)
        {
            if (value == null || text == null || value.Length >= totalWidth)
            {
                return value;
            }
            StringBuilder builder = new();
            while (builder.Length + value.Length < totalWidth)
            {
                builder.Append(text);
            }
            builder.Append(value);
            return exact ? builder.ToString().Substring(0, totalWidth) : builder.ToString();
        }

        public static string PadRight(this string value, int totalWidth, string text = " ", bool exact = false)
        {
            if (value == null || text == null || value.Length >= totalWidth)
            {
                return value;
            }
            StringBuilder builder = new();
            builder.Append(value);
            while (builder.Length < totalWidth)
            {
                builder.Append(text);
            }
            return exact ? builder.ToString().Substring(0, totalWidth) : builder.ToString();
        }
    }
}
