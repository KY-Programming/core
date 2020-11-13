using System.Runtime.Versioning;

namespace KY.Core.Extension
{
    public static class FrameworkNameExtension
    {
        public static bool IsStandard(this FrameworkName framework)
        {
            return framework?.Identifier == ".NETStandard";
        }

        public static bool IsFramework(this FrameworkName framework)
        {
            return framework?.Identifier == ".NETFramework";
        }

        public static bool IsCore(this FrameworkName framework)
        {
            return framework?.Identifier == ".NETCoreApp";
        }
    }
}