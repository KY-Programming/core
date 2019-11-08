using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using KY.Core.DataAccess;
using KY.Core.Nuget;

namespace KY.Core
{
    public static class NugetPackageDependencyLoader
    {
        public static List<SearchLocation> Locations { get; }

        static NugetPackageDependencyLoader()
        {
            Locations = new List<SearchLocation>
                        {
                            new SearchLocation(Environment.CurrentDirectory),
                            new SearchLocation(Assembly.GetCallingAssembly().Location).SearchOnlyLocal(),
                            new SearchLocation(Assembly.GetEntryAssembly()?.Location).SearchOnlyLocal(),
                            new SearchLocation(FileSystem.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), ".nuget\\packages")).SearchOnlyByVersion(),
                            new SearchLocation(FileSystem.Combine(Environment.ExpandEnvironmentVariables("%PROGRAMFILES%"), "dotnet\\sdk\\NuGetFallbackFolder")).SearchOnlyByVersion()
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
            return CreateLocator()
                   .AddLocation(0, new SearchLocation(args.RequestingAssembly?.Location).SearchOnlyLocal())
                   .Locate(args.Name);
        }

        public static NugetAssemblyLocator CreateLocator()
        {
            return new NugetAssemblyLocator(Locations);
        }
    }
}