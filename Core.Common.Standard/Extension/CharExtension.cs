using System.Text.RegularExpressions;

namespace KY.Core.Extension
{
    public static class CharExtension
    {
        public static CaseType GetCaseType(this char value)
        {
            string input = value.ToString();
            if (Regex.IsMatch(input, "[a-z]"))
            {
                return CaseType.Lower;
            }
            if (Regex.IsMatch(input, "[A-Z]"))
            {
                return CaseType.Upper;
            }
            if (Regex.IsMatch(input, "[0-9]"))
            {
                return CaseType.Number;
            }
            return CaseType.Special;
        }
    }

    public enum CaseType
    {
        Lower,
        Upper,
        Number,
        Special
    }
}
