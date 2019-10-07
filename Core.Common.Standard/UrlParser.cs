using System;
using System.Text.RegularExpressions;

namespace KY.Core
{
    public static class UrlParser
    {
        private static readonly Regex Regex = new Regex(@"(?<protocol>\w+:\/\/)?((?<subdomain>.*)\.)?(?<domain>[^.]+)\.(?<topleveldomain>[^\/:]+)(:(?<port>\d+))?(?<path>\/.*)?", RegexOptions.IgnoreCase);

        public static string GetDomainAndTopLevelDomain(Uri uri)
        {
            return GetDomainAndTopLevelDomain(uri.OriginalString);
        }

        public static string GetDomainAndTopLevelDomain(string url)
        {
            Match match = Regex.Match(url);
            if (!match.Success)
                return null;

            string domain = match.Domain();
            var tld = match.TopLevelDomain();
            return string.Join(".", domain, tld);
        }

        public static string GetSubdomainDomainAndTopLevelDomain(Uri uri)
        {
            return GetSubdomainDomainAndTopLevelDomain(uri.OriginalString);
        }

        public static string GetSubdomainDomainAndTopLevelDomain(string url)
        {
            Match match = Regex.Match(url);
            if (!match.Success)
                return null;

            string subdomain = match.Subdomain();
            if (string.IsNullOrEmpty(subdomain))
                return string.Empty;

            string domain = match.Domain();
            string tld = match.TopLevelDomain();
            return string.Join(".", subdomain, domain, tld);
        }

        public static string GetSubdomain(Uri uri)
        {
            return GetSubdomain(uri.OriginalString);
        }

        public static string GetSubdomain(string url)
        {
            Match match = Regex.Match(url);
            if (!match.Success)
                return null;

            return match.Subdomain();
        }

        private static string Protocol(this Match match)
        {
            return match.Groups["protocol"].Value;
        }

        private static string Domain(this Match match)
        {
            return match.Groups["domain"].Value;
        }

        private static string Subdomain(this Match match)
        {
            return match.Groups["subdomain"].Value;
        }

        private static string TopLevelDomain(this Match match)
        {
            return match.Groups["topleveldomain"].Value;
        }

        private static string Path(this Match match)
        {
            return match.Groups["path"].Value;
        }

        private static string Port(this Match match)
        {
            return match.Groups["port"].Value;
        }
    }
}