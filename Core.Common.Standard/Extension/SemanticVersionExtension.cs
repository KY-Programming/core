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
               ?? list.FirstOrDefault(x => x.Equals(version, SemanticVersion.Compare.MajorAndMinor))
               ?? list.FirstOrDefault(x => x.Equals(version, SemanticVersion.Compare.Major))
               ?? list.ClosestOlder(version)
               ?? list.Newest();
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
