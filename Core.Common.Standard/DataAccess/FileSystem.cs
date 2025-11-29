using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace KY.Core.DataAccess;

public static class FileSystem
{
    private static readonly PathHelper pathHelper = new();
    private static readonly FileHelper fileHelper = new(pathHelper);
    private static readonly DirectoryHelper directoryHelper = new(pathHelper);

    public static string Root => pathHelper.Root;

    public static IFileSystem Create(string root = null)
    {
        return new FileSystemInstance(root);
    }

    public static IFileSystem Create(Assembly assembly)
    {
        return new FileSystemInstance(Parent(assembly.Location));
    }

    public static string[] GetDirectories(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
    {
        return directoryHelper.GetDirectories(path, searchPattern, option);
    }

    public static DirectoryInfo GetDirectoryInfo(params string[] pathChunks)
    {
        return directoryHelper.GetDirectoryInfo(pathChunks);
    }

    public static DirectoryInfo[] GetDirectoryInfos(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
    {
        return directoryHelper.GetDirectoryInfos(path, searchPattern, option);
    }

    public static bool DirectoryExists(params string[] pathChunks)
    {
        return directoryHelper.Exists(pathChunks);
    }

    public static void CreateDirectory(params string[] pathChunks)
    {
        directoryHelper.Create(pathChunks);
    }

    public static void MoveDirectory(string path, string oldDirectoryName, string newDirectoryName, bool overwrite = false)
    {
        MoveDirectory(pathHelper.Combine(path, oldDirectoryName), pathHelper.Combine(path, newDirectoryName), overwrite);
    }

    public static void MoveDirectory(string from, string to, bool overwrite = false)
    {
        directoryHelper.Move(from, to, overwrite);
    }

    public static void DeleteDirectory(params string[] pathChunks)
    {
        directoryHelper.Delete(pathChunks);
    }

    public static void CopyDirectory(string from, string to, bool overwrite = false)
    {
        directoryHelper.Copy(from, to, overwrite);
    }

    public static FileInfo[] GetFileInfos(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
    {
        return fileHelper.GetInfos(path, searchPattern, option);
    }

    public static string[] GetFiles(string path, string searchPattern = null, SearchOption option = SearchOption.TopDirectoryOnly)
    {
        return fileHelper.GetFiles(path, searchPattern, option);
    }

    public static string ReadAllText(string path, Encoding encoding = null)
    {
        return fileHelper.ReadAllText(path, encoding);
    }

    public static string ReadAllText(string path, string fileName, Encoding encoding = null)
    {
        return ReadAllText(pathHelper.Combine(path, fileName), encoding);
    }

    public static void WriteAllText(string path, string content, Encoding encoding = null)
    {
        fileHelper.WriteAllText(path, content, encoding);
    }

    public static void WriteAllText(string path, string fileName, string content, Encoding encoding = null)
    {
        WriteAllText(pathHelper.Combine(path, fileName), content, encoding);
    }

    public static void AppendAllText(string path, string content, Encoding encoding = null)
    {
        fileHelper.AppendAllText(path, content, encoding);
    }

    public static bool FileExists(params string[] pathChunks)
    {
        return fileHelper.Exists(pathChunks);
    }

    public static void DeleteFile(params string[] pathChunks)
    {
        fileHelper.Delete(pathChunks);
    }

    public static string[] ReadAllLines(string path, Encoding encoding = null)
    {
        return fileHelper.ReadAllLines(path, encoding);
    }

    public static XElement ReadXml(params string[] pathChunks)
    {
        return fileHelper.ReadXml(pathChunks);
    }

    public static Stream OpenRead(params string[] pathChunks)
    {
        return fileHelper.OpenRead(pathChunks);
    }

    public static Stream OpenWrite(params string[] pathChunks)
    {
        return fileHelper.OpenWrite(pathChunks);
    }

    public static void CopyFile(string from, string to, bool overwrite = false)
    {
        fileHelper.Copy(from, to, overwrite);
    }

    public static void MoveFile(string from, string to, bool overwrite = false)
    {
        fileHelper.Move(from, to, overwrite);
    }

    public static string FormatFileSize(long size)
    {
        return fileHelper.FormatSize(size);
    }

    public static bool IsAbsolute(string path)
    {
        return pathHelper.IsAbsolute(path);
    }

    public static string ToAbsolutePath(string path)
    {
        return pathHelper.ToAbsolute(path);
    }

    public static string ToRelativePath(string path, bool useRelativeChar = true)
    {
        return pathHelper.ToRelative(path, useRelativeChar);
    }

    public static string FormatPath(string path)
    {
        return pathHelper.Format(path);
    }

    public static string Combine(params string[] pathChunks)
    {
        return pathHelper.Combine(pathChunks);
    }

    public static string Parent(string path)
    {
        return pathHelper.Parent(path);
    }

    public static string RelativeTo(string path, string to)
    {
        return pathHelper.RelativeTo(path, to);
    }

    public static string DirectorySeparator()
    {
        return Path.DirectorySeparatorChar.ToString();
    }

    public static DirectoryInfo ParentInfo(string path)
    {
        return new DirectoryInfo(pathHelper.Parent(path));
    }

    public static void Write(string path, XElement xml)
    {
        fileHelper.Write(path, xml);
    }

    public static string? ToDirectory(string? directory)
    {
        return directoryHelper.ToDirectory(directory);
    }

    public static string? GetDirectoryName(string? path)
    {
        return directoryHelper.GetName(path);
    }

    public static string GetFileName(string path)
    {
        return fileHelper.GetName(path);
    }

    public static FileSystemWatcher Watch(string path)
    {
        if (DirectoryExists(path))
        {
            return directoryHelper.Watch(path);
        }
        if (FileExists(path))
        {
            return fileHelper.Watch(path);
        }
        throw new InvalidOperationException($"Can not watch file or directory '{path}': Not found.");
    }

    public static DateTime GetLastWriteTime(params string[] pathChunks)
    {
        if (DirectoryExists(pathChunks))
        {
            return directoryHelper.GetLastWriteTime(pathChunks);
        }
        return fileHelper.GetLastWriteTime(pathChunks);
    }
}
