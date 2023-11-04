using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using KY.Core.DataAccess;
using KY.Core.Nuget;

namespace KY.Core
{
    public static class NugetPackageDependencyLoader
    {
        private static bool isActivated;
        private static bool isRuntimeLocationsAdded;
        private static IAssemblyCache cache;

        public static List<SearchLocation> Locations { get; }

        static NugetPackageDependencyLoader()
        {
            Locations = new List<SearchLocation>
                        {
                            new SearchLocation(Environment.CurrentDirectory),
                            new SearchLocation(Assembly.GetCallingAssembly().Location).SearchOnlyLocal(),
                            new SearchLocation(Assembly.GetEntryAssembly()?.Location).SearchOnlyLocal()
                        };
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Locations.Add(new SearchLocation(FileSystem.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), ".nuget", "packages")).SearchOnlyByVersion());
                Locations.Add(new SearchLocation(FileSystem.Combine(Environment.ExpandEnvironmentVariables("%PROGRAMFILES%"), "dotnet", "sdk", "NuGetFallbackFolder")).SearchOnlyByVersion());
                Locations.Add(new SearchLocation(FileSystem.Combine(Environment.ExpandEnvironmentVariables("%PROGRAMFILES%"), "dotnet", "sdk", "NuGetFallbackFolder")).SearchOnlyLocal());
            }
            else
            {
                Locations.Add(new SearchLocation(FileSystem.Combine(Environment.GetEnvironmentVariable("HOME"), ".nuget", "packages")).SearchOnlyByVersion());
            }
        }

        public static void Activate(IAssemblyCache assemblyCache)
        {
            if (isActivated)
            {
                return;
            }
            isActivated = true;
            cache = assemblyCache;
            AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        }

        public static void Deactivate()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
            isActivated = false;
        }

        private static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            if (cache?.Local != null && cache.Local.TryGetValue(args.Name, out string localPath) && FileSystem.FileExists(localPath))
            {
                return Assembly.Load(localPath);
            }
            if (cache?.Global != null && cache.Global.TryGetValue(args.Name, out string globalPath) && FileSystem.FileExists(globalPath))
            {
                return Assembly.Load(globalPath);
            }
            return CreateLocator()
                   .AddLocation(0, args.RequestingAssembly)
                   .Locate(args.Name, null, false, true);
        }

        public static NugetAssemblyLocator CreateLocator()
        {
            RefreshRuntimeLocations();
            return new NugetAssemblyLocator(Locations);
        }

        private static void RefreshRuntimeLocations()
        {
            if (isRuntimeLocationsAdded)
            {
                return;
            }
            isRuntimeLocationsAdded = true;
            Locations.AddRange(InstalledRuntime.GetCurrent().Select(x => new SearchLocation(x.FullPath).SearchOnlyLocal()));
        }

        public static void Resolve(string assembly, Version version = null)
        {
            CreateLocator().Locate(assembly, version);
        }

        public static void ResolveDependencies(Assembly assembly)
        {
            CreateLocator().AddLocation(0, assembly).ResolveDependencies(assembly);
        }
    }
}
