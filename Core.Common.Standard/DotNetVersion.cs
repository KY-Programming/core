using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace KY.Core;

public class DotNetVersion : IComparable<DotNetVersion>, IEquatable<DotNetVersion>
{
    private static readonly Regex directoryRegex = new(@"^net((?<majorOld>\d)(?<minorOld>\d)(?<buildOld>\d)?|(?<framework>.*?)(?<major>\d+)\.(?<minor>\d+))(-(?<platform>.*))?$", RegexOptions.Compiled);
    private static readonly Regex frameworkRegex = new(@"^\.net(?<framework>.*?),version=v(?<major>\d+)\.(?<minor>\d+)(\.(?<build>\d+))?$", RegexOptions.Compiled);

    private static List<FrameworkVersionInfo> frameworkVersions =
    [
        new(FrameworkType.DotNetFramework, "framework"),
        new(FrameworkType.DotNetStandard, "standard", 1, 1),
        new(FrameworkType.DotNetCore, "coreapp", 1, 3),
        new(FrameworkType.DotNetCore, "Microsoft.AspNetCore.App", 1, 3),
        new(FrameworkType.DotNetCore, "Microsoft.NETCore.App", 1, 3),
        new(FrameworkType.DotNetCore, "Microsoft.WindowsDesktop.App", 1, 3),
        new(FrameworkType.DotNetStandard, "standard", 2, 2),
        new(FrameworkType.DotNet, "coreapp", 5),
        new(FrameworkType.DotNet, "Microsoft.AspNetCore.App", 5),
        new(FrameworkType.DotNet, "Microsoft.NETCore.App", 5),
        new(FrameworkType.DotNet, "Microsoft.WindowsDesktop.App", 5),
        new(FrameworkType.DotNet, "") // >= .NET 5
    ];

    public string Framework { get; private set; } = string.Empty;
    public FrameworkType FrameworkType { get; private set; }
    public int Major { get; private set; }
    public int Minor { get; private set; }
    public int Build { get; private set; }
    public string? Platform { get; private set; }

    protected DotNetVersion()
    { }

    public DotNetVersion(FrameworkType frameworkType, int major, int minor = 0, int build = 0, string? platform = null)
    {
        this.Framework = Enum.GetName(typeof(FrameworkType), frameworkType)?.TrimStart("DotNet") ?? string.Empty;
        this.FrameworkType = frameworkType;
        this.Major = major;
        this.Minor = minor;
        this.Build = build;
        this.Platform = platform;
    }

    public DotNetVersion(string framework, int major, int minor = 0, int build = 0, string? platform = null)
    {
        this.Framework = framework;
        this.Major = major;
        this.Minor = minor;
        this.Build = build;
        this.Platform = platform;
        this.FrameworkType = GetFrameworkType(this);
    }

    public static DotNetVersion FromRuntime(InstalledRuntime runtime)
    {
        DotNetVersion version = new(runtime.Type, runtime.Version.Major, runtime.Version.Minor);
        version.FrameworkType = GetFrameworkType(version);
        return version;
    }

    public static DotNetVersion? FromDirectory(DirectoryInfo directoryInfo)
    {
        return FromDirectoryName(directoryInfo.Name);
    }

    public static DotNetVersion? FromDirectoryName(string directoryName)
    {
        Match match = directoryRegex.Match(directoryName.ToLowerInvariant());
        if (!match.Success)
        {
            return null;
        }
        DotNetVersion version = new();
        if (match.Groups["majorOld"].Success)
        {
            version.Framework = "framework";
            version.Major = int.Parse(match.Groups["majorOld"].Value);
            version.Minor = int.Parse(match.Groups["minorOld"].Value);
            version.Build = match.Groups["buildOld"].Success ? int.Parse(match.Groups["buildOld"].Value) : 0;
        }
        else
        {
            version.Framework = match.Groups["framework"].Value;
            version.Major = int.Parse(match.Groups["major"].Value);
            version.Minor = int.Parse(match.Groups["minor"].Value);
        }
        version.FrameworkType = GetFrameworkType(version);
        version.Platform = match.Groups["platform"].Success ? match.Groups["platform"].Value : null;
        return version;
    }

