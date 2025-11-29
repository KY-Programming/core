using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace KY.Core.Extension;

public static class AssemblyExtension
{
    public static string GetManifestResourceString(this Assembly assembly, string resourceName)
    {
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream != null)
            {
                using (StreamReader reader = new(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        string alternative = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith("." + resourceName));
        if (alternative == null)
        {
            return null;
        }
        using (Stream stream = assembly.GetManifestResourceStream(alternative))
        {
            if (stream == null)
            {
                return null;
            }
            using (StreamReader reader = new(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public static FrameworkName GetTargetFramework(this Assembly assembly)
    {
        TargetFrameworkAttribute frameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
        return new FrameworkName(frameworkAttribute.FrameworkName);
    }

    public static DotNetVersion GetDotNetVersion(this Assembly assembly)
    {
        TargetFrameworkAttribute frameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
        return DotNetVersion.FromFrameworkName(frameworkAttribute.FrameworkName);
    }
}
