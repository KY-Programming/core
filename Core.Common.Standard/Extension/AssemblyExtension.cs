using System.IO;
using System.Linq;
using System.Reflection;

namespace KY.Core.Extension
{
    public static class AssemblyExtension
    {
        public static string GetManifestResourceString(this Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
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
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}