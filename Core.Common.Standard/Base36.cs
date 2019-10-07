namespace KY.Core
{
    public static class Base36
    {
        private static readonly char[] language = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public static string Convert(long number)
        {
            return LanguageCoder.Convert(number, language);
        }

        public static string Convert(ulong number)
        {
            return LanguageCoder.Convert(number, language);
        }

        public static long Convert(string text)
        {
            return LanguageCoder.Convert(text, language);
        }

        public static string Random(int length)
        {
            return LanguageCoder.Random(length, language);
        }
    }
}