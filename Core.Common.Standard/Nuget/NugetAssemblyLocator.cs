using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using KY.Core.DataAccess;
using KY.Core.Nuget;

namespace KY.Core
{
    public class NugetAssemblyLocator
    {
        private const string PackageDirectoryName = "\\packages\\";

        public List<SearchLocation> Locations { get; }
        public bool SkipResourceAssemblies { get; set; }
        public bool SkipSystemAssemblies { get; set; }

        public NugetAssemblyLocator(IEnumerable<SearchLocation> locations)
        {
            this.Locations = new List<SearchLocation>(locations);
            this.SkipResourceAssemblies = true;
        }

        public NugetAssemblyLocator AddLocation(SearchLocation location)
        {
            return this.AddLocation(this.Locations.Count, location);
        }

        public NugetAssemblyLocator AddLocation(int index, SearchLocation location)
        {
            this.Locations.Insert(index, location);
            return this;
        }

        public Assembly Locate(string search, Version defaultVersion = null)
        {
            AssemblyInfo info = (this.GetAssemblyInfoFromLongName(search) ?? this.GetAssemblyInfoFromPath(search, defaultVersion)) ?? new AssemblyInfo(search, defaultVersion);
            if (this.SkipResourceAssemblies && info.IsResource || this.SkipSystemAssemblies && info.Name.StartsWith("System."))
            {
                return null;
            }
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == info.Name);
            if (assembly != null)
            {
                return assembly;
            }
            Logger.Trace($"Try to find assembly {info.Name}-{info.Version}...");
            List<SearchLocation> locations = this.CleanLocations(this.Locations);
            foreach (SearchLocation location in locations)
            {
                assembly = (location.SearchLocal ? this.TryFind(info, location.Path, info.Path) : null)
                           ?? (location.SearchBin ? this.TryFind(info, location.Path, "bin", info.Path) : null)
                           ?? (location.SearchBinDebug ? this.TryFind(info, location.Path, "bin", "debug", info.Path) : null)
                           ?? (location.SearchBinRelease ? this.TryFind(info, location.Path, "bin", "release", info.Path) : null);
                if (assembly != null)
                {
                    return assembly;
                }
            }
            string entryLocation = Assembly.GetEntryAssembly()?.Location;
            if (entryLocation != null && entryLocation.Contains(PackageDirectoryName))
            {
                string path = FileSystem.Combine(entryLocation.Split(new[] { PackageDirectoryName }, StringSplitOptions.RemoveEmptyEntries).First(), PackageDirectoryName);
                locations.Insert(0, new SearchLocation(path));
            }
            List<PossibleLocation> possibleLocations = new List<PossibleLocation>();
            foreach (SearchLocation location in locations.Where(x => x.SearchByVersion))
            {
                DirectoryInfo packageDirectory = FileSystem.GetDirectoryInfo(location.Path, info.Name);
                if (packageDirectory.Exists)
                {
                    packageDirectory.GetDirectories()
                                    .Select(x => new PossibleLocation(x.FullName, this.Parse(x.Name)))
                                    .Where(x => x.Version != null)
                                    .ForEach(possibleLocations.Add);
                }
                FileSystem.GetDirectoryInfos(location.Path, info.Name + "*")
                          .Select(x => new PossibleLocation(x.FullName, this.Parse(x.Name.Remove(info.Name).Trim("."))))
                          .Where(x => x.Version != null)
                          .ForEach(possibleLocations.Add);
            }
            possibleLocations = possibleLocations.OrderByDescending(x => x.Version).ToList();
            possibleLocations.ForEach(x => Logger.Trace($"Assembly found in: {x.Path}"));
            PossibleLocation possibleLocation = possibleLocations.FirstOrDefault(x => info.Version == null || x.Version.ToString() == info.Version.ToString())
                                                ?? possibleLocations.FirstOrDefault(x => x.Version.ToString(3) == info.Version.ToString(3))
                                                ?? possibleLocations.FirstOrDefault(x => x.Version.ToString(2) == info.Version.ToString(2))
                                                ?? possibleLocations.FirstOrDefault(x => x.Version.ToString(1) == info.Version.ToString(1))
                                                ?? possibleLocations.FirstOrDefault();
            if (possibleLocation != null)
            {
                Logger.Trace($"Best matching version found in: {possibleLocation.Path}");
                Logger.Trace("To specify a exact version, write assembly name like this: \"MyAssembly.dll, Version=1.2.3.0\"");
                DirectoryInfo assemblyDirectory = FileSystem.GetDirectoryInfos(FileSystem.Combine(possibleLocation.Path, "lib"), "netstandard").OrderByDescending(x => x.Name).FirstOrDefault()
                                                  ?? FileSystem.GetDirectoryInfos(FileSystem.Combine(possibleLocation.Path, "lib")).OrderByDescending(x => x.Name).FirstOrDefault();
                assembly = assemblyDirectory == null ? null : this.TryFind(info, assemblyDirectory.FullName, info.Name);
            }
            if (assembly != null)
            {
                return assembly;
            }
            Logger.Warning($"Assembly {info.Name} not found");
            return null;
        }

        private Assembly TryFind(AssemblyInfo info, params string[] chunks)
        {
            string file = FileSystem.Combine(chunks);
            file = info.IsExecutable ? file.TrimEnd(".exe") + ".exe" : info.IsResource ? file.TrimEnd(".resources") + ".resources" : file.TrimEnd(".dll") + ".dll";
            if (FileSystem.FileExists(file))
            {
                Logger.Trace($"Assembly found in: {file}");
                return Assembly.LoadFrom(file);
            }
            Logger.Trace($"Assembly searched in: {file}");
            return null;
        }

        private AssemblyInfo GetAssemblyInfoFromPath(string path, Version defaultVersion)
        {
            Regex regex = new Regex(@"(?<name>[a-zA-Z0-9-._\s]+)\.(dll|exe)$");
            Match match = regex.Match(path);
            if (match.Success)
            {
                return new AssemblyInfo(match.Groups["name"].Value, defaultVersion, path);
            }
            return null;
        }

        private AssemblyInfo GetAssemblyInfoFromLongName(string name)
        {
            Regex regex = new Regex(@"(?<name>[\w.]+),\sVersion=(?<version>[\d.]+)(,\sCulture=(?<culture>[\w-]+),\sPublicKeyToken=(?<token>\w+))?");
            Match match = regex.Match(name);
            if (match.Success)
            {
                return new AssemblyInfo(match.Groups["name"].Value, new Version(match.Groups["version"].Value));
            }
            return null;
        }

        private Version Parse(string version)
        {
            try
            {
                return new Version(version);
            }
            catch
            {
                return null;
            }
        }

        private List<SearchLocation> CleanLocations(List<SearchLocation> input)
        {
            List<SearchLocation> output = new List<SearchLocation>();
            foreach (SearchLocation location in input)
            {
                if (location?.Path == null)
                {
                    continue;
                }
                if (FileSystem.FileExists(location.Path))
                {
                    location.Path = FileSystem.Parent(location.Path);
                }
                if (string.IsNullOrEmpty(location.Path) || output.Any(x => x.Path.Equals(location.Path, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }
                output.Add(location);
            }
            return output;
        }
    }
}