using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using KY.Core.DataAccess;
using KY.Core.Nuget;

namespace KY.Core
{
    public static class NugetPackageDependencyLoader
    {
        public static readonly string WindowsNugetCachePath = FileSystem.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), ".nuget", "packages");
        public static readonly string WindowsNugetFallbackPath = FileSystem.Combine(Environment.ExpandEnvironmentVariables("%PROGRAMFILES%"), "dotnet", "sdk", "NuGetFallbackFolder");
        public static readonly string LinuxNugetCachePath = FileSystem.Combine(Environment.GetEnvironmentVariable("HOME"), ".nuget", "packages");
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
                Locations.Add(new SearchLocation(WindowsNugetCachePath).SearchOnlyByVersion());
                Locations.Add(new SearchLocation(WindowsNugetFallbackPath).SearchOnlyByVersion());
                Locations.Add(new SearchLocation(WindowsNugetFallbackPath).SearchOnlyLocal());
            }
            else
            {
                Locations.Add(new SearchLocation(LinuxNugetCachePath).SearchOnlyByVersion());
            }
        }

        public static void Activate(IAssemblyCache assemblyCache = null)
        {
            cache = assemblyCache ?? cache;
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
            string assemblyPath = cache?.Resolve(args.Name);
            if (assemblyPath != null && FileSystem.FileExists(assemblyPath))
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                Assembly cachedAssembly = AssemblyLoadContext.Default?.LoadFromAssemblyPath(assemblyPath);
                stopwatch.Stop();
                Logger.Trace($"Assembly {args.Name.Split(',').FirstOrDefault()} loaded in {(stopwatch.ElapsedMilliseconds >= 1 ? stopwatch.ElapsedMilliseconds.ToString() : "<1")} ms");
                return cachedAssembly;
            }
            Assembly assembly = CreateLocator()
                                .AddLocation(0, args.RequestingAssembly)
                                .Locate(args.Name, null, false, true);
            cache?.Add(args.Name, assembly.Location);
            return assembly;
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
