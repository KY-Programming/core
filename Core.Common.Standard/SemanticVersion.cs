using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KY.Core;

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

    public SemanticVersion(int major, int minor = -1, int build = -1, int revision = -1)
    {
        this.Major = major.AssertIsNotNegative();
        this.Minor = minor;
        this.Build = build;
        this.Revision = revision;
    }

    public SemanticVersion(string version)
    {
        Match match = regex.Match(version);
        match.AssertIsNotNull(message: "Can not parse SemanticVersion");
        this.Major = match.Groups["major"].Length == 0 ? 0 : int.Parse(match.Groups["major"].Value);
        this.MajorPre = match.Groups["majorPre"].Value;
        this.Minor = match.Groups["minor"].Length == 0 ? -1 : int.Parse(match.Groups["minor"].Value);
        this.MinorPre = match.Groups["minorPre"].Value;
        this.Build = match.Groups["build"].Length == 0 ? -1 : int.Parse(match.Groups["build"].Value);
        this.BuildPre = match.Groups["buildPre"].Value;
        this.Revision = match.Groups["revision"].Length == 0 ? -1 : int.Parse(match.Groups["revision"].Value);
        this.RevisionPre = match.Groups["revisionPre"].Value;
    }

    public SemanticVersion(Version version)
        : this(version.Major, version.Minor, version.Build, version.Revision)
    { }

    public int CompareTo(SemanticVersion? other)
    {
        return this.CompareTo(other, Compare.All);
    }

    public int CompareTo(SemanticVersion? other, Compare compare)
    {
        if (other is null)
        {
            return 1;
        }
        if (compare.HasFlag(Compare.Major) && this.Major != other.Major)
        {
            return this.Major > other.Major ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Major) && this.MajorPre != other.MajorPre)
        {
            return this.MajorPre == string.Empty ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Minor) && this.Minor != other.Minor)
        {
            return this.Minor > other.Minor ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Minor) && this.MinorPre != other.MinorPre)
        {
            return this.MinorPre == string.Empty ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Build) && this.Build != other.Build)
        {
            return this.Build > other.Build ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Build) && this.BuildPre != other.BuildPre)
        {
            return this.BuildPre == string.Empty ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Revision) && this.Revision != other.Revision)
        {
            return this.Revision > other.Revision ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Revision) && this.RevisionPre != other.RevisionPre)
        {
            return this.RevisionPre == string.Empty ? 1 : -1;
        }
        return 0;
    }

    public int CompareTo(Version? other)
    {
        return this.CompareTo(other, Compare.All);
    }

    public int CompareTo(Version? other, Compare compare)
    {
        if (other is null)
        {
            return 1;
        }
        if (compare.HasFlag(Compare.Major) && this.Major != other.Major)
        {
            return this.Major > other.Major ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Major) && this.MajorPre != string.Empty)
        {
            return -1;
        }
        if (compare.HasFlag(Compare.Minor) && this.Minor != other.Minor)
        {
            return this.Minor > other.Minor ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Minor) && this.MinorPre != string.Empty)
        {
            return -1;
        }
        if (compare.HasFlag(Compare.Build) && this.Build != other.Build)
        {
            return this.Build > other.Build ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Build) && this.BuildPre != string.Empty)
        {
            return -1;
        }
        if (compare.HasFlag(Compare.Revision) && this.Revision != other.Revision)
        {
            return this.Revision > other.Revision ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Revision) && this.RevisionPre != string.Empty)
        {
            return -1;
        }
        return 0;
    }

    public bool Equals(SemanticVersion? other)
    {
        return this.CompareTo(other) == 0;
    }

    public bool Equals(SemanticVersion? other, Compare compare)
    {
        return this.CompareTo(other, compare) == 0;
    }

    public bool Equals(Version? other)
    {
        return this.CompareTo(other) == 0;
    }

    public bool Equals(Version? other, Compare compare)
    {
        return this.CompareTo(other, compare) == 0;
    }

    public override bool Equals(object? obj)
    {
        if (obj is SemanticVersion semanticVersion)
        {
            return this.Equals(semanticVersion);
        }
        if (obj is Version version)
        {
            return this.Equals(version);
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + this.Major.GetHashCode();
            hash = hash * 23 + this.MajorPre.GetHashCode();
            hash = hash * 23 + this.Minor.GetHashCode();
            hash = hash * 23 + this.MinorPre.GetHashCode();
            hash = hash * 23 + this.Build.GetHashCode();
            hash = hash * 23 + this.BuildPre.GetHashCode();
            hash = hash * 23 + this.Revision.GetHashCode();
            hash = hash * 23 + this.RevisionPre.GetHashCode();
            return hash;
        }
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
        if (fieldCount >= 2 && this.Minor >= 0)
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

    public static bool operator ==(SemanticVersion? left, SemanticVersion? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }
        if (left is null || right is null)
        {
            return false;
        }
        return left.Equals(right);
    }

    public static bool operator !=(SemanticVersion? left, SemanticVersion? right)
    {
        return !(left == right);
    }

    public static bool operator <(SemanticVersion? left, SemanticVersion? right)
    {
        return right != null && left?.CompareTo(right) < 0;
    }

    public static bool operator >(SemanticVersion? left, SemanticVersion? right)
    {
        return right != null && left?.CompareTo(right) > 0;
    }

    public static bool operator <=(SemanticVersion? left, SemanticVersion? right)
    {
        return right != null && left?.CompareTo(right) <= 0;
    }

    public static bool operator >=(SemanticVersion? left, SemanticVersion? right)
    {
        return right != null && left?.CompareTo(right) >= 0;
    }

    [Flags]
    public enum Compare
    {
        Major,
        Minor,
        Build,
        Revision,
        MajorAndMinor = Major | Minor,
        MajorAndMinorAndBuild = MajorAndMinor | Build,
        MajorAndMinorAndBuildAndRevision = MajorAndMinorAndBuild | Revision,
        All = MajorAndMinorAndBuildAndRevision
    }
}
