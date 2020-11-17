using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using KY.Core.DataAccess;

namespace KY.Core
{
    public class InstalledRuntime
    {
        public string Type { get; }
        public Version Version { get; }
        public string Path { get; }
        public string FullPath => FileSystem.Combine(this.Path, this.Version.ToString());

        public InstalledRuntime(string type, Version version, string path)
        {
            this.Type = type;
            this.Version = version;
            this.Path = path;
        }

        public static InstalledRuntime Parse(string value)
        {
            Regex regex = new Regex(@"^(?<type>\S+)\s(?<version>\S+)\s\[(?<path>.*)]$");
            Match match = regex.Match(value);
            return match.Success ? new InstalledRuntime(match.Groups["type"].Value, new Version(match.Groups["version"].Value), match.Groups["path"].Value) : null;
        }
        
        public static InstalledRuntime[] Get()
        {
            Process process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "--list-runtimes";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Split('\n').Select(x => InstalledRuntime.Parse(x.Trim())).Where(x => x != null).ToArray();
        }

        public static InstalledRuntime[] GetCurrent()
        {
            return Get().Where(x => x.Version == Environment.Version).ToArray();
        }
    }
}