namespace KY.Core
{
    public static class Base32
    {
        private static readonly char[] language = "12345678ABCDEFGHIJKLMNPQRSTUVWXY".ToCharArray();

        public static string Convert(long number)
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