using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KY.Core
{
    public class SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>, IComparable<Version>, IEquatable<Version>
    {
        private static readonly Regex regex = new(@"^(?<major>\d+)(?<majorPre>[^.]+)?\.(?<minor>\d+)(?<minorPre>[^.]+)?(\.(?<build>\d+)(?<buildPre>[^.]+)?(\.(?<revision>\d+)(?<revisionPre>.+)?)?)?$", RegexOptions.Compiled);

        public int Major { get; }
        public string MajorPre { get; } = string.Empty;
        public int Minor { get; }
        public string MinorPre { get; } = string.Empty;
        public int Build { get; }
        public string BuildPre { get; } = string.Empty;
        public int Revision { get; }
        public string RevisionPre { get; } = string.Empty;

        public bool IsPreVersion => !string.IsNullOrEmpty(this.MajorPre) || !string.IsNullOrEmpty(this.MinorPre) || !string.IsNullOrEmpty(this.BuildPre) || !string.IsNullOrEmpty(this.RevisionPre);

        public SemanticVersion(int major, int minor, int build = -1, int revision = -1)
        {
            this.Major = major.AssertIsPositive();
            this.Minor = minor.AssertIsPositive();
            this.Build = build;
            this.Revision = revision;
        }

        public SemanticVersion(string version)
        {
            Match match = regex.Match(version);
            match.AssertIsNotNull(message: "Can not parse SemanticVersion");
            this.Major = int.Parse(match.Groups["major"].Value);
            this.MajorPre = match.Groups["majorPre"].Value;
            this.Minor = int.Parse(match.Groups["minor"].Value);
            this.MinorPre = match.Groups["minorPre"].Value;
            this.Build = match.Groups["build"].Length == 0 ? -1 : int.Parse(match.Groups["build"].Value);
            this.BuildPre = match.Groups["buildPre"].Value;
            this.Revision = match.Groups["revision"].Length == 0 ? -1 : int.Parse(match.Groups["revision"].Value);
            this.RevisionPre = match.Groups["revisionPre"].Value;
        }

        public int CompareTo(SemanticVersion other)
        {
            if (this.Major != other.Major)
                return this.Major > other.Major ? 1 : -1;
            if (this.MajorPre != other.MajorPre)
                return this.MajorPre == string.Empty ? 1 : -1;
            if (this.Minor != other.Minor)
                return this.Minor > other.Minor ? 1 : -1;
            if (this.MinorPre != other.MinorPre)
                return this.MinorPre == string.Empty ? 1 : -1;
            if (this.Build != other.Build)
                return this.Build > other.Build ? 1 : -1;
            if (this.BuildPre != other.BuildPre)
                return this.BuildPre == string.Empty ? 1 : -1;
            if (this.Revision != other.Revision)
                return this.Revision > other.Revision ? 1 : -1;
            if (this.RevisionPre != other.RevisionPre)
                return this.RevisionPre == string.Empty ? 1 : -1;
            return 0;
        }

        public int CompareTo(Version other)
        {
            if (this.Major != other.Major)
                return this.Major > other.Major ? 1 : -1;
            if (this.MajorPre !=  string.Empty)
                return -1;
            if (this.Minor != other.Minor)
                return this.Minor > other.Minor ? 1 : -1;
            if (this.MinorPre != string.Empty)
                return -1;
            if (this.Build != other.Build)
                return this.Build > other.Build ? 1 : -1;
            if (this.BuildPre != string.Empty)
                return -1;
            if (this.Revision != other.Revision)
                return this.Revision > other.Revision ? 1 : -1;
            if (this.RevisionPre != string.Empty)
                return -1;
            return 0;
        }

        public bool Equals(SemanticVersion other)
        {
            return other != null && this.Major == other.Major && this.Minor == other.Minor && this.Revision == other.Revision && this.Build == other.Build
                   && this.MajorPre == other.MajorPre && this.MinorPre == other.MinorPre && this.BuildPre == other.BuildPre && this.RevisionPre == other.RevisionPre;
        }

        public bool Equals(Version other)
        {
            return other != null && this.Major == other.Major && this.Minor == other.Minor && this.Revision == other.Revision && this.Build == other.Build;
        }

        public override string ToString()
        {
            return this.ToString(4);
        }

        public string ToString(int fieldCount)
        {
            List<string> parts = new();
            if (fieldCount >= 1)
            {
                parts.Add(this.MajorPre == string.Empty ? this.Major.ToString() : $"{this.Major}{this.MajorPre}");
            }
            if (fieldCount >= 2)
            {
                parts.Add(this.MinorPre == string.Empty ? this.Minor.ToString() : $"{this.Minor}{this.MinorPre}");
            }
            if (fieldCount >= 3 && this.Build >= 0)
            {
                parts.Add(this.BuildPre == string.Empty ? this.Build.ToString() : $"{this.Build}{this.BuildPre}");
            }
            if (fieldCount >= 4 && this.Revision >= 0)
            {
                parts.Add(this.RevisionPre == string.Empty ? this.Revision.ToString() : $"{this.Revision}{this.RevisionPre}");
            }
            return string.Join(".", parts);
        }
    }
}
