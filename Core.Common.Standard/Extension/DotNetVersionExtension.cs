using System.Collections.Generic;

namespace KY.Core;

public static class DotNetVersionExtension
{
    public static DotNetVersion? Closest(this IEnumerable<DotNetVersion> versions, DotNetVersion version)
    {
        DotNetVersion? closest = null;
        foreach (DotNetVersion current in versions)
        {
            if (current <= version && (current > closest || closest == null))
            {
                closest = current;
            }
        }
        return closest;
    }

    public static DotNetVersion? ClosestNewer(this IEnumerable<DotNetVersion> versions, DotNetVersion version)
    {
        DotNetVersion? closest = null;
        foreach (DotNetVersion current in versions)
        {
            if (current > version && (current < closest || closest == null))
            {
                closest = current;
            }
        }
        return closest;
    }

    public static DotNetVersion? Newest(this IEnumerable<DotNetVersion> versions)
    {
        DotNetVersion? newest = null;
        foreach (DotNetVersion current in versions)
        {
            if (newest == null || current > newest)
            {
                newest = current;
            }
        }
        return newest;
    }
}
