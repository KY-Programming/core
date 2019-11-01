using System;
using System.Linq;
using System.Reflection;

namespace KY.Core
{
    public static class NugetPackageTypeLoader
    {
        public static Type Get(string assemblyName, string nameSpace, string typeName, Version defaultVersion, params string[] locations)
        {
            if (string.IsNullOrEmpty(nameSpace))
            {
                Logger.Error("Can not load type: Namespace can not be empty");
                return null;
            }
            if (string.IsNullOrEmpty(typeName))
            {
                Logger.Error("Can not load type: Name can not be empty");
                return null;
            }
            string name = $"{nameSpace}.{typeName}";
            if (string.IsNullOrEmpty(assemblyName))
            {
                return AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetType(name)).First(x => x != null);
            }
            NugetAssemblyLocator locator = NugetPackageDependencyLoader.CreateLocator();
            locator.Locations.InsertRange(0, locations);
            Assembly assembly = locator.Locate(assemblyName, defaultVersion);
            if (assembly == null)
            {
                Logger.Error($"Can not load type: Assembly {assemblyName} not found");
                return null;
            }
            Type type = assembly.GetType(name);
            if (type == null)
            {
                Logger.Error($"Can not load type: {name} not found in {assembly.GetName().Name}");
                return null;
            }
            return type;
        }
    }
}