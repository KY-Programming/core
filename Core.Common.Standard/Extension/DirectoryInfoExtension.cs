using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KY.Core
{
    public static class DirectoryInfoExtension
    {
        public static IEnumerable<FileInfo> GetFilesByExtension(this DirectoryInfo directory, string extension)
        {
            return directory.GetFilesByExtension(extension.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static IEnumerable<FileInfo> GetFilesByExtension(this DirectoryInfo directory, params string[] extension)
        {
            List<string> extensions = extension.Select(x => x.Replace("*", string.Empty).ToLowerInvariant()).ToList();
            foreach (FileInfo fileInfo in directory.GetFiles())
            {
                if (extensions.Contains(fileInfo.Extension.ToLowerInvariant()))
                    yield return fileInfo;
            }
        }
    }
}