using System;
using System.Linq;
using System.Text;

namespace KY.Core
{
    public static class LanguageCoder
    {
        public static string Convert<T>(long number, T[] language)
        {
            ulong convertedNumber = number < 0 ? ulong.MaxValue - long.MaxValue + (ulong)Math.Abs(number) : (ulong)number;
            return Convert(convertedNumber, language);
        }

        public static string Convert<T>(ulong number, T[] language)
        {
            StringBuilder builder = new StringBuilder();
            while (number > 0)
            {
                builder.Append(language[number % (ulong)language.Length]);
                number /= (ulong)language.Length;
            }
            return builder.ToString();
        }

        public static long Convert<T>(string text, T[] language)
        {
            long result = 0;
            int pos = 0;
            foreach (char c in text.Reverse())
            {
                result += Array.IndexOf(language, c) * (long)Math.Pow(language.Length, pos);
                pos++;
            }
            return result;
        }

        public static string Random<T>(int length, T[] language)
        {
            StringBuilder builder = new StringBuilder(length);
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                builder.Append(language[random.Next(language.Length)]);
            }
            return builder.ToString();
        }
    }
}