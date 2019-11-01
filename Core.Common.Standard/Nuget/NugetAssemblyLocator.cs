using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using KY.Core.DataAccess;

namespace KY.Core
{
    public class NugetAssemblyLocator
    {
        private const string PackageDirectoryName = "\\packages\\";

        public List<string> Locations { get; }
        public bool SkipResourceAssemblies { get; set; }
        public bool SkipSystemAssemblies { get; set; }

        public NugetAssemblyLocator(IEnumerable<string> locations)
        {
            this.Locations = new List<string>(locations);
            this.SkipResourceAssemblies = true;
        }

        public Assembly Locate(string search, Version defaultVersion = null)
        {
            AssemblyInfo info = (this.GetAssemblyInfoFromLongName(search) ?? this.GetAssemblyInfoFromPath(search, defaultVersion)) ?? new AssemblyInfo(search, defaultVersion);
            if (this.SkipResourceAssemblies && info.Name.EndsWith(".resources") || this.SkipSystemAssemblies && info.Name.StartsWith("System."))
            {
                return null;
            }
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == info.Name);
            if (assembly != null)
            {
                return assembly;
            }
            Logger.Trace($"Try to find assembly {info.Name}...");
            List<string> locations = this.CleanLocations(this.Locations);
            foreach (string location in locations)
            {
                assembly = this.TryFind(location, info.Name)
                           ?? this.TryFind(location, info.Name)
                           ?? this.TryFind(location, "bin", info.Name)
                           ?? this.TryFind(location, "bin", "debug", info.Name)
                           ?? this.TryFind(location, "bin", "release", info.Name);
                if (assembly != null)
                {
                    return assembly;
                }
            }
            string entryLocation = Assembly.GetEntryAssembly()?.Location;
            if (entryLocation != null && entryLocation.Contains(PackageDirectoryName))
            {
                locations.Insert(0, FileSystem.Combine(entryLocation.Split(new[] { PackageDirectoryName }, StringSplitOptions.RemoveEmptyEntries).First(), PackageDirectoryName));
            }
            List<PossibleLocation> possibleLocations = new List<PossibleLocation>();
            foreach (string location in locations)
            {
                DirectoryInfo packageDirectory = FileSystem.GetDirectoryInfo(location, info.Name);
                if (packageDirectory.Exists)
                {
                    packageDirectory.GetDirectories()
                                    .Select(x => new PossibleLocation(x.FullName, this.Parse(x.Name)))
                                    .Where(x => x.Version != null)
                                    .ForEach(possibleLocations.Add);
                }
                FileSystem.GetDirectoryInfos(location, info.Name + "*")
                          .Select(x => new PossibleLocation(x.FullName, this.Parse(x.Name.Remove(info.Name).Trim("."))))
                          .Where(x => x.Version != null)
                          .ForEach(possibleLocations.Add);
            }
            possibleLocations = possibleLocations.OrderByDescending(x => x.Version).ToList();
            PossibleLocation possibleLocation = possibleLocations.FirstOrDefault(x => info.Version == null || x.Version.ToString() == info.Version.ToString())
                                                ?? possibleLocations.FirstOrDefault(x => x.Version.ToString(3) == info.Version.ToString(3))
                                                ?? possibleLocations.FirstOrDefault(x => x.Version.ToString(2) == info.Version.ToString(2))
                                                ?? possibleLocations.FirstOrDefault(x => x.Version.ToString(1) == info.Version.ToString(1))
                                                ?? possibleLocations.FirstOrDefault();
            if (possibleLocation != null)
            {
                DirectoryInfo assemblyDirectory = FileSystem.GetDirectoryInfos(FileSystem.Combine(possibleLocation.Path, "lib"), "netstandard").OrderByDescending(x => x.Name).FirstOrDefault()
                                                  ?? FileSystem.GetDirectoryInfos(FileSystem.Combine(possibleLocation.Path, "lib")).OrderByDescending(x => x.Name).FirstOrDefault();
                assembly = assemblyDirectory == null ? null : this.TryFind(assemblyDirectory.FullName, info.Name);
            }
            if (assembly != null)
            {
                return assembly;
            }
            Logger.Trace($"Assembly {info.Name} not found. Searched in: {string.Join("", locations.Select(x => $"{Environment.NewLine}  - {x}"))}");
            return null;
        }

        private Assembly TryFind(params string[] chunks)
        {
            string file = FileSystem.Combine(chunks).TrimEnd(".dll") + ".dll";
            return FileSystem.FileExists(file) ? this.Found(file) : null;
        }

        private Assembly Found(string fullPath)
        {
            Logger.Trace($"Assembly found in: {fullPath}");
            return Assembly.LoadFrom(fullPath);
        }

        private AssemblyInfo GetAssemblyInfoFromPath(string path, Version defaultVersion)
        {
            Regex regex = new Regex(@"(?<name>[a-zA-Z0-9-._\s]+)\.(dll|exe)$");
            Match match = regex.Match(path);
            if (match.Success)
            {
                return new AssemblyInfo(match.Groups["name"].Value, defaultVersion);
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

        private List<string> CleanLocations(List<string> input)
        {
            List<string> output = new List<string>();
            foreach (string path in input)
            {
                if (path == null)
                {
                    continue;
                }
                if (FileSystem.FileExists(path))
                {
                    output.Add(FileSystem.Parent(path));
                    continue;
                }
                if (string.IsNullOrEmpty(path) || output.Contains(path))
                {
                    continue;
                }
                output.Add(path);
            }
            return output;
        }
    }
}