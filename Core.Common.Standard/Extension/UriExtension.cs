using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace KY.Core
{
    public static class UriExtension
    {
        //private static Regex regex = new Regex(@"^(?<protocol>http|https):\/\/((?<subdomains>\w+)\.)*(?<domain>\w+)(:(?<port>\d+))?(\.(?<tld>\w))?(?<query>\/?.*)?$");

        public static Uri WithoutSubdomain(this Uri uri)
        {
            //Match match = regex.Match(uri.OriginalString);
            //if (!match.Success)
            //{
            //    throw new InvalidOperationException("Can not parse url #23df");
            //}
            //string tld = match.Groups["tld"].Value;
            //tld = string.IsNullOrEmpty(tld) ? string.Empty : $".{tld}";
            //string port = match.Groups["port"].Value;
            //port = string.IsNullOrEmpty(port) ? string.Empty : $":{port}";
            //return new Uri($"{match.Groups["protocol"].Value}://{match.Groups["domain"].Value}{port}{tld}{match.Groups["query"].Value}", uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
            string[] chunks = uri.Authority.Split('.');
            string authority = string.Join(".", chunks.Skip(Math.Max(0, chunks.Length - 2)));
            return new Uri(uri.OriginalString.Replace(uri.Authority, authority), uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }
    }
}
