using System;
using System.IO;
using System.Text.RegularExpressions;

// ReSharper disable UseFileSystem

namespace KY.Core.DataAccess
{
    internal class DirectoryHelper
    {
        private readonly PathHelper pathHelper;
        private static readonly Regex nameRegex = new Regex(@"[^A-z0-9!#$%&'()+,-.;=@[\]^_`{}~\s]");

        public DirectoryHelper(PathHelper pathHelper)
        {
            this.pathHelper = pathHelper;
        }

        public string[] GetDirectories(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            path = this.pathHelper.ToAbsolute(path);
            return searchPattern == null ? Directory.GetDirectories(path) : Directory.GetDirectories(path, searchPattern, option);
        }

        public DirectoryInfo GetDirectoryInfo(string[] pathChunks)
        {
            string path = this.pathHelper.ToAbsolute(pathChunks);
            return new DirectoryInfo(path);
        }

        public DirectoryInfo[] GetDirectoryInfos(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            path = this.pathHelper.ToAbsolute(path);
            DirectoryInfo directory = new DirectoryInfo(path);
            if (!directory.Exists)
            {
                return new DirectoryInfo[0];
            }
            return searchPattern == null ? directory.GetDirectories() : directory.GetDirectories(searchPattern, option);
        }

        public bool Exists(params string[] pathChunks)
        {
            string path = this.pathHelper.ToAbsolute(pathChunks);
            return Directory.Exists(path);
        }

        public void Create(params string[] pathChunks)
        {
            string path = this.pathHelper.ToAbsolute(pathChunks);
            Directory.CreateDirectory(path);
        }

        public void Move(string from, string to, bool overwrite = false)
        {
            from = this.pathHelper.ToAbsolute(from);
            to = this.pathHelper.ToAbsolute(to);
            if (overwrite && Directory.Exists(to))
            {
                Directory.Delete(to, true);
            }
            Directory.Move(from, to);
        }

        public void Delete(params string[] pathChunks)
        {
            string path = this.pathHelper.ToAbsolute(pathChunks);
            Directory.Delete(path, true);
        }

        public void Copy(string from, string to, bool overwrite = false)
        {
            from = this.pathHelper.ToAbsolute(from);
            to = this.pathHelper.ToAbsolute(to);
            this.Copy(new DirectoryInfo(from), to, overwrite);
        }

        private void Copy(DirectoryInfo from, string to, bool overwrite = false)
        {
            if (!from.Exists)
                throw new DirectoryNotFoundException($"Directory {from} not found");

            Directory.CreateDirectory(to);
            from.GetFiles().ForEach(x => x.CopyTo(Path.Combine(to, x.Name), overwrite));
            from.GetDirectories().ForEach(x => this.Copy(x, Path.Combine(to, x.Name), overwrite));
        }

        public string? ToDirectory(string? directory)
        {
            return directory == null ? null : nameRegex.Replace(directory, string.Empty).Replace("\\", "");
        }

        public string? GetName(string? directory)
        {
            return Path.GetDirectoryName(directory);
        }

        public FileSystemWatcher Watch(string path)
        {
            string absolutePath = this.pathHelper.ToAbsolute(path);
            FileSystemWatcher watchdog = new (absolutePath);
            watchdog.EnableRaisingEvents = true;
            return watchdog;
        }

        public DateTime GetLastWriteTime(params string[] pathChunks)
        {
            return Directory.GetLastWriteTime(this.pathHelper.ToAbsolute(pathChunks));
        }
    }
}
