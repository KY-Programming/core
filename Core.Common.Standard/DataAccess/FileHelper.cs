using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

// ReSharper disable UseFileSystem

namespace KY.Core.DataAccess
{
    internal class FileHelper
    {
        private static  readonly Regex sizeRegex = new Regex(@"^(?<size>[0-9.,]+)\s?(?<unit>[A-z]+)$");
        private readonly PathHelper pathHelper;

        public FileHelper(PathHelper pathHelper)
        {
            this.pathHelper = pathHelper;
        }

        public FileInfo GetInfo(string path)
        {
            path = this.pathHelper.ToAbsolute(path);
            return new FileInfo(path);
        }

        public FileInfo[] GetInfos(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            path = this.pathHelper.ToAbsolute(path);
            DirectoryInfo directory = new DirectoryInfo(path);
            return searchPattern == null ? directory.GetFiles() : directory.GetFiles(searchPattern, option);
        }

        public string[] GetFiles(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
        {
            path = this.pathHelper.ToAbsolute(path);
            return searchPattern == null ? Directory.GetFiles(path) : Directory.GetFiles(path, searchPattern, option);
        }

        public string ReadAllText(string path, Encoding encoding = null)
        {
            path = this.pathHelper.ToAbsolute(path);
            return File.ReadAllText(path, encoding ?? Encoding.UTF8);
        }

        public void WriteAllText(string path, string content, Encoding encoding = null)
        {
            path = this.pathHelper.ToAbsolute(path);
            File.WriteAllText(path, content, encoding ?? Encoding.UTF8);
        }

        public void AppendAllText(string path, string content, Encoding encoding = null)
        {
            path = this.pathHelper.ToAbsolute(path);
            File.AppendAllText(path, content, encoding ?? Encoding.UTF8);
        }

        public void WriteAllBytes(string path, byte[] content)
        {
            path = this.pathHelper.ToAbsolute(path);
            File.WriteAllBytes(path, content);
        }

        public bool Exists(params string[] pathChunks)
        {
            string path = this.pathHelper.ToAbsolute(pathChunks);
            return File.Exists(path);
        }

        public void Delete(params string[] pathChunks)
        {
            string path = this.pathHelper.ToAbsolute(pathChunks);
            File.Delete(path);
        }

        public string[] ReadAllLines(string path, Encoding encoding = null)
        {
            path = this.pathHelper.ToAbsolute(path);
            return File.ReadAllLines(path, encoding ?? Encoding.UTF8);
        }

        public XElement ReadXml(params string[] pathChunks)
        {
            string path = this.pathHelper.ToAbsolute(pathChunks);
            return XElement.Load(path);
        }

        public Stream OpenRead(params string[] pathChunks)
        {
            string path = this.pathHelper.ToAbsolute(pathChunks);
            return File.OpenRead(path);
        }

        public Stream OpenWrite(params string[] pathChunks)
        {
            string path = this.pathHelper.ToAbsolute(pathChunks);
            return File.OpenWrite(path);
        }

        public void Copy(string from, string to, bool overwrite = false)
        {
            from = this.pathHelper.ToAbsolute(from);
            to = this.pathHelper.ToAbsolute(to);
            File.Copy(from, to, overwrite);
        }

        public void Move(string path, string oldFileName, string newFileName, bool overwrite = false)
        {
            string from = this.pathHelper.Combine(path, oldFileName);
            string to = this.pathHelper.Combine(path, newFileName);
            this.Move(from, to, overwrite);
        }

        public void Move(string from, string to, bool overwrite = false)
        {
            from = this.pathHelper.ToAbsolute(from);
            to = this.pathHelper.ToAbsolute(to);
            if (overwrite && File.Exists(to))
            {
                File.Delete(to);
            }
            File.Move(from, to);
        }

        public void Write(string path, XElement xml)
        {
            path = this.pathHelper.ToAbsolute(path);
            xml.Save(path);
        }

        public string FormatSize(long size)
        {
            if (size >= 1024L * 1024 * 1024 * 1024 * 1024 * 1024)
                return $"{size / 1024.0 / 1024 / 1024 / 1024 / 1024 / 1024:0.#} EB";
            if (size >= 1024L * 1024 * 1024 * 1024 * 1024)
                return $"{size / 1024.0 / 1024 / 1024 / 1024 / 1024:0.#} PB";
            if (size >= 1024L * 1024 * 1024 * 1024)
                return $"{size / 1024.0 / 1024 / 1024 / 1024:0.#} TB";
            if (size >= 1024 * 1024 * 1024)
                return $"{size / 1024.0 / 1024 / 1024:0.#} GB";
            if (size >= 1024 * 1024)
                return $"{size / 1024.0 / 1024:0.#} MB";
            if (size >= 1024)
                return $"{size / 1024.0:0.#} kB";
            return $"{size} B";
        }

        public long ParseSize(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            Match match = sizeRegex.Match(value);
            if (!match.Success)
                return 0;
            double size = double.Parse(match.Groups["size"].Value);
            switch (match.Groups["unit"].Value.ToUpper())
            {
                case "EB":
                    return (long)(size * 1024 * 1024 * 1024 * 1024 * 1024 * 1024);
                case "PB":
                    return (long)(size * 1024 * 1024 * 1024 * 1024 * 1024);
                case "TB":
                    return (long)(size * 1024 * 1024 * 1024 * 1024);
                case "GB":
                    return (long)(size * 1024 * 1024 * 1024);
                case "MB":
                    return (long)(size * 1024 * 1024);
                case "KB":
                    return (long)(size * 1024);
                default:
                    return (long)size;
            }
        }

        public string GetName(string path)
        {
            return Path.GetFileName(path);
        }

        public FileSystemWatcher Watch(string path)
        {
            string absolutePath = FileSystem.ToAbsolutePath(path);
            string directory = FileSystem.GetDirectoryName(absolutePath);
            string file = FileSystem.GetFileName(absolutePath);
            FileSystemWatcher watchdog = new (directory);
            watchdog.Filter = file;
            watchdog.EnableRaisingEvents = true;
            return watchdog;
        }

        public DateTime GetLastWriteTime(params string[] pathChunks)
        {
            return File.GetLastWriteTime(this.pathHelper.ToAbsolute(pathChunks));
        }
    }
}
