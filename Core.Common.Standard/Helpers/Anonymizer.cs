using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using KY.Core.Extension;

namespace KY.Core
{
    public static class Anonymizer
    {
        private static readonly Regex ipRegex = new(@"(\d+\.\d+\.\d+\.\d+)", RegexOptions.Compiled);
        private static readonly Regex pathRegex = new(@"[a-zA-Z]:\\([^\s\\]*\\)+[^\s\\]*", RegexOptions.Compiled);
        private static readonly Regex urlRegex = new(@"[a-zA-Z]+:\/\/([^\s\/]*\/)*[^\s\/]*", RegexOptions.Compiled);
        private static readonly Regex mailRegex = new(@"[^@\s]+@[^\s]+", RegexOptions.Compiled);

        public static string ReplaceAll(string input)
        {
            input = ReplaceIps(input);
            input = ReplacePaths(input);
            input = ReplaceUrls(input);
            input = ReplaceMails(input);
            return input;
        }

        public static string ReplaceIps(string input)
        {
            return ipRegex.Matches(input).Cast<Match>().Aggregate(input, (current, match) => current.Replace(match.Value, RandomIp()));
        }

        private static string RandomIp()
        {
            return $"{Random2.Next(0, 255)}.{Random2.Next(0, 255)}.{Random2.Next(0, 255)}.{Random2.Next(0, 255)}";
        }

        public static string ReplacePaths(string input)
        {
            return pathRegex.Matches(input).Cast<Match>().Aggregate(input, (current, match) => current.Replace(match.Value, Replace(match.Value, "/", "\\", ":", ".")));
        }

        public static string ReplaceUrls(string input)
        {
            return urlRegex.Matches(input).Cast<Match>().Aggregate(input, (current, match) => current.Replace(match.Value, Replace(match.Value, "http://", "https://", "/", "\\", ":", ".", "?", "&", "=")));
        }

        public static string ReplaceMails(string input)
        {
            return mailRegex.Matches(input).Cast<Match>().Aggregate(input, (current, match) => current.Replace(match.Value, Replace(match.Value, "@", ".")));
        }

        public static string Replace(string input, params string[] keep)
        {
            return Replace(input, StringComparison.OrdinalIgnoreCase, keep);
        }

        public static string Replace(string input, StringComparison comparison, params string[] keep)
        {
            StringBuilder builder = new();
            while (input.Length > 0)
            {
                string valueFound = null;
                foreach (string value in keep)
                {
                    if (input.StartsWith(value, comparison))
                    {
                        valueFound = input.Substring(0, value.Length);
                        break;
                    }
                }
                if (valueFound == null)
                {
                    switch (input[0].GetCaseType())
                    {
                        case CaseType.Lower:
                            valueFound = Random2.NextString(1, 1, "abcdefghijklmnopqrstuvwxyz");
                            break;
                        case CaseType.Upper:
                            valueFound = Random2.NextString(1, 1, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                            break;
                        case CaseType.Number:
                            valueFound = Random2.NextString(1, 1, "0123456789").ToLower();
                            break;
                        case CaseType.Special:
                            valueFound = Random2.NextString(1, 1, ",;.:-_#'+*~!\"§$%&/()=?`´\\}][{}@<>|").ToLower();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                builder.Append(valueFound);
                input = input.Substring(valueFound.Length, input.Length - valueFound.Length);
            }

            return builder.ToString();
        }
    }
}
