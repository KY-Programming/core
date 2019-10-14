using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using KY.Core.DataAccess;

namespace KY.Core
{
    public static class NugetPackageDependencyLoader
    {
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
            if (args.Name.EndsWith(".resources"))
            {
                return null;
            }
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName == args.Name);
            if (assembly != null)
            {
                return assembly;
            }
            Logger.Trace($"Try to find {args.Name}");
            Regex regex = new Regex(@"(?<name>[\w.]+),\sVersion=(?<version>[\d.]+),\sCulture=(?<culture>[\w-]+),\sPublicKeyToken=(?<token>\w+)");
            Match match = regex.Match(args.Name);
            if (match.Success)
            {
                string name = match.Groups["name"].Value;
                if (name.StartsWith("System."))
                {
                    return null;
                }
                List<DirectoryInfo> versions = FileSystem.GetDirectoryInfos(FileSystem.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), ".nuget\\packages", name)).ToList();
                FileSystem.GetDirectoryInfos(FileSystem.Combine(Environment.ExpandEnvironmentVariables("%PROGRAMFILES%"), "dotnet\\sdk\\NuGetFallbackFolder", name)).ForEach(versions.Add);
                Version version = new Version(match.Groups["version"].Value);
                DirectoryInfo versionDirectory = versions.FirstOrDefault(x => x.Name == version.ToString())
                                                 ?? versions.FirstOrDefault(x => x.Name.StartsWith(version.ToString(3)))
                                                 ?? versions.FirstOrDefault(x => x.Name.StartsWith(version.ToString(2)))
                                                 ?? versions.FirstOrDefault(x => x.Name.StartsWith(version.ToString(1)));
                if (versionDirectory != null)
                {
                    string fullPath = FileSystem.Combine(versionDirectory.FullName, "lib", "netstandard2.0", name) + ".dll";
                    return Assembly.LoadFile(fullPath);
                }
            }
            return null;
        }
    }
}