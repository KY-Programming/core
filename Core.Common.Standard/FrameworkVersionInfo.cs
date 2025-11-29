using System;

namespace KY.Core;

public class FrameworkVersionInfo
{
    public FrameworkType Type { get; set; }
    public string Name { get; set; }
    public int MinMajor { get; set; }
    public int MaxMajor { get; set; }

    public FrameworkVersionInfo(FrameworkType type, string name, int minMajor = 0, int maxMajor = int.MaxValue)
    {
        this.Type = type;
        this.Name = name;
        this.MinMajor = minMajor;
        this.MaxMajor = maxMajor;
    }

    public bool Match(string frameworkName, DotNetVersion version)
    {
        return this.Name.Equals(frameworkName, StringComparison.OrdinalIgnoreCase) && version.Major >= this.MinMajor && version.Major <= this.MaxMajor;
    }
}