    public static DotNetVersion FromFrameworkName(string frameworkName)
    {
        Match match = frameworkRegex.Match(frameworkName.ToLowerInvariant());
        match.AssertIsNotNull(message: "Can not parse DotNetVersion");
        match.Success.AssertIs(true, message: "Can not parse DotNetVersion");
        DotNetVersion version = new();
        version.Framework = match.Groups["framework"].Value;
        version.Major = int.Parse(match.Groups["major"].Value);
        version.Minor = int.Parse(match.Groups["minor"].Value);
        version.Build = int.TryParse(match.Groups["build"].Value, out int build) ? build : 0;
        version.FrameworkType = GetFrameworkType(version);
        return version;
    }

    private static FrameworkType GetFrameworkType(DotNetVersion version)
    {
        return frameworkVersions.FirstOrDefault(x => x.Match(version.Framework, version))?.Type ?? FrameworkType.Unknown;
    }

    public int CompareTo(DotNetVersion other)
    {
        return this.CompareTo(other, Compare.All);
    }

    public int CompareTo(DotNetVersion other, Compare compare)
    {
        if (compare.HasFlag(Compare.Framework) && this.FrameworkType != other.FrameworkType)
        {
            return this.FrameworkType > other.FrameworkType ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Major) && this.Major != other.Major)
        {
            return this.Major > other.Major ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Minor) && this.Minor != other.Minor)
        {
            return this.Minor > other.Minor ? 1 : -1;
        }
        if (compare.HasFlag(Compare.Build) && this.Build != other.Build)
        {
            return this.Build > other.Build ? 1 : -1;
        }
        return 0;
    }

    public bool Equals(DotNetVersion? other)
    {
        return this.Equals(other, Compare.All);
    }

    public bool Equals(DotNetVersion? other, Compare compare)
    {
        if (other is null)
        {
            return false;
        }
        return this.CompareTo(other, compare) == 0;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj.GetType() != this.GetType())
        {
            return false;
        }
        return this.Equals((DotNetVersion)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = (int)this.FrameworkType;
            hashCode = hashCode * 397 ^ this.Major;
            hashCode = hashCode * 397 ^ this.Minor;
            hashCode = hashCode * 397 ^ this.Build;
            hashCode = hashCode * 397 ^ (this.Platform != null ? this.Platform.GetHashCode() : 0);
            return hashCode;
        }
    }

    public override string ToString()
    {
        if (this.FrameworkType == FrameworkType.DotNetFramework)
        {
            return $"net{this.Major}{this.Minor}{(this.Build == 0 ? "" : this.Build)}";
        }
        return $"net{this.Framework}{this.Major}.{this.Minor}{(this.Build == 0 ? "" : $".{this.Build}")}{(this.Platform is null ? "" : $"-{this.Platform}")}";
    }

    public static bool operator ==(DotNetVersion? left, DotNetVersion? right)
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

    public static bool operator !=(DotNetVersion? left, DotNetVersion? right)
    {
        return !(left == right);
    }

    public static bool operator <(DotNetVersion? left, DotNetVersion? right)
    {
        return right != null && left?.CompareTo(right) < 0;
    }

    public static bool operator >(DotNetVersion? left, DotNetVersion? right)
    {
        return right != null && left?.CompareTo(right) > 0;
    }

    public static bool operator <=(DotNetVersion? left, DotNetVersion? right)
    {
        return right != null && left?.CompareTo(right) <= 0;
    }

    public static bool operator >=(DotNetVersion? left, DotNetVersion? right)
    {
        return right != null && left?.CompareTo(right) >= 0;
    }

    [Flags]
    public enum Compare
    {
        Framework = 0,
        Major = 1,
        Minor = 2,
        Build = 4,
        FrameworkAndMajor = Framework | Major,
        FrameworkAndMajorAndMinor = FrameworkAndMajor | Minor,
        FrameworkAndMajorAndMinorAndBuild = FrameworkAndMajorAndMinor | Build,
        All = FrameworkAndMajorAndMinorAndBuild
    }
}