using System;
using System.Collections.Generic;
using System.Reflection;
using KY.Core.DataAccess;

namespace KY.Core
{
    public static class NugetPackageDependencyLoader
    {
        public static List<string> Locations { get; }

        static NugetPackageDependencyLoader()
        {
            Locations = new List<string>
                        {
                            Assembly.GetCallingAssembly().Location,
                            Assembly.GetEntryAssembly()?.Location,
                            FileSystem.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), ".nuget\\packages"),
                            FileSystem.Combine(Environment.ExpandEnvironmentVariables("%PROGRAMFILES%"), "dotnet\\sdk\\NuGetFallbackFolder")
                        };
        }

        public static void Activate()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        }

        public static void Deactivate()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
        }

        private static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            return CreateLocator().Locate(args.Name);
        }

        public static NugetAssemblyLocator CreateLocator()
        {
            return new NugetAssemblyLocator(Locations);
        }
    }
}