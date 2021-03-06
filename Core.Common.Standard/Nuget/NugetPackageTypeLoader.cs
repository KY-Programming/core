﻿using System;
using System.Linq;
using System.Reflection;
using KY.Core.Nuget;

namespace KY.Core
{
    public static class NugetPackageTypeLoader
    {
        public static Type Get(string assemblyName, string nameSpace, string typeName, Version defaultVersion, params SearchLocation[] locations)
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
                return AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetType(name)).FirstOrDefault(x => x != null);
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
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed : Force a ReflectionTypeLoadException if something is wrong
                assembly.DefinedTypes.FirstOrDefault();
                Logger.Error($"Can not load type: {name} not found in {assembly.GetName().Name}");
                return null;
            }
            return type;
        }
    }
}