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

        public static List<string> FrameworkFolders { get; } = new List<string>(new[] { "netstandard*", "netcore*", "net5*", "net4*" });
        public static List<string> NugetFolders { get; } = new List<string>(new[] { "lib", "ref" });

        private readonly List<string> resolvedAssemblies = new List<string>();

        public List<SearchLocation> Locations { get; }
        public bool SkipResourceAssemblies { get; set; }
        public bool SkipSystemAssemblies { get; set; }
        public bool DoNotTryToLoadDependenciesOnError { get; set; }

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

        public NugetAssemblyLocator AddLocation(int index, Assembly requestingAssembly)
        {
            return this.AddLocation(index, new SearchLocation(requestingAssembly?.Location).SearchOnlyLocal());
        }

        public Assembly Locate(string search, bool loadDependencies)
        {
            return this.Locate(search, null, loadDependencies);
        }

        public Assembly Locate(string search, Version defaultVersion = null, bool loadDependencies = false, bool forceSearchOnDisk = false)
        {
            AssemblyInfo info = (this.GetAssemblyInfoFromLongName(search) ?? this.GetAssemblyInfoFromPath(search, defaultVersion)) ?? new AssemblyInfo(search, defaultVersion);
            if (this.SkipResourceAssemblies && info.IsResource || this.SkipSystemAssemblies && info.Name.StartsWith("System.") || info.Name == "netstandard" || info.Name == "mscorlib")
            {
                return null;
            }
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == info.Name);
            if (assembly != null)
            {
                if (loadDependencies)
                {
                    this.ResolveDependencies(assembly);
                }
                return assembly;
            }
            if (info.Path != null && info.Path.Contains(":"))
            {
                Logger.Trace($"Try to load assembly from path {info}...");
                return this.TryFind(info, info.Path);
            }
            if (!forceSearchOnDisk && info.Name.StartsWith("System."))
            {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(search));
                    if (assembly != null)
                    {
                        if (loadDependencies)
                        {
                            this.ResolveDependencies(assembly);
                        }
                        return assembly;
                    }
            }
            Logger.Trace($"Try to find assembly {info}...");
            List<SearchLocation> locations = this.CleanLocations(this.Locations);
            foreach (SearchLocation location in locations)
            {
                assembly = (location.SearchLocal ? this.TryFind(info, location.Path, info.Path) : null)
                           ?? (location.SearchBin ? this.TryFind(info, location.Path, "bin", info.Path) : null)
                           ?? (location.SearchBinDebug ? this.TryFindExtended(info, location.Path, "bin", "debug") : null)
                           ?? (location.SearchBinRelease ? this.TryFindExtended(info, location.Path, "bin", "release") : null);
                if (assembly != null)
                {
                    if (loadDependencies)
                    {
                        this.ResolveDependencies(assembly);
                    }
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
                          .Select(x => new PossibleLocation(x.FullName, this.Parse(Regex.Replace(x.Name, Regex.Escape(info.Name), string.Empty, RegexOptions.IgnoreCase).Trim("."))))
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
                foreach (string nugetFolder in NugetFolders)
                {
                    foreach (string frameworkFolder in FrameworkFolders)
                    {
                        assembly = FileSystem.GetDirectoryInfos(FileSystem.Combine(possibleLocation.Path, nugetFolder), frameworkFolder)
                                             .OrderByDescending(x => x.Name)
                                             .Select(x => this.TryFind(info, x.FullName, info.Name))
                                             .FirstOrDefault();
                        if (assembly != null)
                        {
                            if (loadDependencies)
                            {
                                this.ResolveDependencies(assembly);
                            }
                            return assembly;
                        }
                    }
                }
            }
            Logger.Warning($"Assembly {info.Name} not found");
            return null;
        }

        public void ResolveDependencies(Assembly assembly)
        {
            if (this.resolvedAssemblies.Contains(assembly.GetName().Name))
            {
                return;
            }
            this.resolvedAssemblies.Add(assembly.GetName().Name);
            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                if (this.resolvedAssemblies.Contains(reference.Name))
                {
                    continue;
                }
                this.resolvedAssemblies.Add(reference.Name);
                this.Locate(reference.Name, true);
            }
        }

        private Assembly TryFindExtended(AssemblyInfo info, params string[] chunks)
        {
            string path = FileSystem.Combine(FileSystem.Combine(chunks));
            if (FileSystem.DirectoryExists(path))
            {
                DirectoryInfo[] directories = FileSystem.GetDirectoryInfos(path, "netcoreapp*").Concat(FileSystem.GetDirectoryInfos(path, "netstandard*")).ToArray();
                return directories.Select(directory => this.TryFind(info, directory.FullName, info.Name)).FirstOrDefault()
                       ?? this.TryFind(info, path, info.Name);
            }
            List<string> list = chunks.ToList();
            list.Add(info.Name);
            return this.TryFind(info, list.ToArray());
        }

        private Assembly TryFind(AssemblyInfo info, params string[] chunks)
        {
            string file = FileSystem.Combine(chunks);
            file = info.IsExecutable ? file.TrimEnd(".exe") + ".exe" : info.IsResource ? file.TrimEnd(".resources") + ".resources" : file.TrimEnd(".dll") + ".dll";
            if (FileSystem.FileExists(file))
            {
                Logger.Trace($"Assembly found in: {file}");
                try
                {
                    return AssemblyLoadContext.Default?.LoadFromAssemblyPath(file);
                }
                catch (TargetInvocationException)
                {
                    if (DoNotTryToLoadDependenciesOnError)
                    {
                        throw;
                    }
                    Logger.Trace("Could not load assembly. Trying to load dependencies first...");
                    Assembly assembly = Assembly.LoadFile(file);
                    this.ResolveDependencies(assembly);
                    Logger.Trace($"All dependencies loaded. Clean up and try to load {info.Name} again...");
                    AssemblyLoadContext.GetLoadContext(assembly)?.Unload();
                    return AssemblyLoadContext.Default?.LoadFromAssemblyPath(file);
                }
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
            Regex regex = new Regex(@"^(?<name>[^,]+)(,\sVersion=(?<version>[\d.]+))?(,\sCulture=(?<culture>[\w-]+))?(,\sPublicKeyToken=(?<token>\w+))?$");
            Match match = regex.Match(name);
            if (match.Success)
            {
                return new AssemblyInfo(match.Groups["name"].Value, string.IsNullOrEmpty(match.Groups["version"].Value) ? null : new Version(match.Groups["version"].Value));
            }
            return null;
        }

        private Version Parse(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return null;
            }
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