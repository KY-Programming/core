using System;
using System.Collections.Generic;
using System.Linq;

namespace KY.Core;

public static class SemanticVersionExtension
{
    public static SemanticVersion? Closest(this IEnumerable<SemanticVersion> versions, string version)
    {
        return versions.Closest(new SemanticVersion(version));
    }

    public static SemanticVersion? Closest(this IEnumerable<SemanticVersion> versions, Version version)
    {
        return versions.Closest(new SemanticVersion(version));
    }

    public static SemanticVersion? Closest(this IEnumerable<SemanticVersion> versions, SemanticVersion version)
    {
        List<SemanticVersion> list = versions.ToList();
        list.Sort();
        list.Reverse();
        return list.FirstOrDefault(x => x.Equals(version))
               ?? list.FirstOrDefault(x => IsSameVersion(x, version))
               ?? list.FirstOrDefault(x => x.Equals(version, SemanticVersion.Compare.MajorAndMinor))
               ?? list.FirstOrDefault(x => x.Equals(version, SemanticVersion.Compare.Major))
               ?? list.ClosestOlder(version)
               ?? list.Newest();
    }

    /// <summary>
    /// The two sides of a lookup rarely carry the same number of parts - a NuGet package folder is "10.0.0" while the
    /// assembly version asking for it is "10.0.0.0". Both name the same version, but they are not <see cref="SemanticVersion.Equals(SemanticVersion)" />,
    /// because an unspecified part is -1 and a written zero is 0. Comparing only the parts both sides specify closes
    /// that gap, without letting "10.0.1-preview.13" pass as "10.0.0".
    /// </summary>
    private static bool IsSameVersion(SemanticVersion left, SemanticVersion right)
    {
        SemanticVersion.Compare compare = SemanticVersion.Compare.MajorAndMinor;
        if (left.Build >= 0 && right.Build >= 0)
        {
            compare |= SemanticVersion.Compare.Build;
        }
        if (left.Revision >= 0 && right.Revision >= 0)
        {
            compare |= SemanticVersion.Compare.Revision;
        }
        return left.Equals(right, compare);
    }

    public static SemanticVersion? ClosestOlder(this IEnumerable<SemanticVersion> versions, string version)
    {
        return versions.ClosestOlder(new SemanticVersion(version));
    }

    public static SemanticVersion? ClosestOlder(this IEnumerable<SemanticVersion> versions, Version version)
    {
        return versions.ClosestOlder(new SemanticVersion(version));
    }

    public static SemanticVersion? ClosestOlder(this IEnumerable<SemanticVersion> versions, SemanticVersion version)
    {
        SemanticVersion? closest = null;
        foreach (SemanticVersion current in versions)
        {
            if (current <= version && (current > closest || closest == null))
            {
                closest = current;
            }
        }
        return closest;
    }

    public static SemanticVersion? ClosestNewer(this IEnumerable<SemanticVersion> versions, string version)
    {
        return versions.ClosestNewer(new SemanticVersion(version));
    }

    public static SemanticVersion? ClosestNewer(this IEnumerable<SemanticVersion> versions, Version version)
    {
        return versions.ClosestNewer(new SemanticVersion(version));
    }

    public static SemanticVersion? ClosestNewer(this IEnumerable<SemanticVersion> versions, SemanticVersion version)
    {
        SemanticVersion? closest = null;
        foreach (SemanticVersion current in versions)
        {
            if (current >= version && (current < closest || closest == null))
            {
                closest = current;
            }
        }
        return closest;
    }

    public static SemanticVersion? Newest(this IEnumerable<SemanticVersion> versions)
    {
        SemanticVersion? newest = null;
        foreach (SemanticVersion current in versions)
        {
            if (newest == null || current > newest)
            {
                newest = current;
            }
        }
        return newest;
    }
}
