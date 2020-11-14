using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

// ReSharper disable UseFileSystem

namespace KY.Core.DataAccess
{
    public class PathHelper
    {
        private const string currentSymbol = ".";
        private const string parentSymbol = "..";
        private const string driveSymbol = ":";

        private static readonly Regex absolutePathRegex = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                                                              ? new Regex(@"^(([A-z]:)|(file:\\\\)|(\\\\))")
                                                              : new Regex("^/");
        public string Root { get; }

        public PathHelper(string root = null)
        {
            this.Root = AppDomain.CurrentDomain.BaseDirectory;
            this.Root = this.ToAbsolute(root);
        }

        public bool IsAbsolute(string path)
        {
            return absolutePathRegex.IsMatch(path);
        }

        public string ToAbsolute(params string[] pathChunks)
        {
            string path = this.Combine(pathChunks);
            return !string.IsNullOrEmpty(path) && this.IsAbsolute(path) ? this.Format(path) : this.Combine(this.Root, this.Format(this.RemoveRelativeChar(path)));
        }

        public string ToRelative(string path, bool useRelativeChar = true)
        {
            path = this.RemoveRelativeChar(path);
            path = path.Replace(this.Root, string.Empty);
            path = this.Format(path);
            if (useRelativeChar)
            {
                path = "~" + Path.DirectorySeparatorChar + path;
            }
            return path;
        }

        public string Format(string path)
        {
            path = path?.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            bool isNetworkPath = path?.StartsWith(Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar) ?? false;
            bool isDrive = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) && (path?.StartsWith(Path.DirectorySeparatorChar.ToString()) ?? false);
            if (isNetworkPath || isDrive)
            {
                return path.TrimEnd(Path.DirectorySeparatorChar);
            }
            return path?.Trim(Path.DirectorySeparatorChar);
        }

        public string RemoveRelativeChar(string path)
        {
            if (path != null && path.StartsWith("~"))
                return path.Substring(1);
            return path;
        }

        public string Combine(params string[] pathChunks)
        {
            List<string> safePathChunks = pathChunks.Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (safePathChunks.Count == 0)
            {
                return pathChunks.FirstOrDefault();
            }
            bool isDrive = safePathChunks.First().Contains(driveSymbol);
            List<string> parts = new List<string>();
            foreach (string pathChunk in safePathChunks.Select(this.Format))
            {
                string[] chunks = pathChunk.Split(Path.DirectorySeparatorChar);
                foreach (string chunk in chunks)
                {
                    string last = parts.LastOrDefault();
                    if (chunk == currentSymbol)
                    {
                        if (parts.Count == 0)
                        {
                            parts.Add(chunk);
                        }
                    }
                    else if (chunk == parentSymbol)
                    {
                        if (parts.Count == 0 || last == parentSymbol)
                        {
                            parts.Add(chunk);
                        }
                        else if (isDrive && parts.Count > 1 || !isDrive && parts.Count > 0)
                        {
                            parts.Remove(last);
                        }
                    }
                    else if (parts.Count == 0 ||last == parentSymbol)
                    {
                        parts.Add(chunk);
                    }
                    else if (!string.IsNullOrEmpty(chunk))
                    {
                        parts.Add(chunk);
                    }
                }
            }
            return string.Join(Path.DirectorySeparatorChar.ToString(), parts);
        }

        public string Parent(string path)
        {
            return string.IsNullOrEmpty(path) ? path : Path.GetDirectoryName(path);
        }

        public string RelativeTo(string path, string to)
        {
            string[] pathChunks = this.Format(path)?.Split(Path.DirectorySeparatorChar) ?? new string[0];
            string[] toChunks = this.Format(to)?.Split(Path.DirectorySeparatorChar) ?? new string[0];
            int sameChunks = 0;
            for (int index = 0; index < pathChunks.Length; index++)
            {
                if (toChunks.Length <= index || pathChunks[index] != toChunks[index])
                {
                    break;
                }
                pathChunks[index] = string.Empty;
                sameChunks = index + 1;
            }
            string newPath = string.Join(Path.DirectorySeparatorChar.ToString(), pathChunks.Where(x => !string.IsNullOrEmpty(x)));
            for (int i = 0; i < toChunks.Length - sameChunks; i++)
            {
                newPath = this.Combine("..", newPath);
            }
            return newPath;
        }
    }
}