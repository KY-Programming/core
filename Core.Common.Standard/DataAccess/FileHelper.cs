using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;

// ReSharper disable UseFileSystem

namespace KY.Core.DataAccess;

internal class FileHelper
{
    private const int replaceAttempts = 20;
    private static readonly TimeSpan replaceRetryDelay = TimeSpan.FromMilliseconds(25);
    private static readonly Regex sizeRegex = new(@"^(?<size>[0-9.,]+)\s?(?<unit>[A-z]+)$");
    // UTF-8 without a byte order mark: it carries no information in UTF-8, JSON forbids it (RFC 8259) and shebangs and
    // several Linux tools break on it. Reading still accepts files that have one.
    private static readonly Encoding defaultEncoding = new UTF8Encoding(false);
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
        DirectoryInfo directory = new(path);
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

    public void WriteAllText(string path, string content, Encoding encoding = null, bool atomic = true)
    {
        path = this.pathHelper.ToAbsolute(path);
        if (!atomic)
        {
            File.WriteAllText(path, content, encoding ?? defaultEncoding);
            return;
        }
        WriteAtomic(path, stream =>
        {
            using StreamWriter writer = new(stream, encoding ?? defaultEncoding, 4096, true);
            writer.Write(content);
        });
    }

    public void AppendAllText(string path, string content, Encoding encoding = null)
    {
        path = this.pathHelper.ToAbsolute(path);
        File.AppendAllText(path, content, encoding ?? defaultEncoding);
    }

    public void WriteAllBytes(string path, byte[] content, bool atomic = true)
    {
        path = this.pathHelper.ToAbsolute(path);
        if (!atomic)
        {
            File.WriteAllBytes(path, content);
            return;
        }
        WriteAtomic(path, stream => stream.Write(content, 0, content.Length));
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
        return File.Open(path, FileMode.Create);
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
#if NET
        // Replaces the target in one rename, without the moment in which it does not exist
        File.Move(from, to, overwrite);
#else
        if (overwrite && File.Exists(to))
        {
            File.Delete(to);
        }
        File.Move(from, to);
#endif
    }

    public void Write(string path, XElement xml)
    {
        path = this.pathHelper.ToAbsolute(path);
        WriteAtomic(path, xml.Save);
    }

    // Writes into a temporary file next to the target and renames it over the target. A reader sees the old or the new
    // content, never a half written file: two concurrent writers to the same path do not block each other on Linux and
    // would otherwise interleave, and a crash mid-write would leave a truncated file behind.
    // The rename gives the target a new inode: a hard link to the old file keeps the old content, and a file that is
    // itself a mount point (a Docker single-file bind mount) cannot be replaced at all.
    private static void WriteAtomic(string path, Action<Stream> write)
    {
        path = ResolveLinkTarget(path);
        string temporaryPath = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
        try
        {
            using (FileStream stream = new(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                write(stream);
                stream.Flush(true);
            }
            Replace(temporaryPath, path);
        }
        catch
        {
            DeleteTemporaryFile(temporaryPath);
            throw;
        }
    }

    // A symbolic link stays a link: the file it points to is replaced
    private static string ResolveLinkTarget(string path)
    {
#if NET
        FileInfo file = new(path);
        if (file.LinkTarget != null)
        {
            return file.ResolveLinkTarget(true)?.FullName ?? path;
        }
#endif
        return path;
    }

    // On Windows the rename fails while a reader holds the target open without FileShare.Delete (File.ReadAllText does),
    // so it is retried for a moment. On Linux it never waits for readers.
    private static void Replace(string temporaryPath, string path)
    {
        for (int attempt = 1; ; attempt++)
        {
            try
            {
#if NET
                File.Move(temporaryPath, path, true);
#else
                if (File.Exists(path))
                {
                    File.Replace(temporaryPath, path, null);
                }
                else
                {
                    File.Move(temporaryPath, path);
                }
#endif
                return;
            }
            catch (Exception exception) when (attempt < replaceAttempts && exception is IOException or UnauthorizedAccessException)
            {
                Thread.Sleep(replaceRetryDelay);
            }
        }
    }

    private static void DeleteTemporaryFile(string temporaryPath)
    {
        try
        {
            File.Delete(temporaryPath);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // The write already failed and that exception is the one that matters; a leftover hidden .tmp file is harmless
        }
    }

    public string FormatSize(long size)
    {
        if (size >= 1024L * 1024 * 1024 * 1024 * 1024 * 1024)
        {
            return $"{size / 1024.0 / 1024 / 1024 / 1024 / 1024 / 1024:0.#} EB";
        }
        if (size >= 1024L * 1024 * 1024 * 1024 * 1024)
        {
            return $"{size / 1024.0 / 1024 / 1024 / 1024 / 1024:0.#} PB";
        }
        if (size >= 1024L * 1024 * 1024 * 1024)
        {
            return $"{size / 1024.0 / 1024 / 1024 / 1024:0.#} TB";
        }
        if (size >= 1024 * 1024 * 1024)
        {
            return $"{size / 1024.0 / 1024 / 1024:0.#} GB";
        }
        if (size >= 1024 * 1024)
        {
            return $"{size / 1024.0 / 1024:0.#} MB";
        }
        if (size >= 1024)
        {
            return $"{size / 1024.0:0.#} kB";
        }
        return $"{size} B";
    }

    public long ParseSize(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return 0;
        }
        Match match = sizeRegex.Match(value);
        if (!match.Success)
        {
            return 0;
        }
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
        FileSystemWatcher watchdog = new(directory);
        watchdog.Filter = file;
        watchdog.EnableRaisingEvents = true;
        return watchdog;
    }

    public DateTime GetLastWriteTime(params string[] pathChunks)
    {
        return File.GetLastWriteTime(this.pathHelper.ToAbsolute(pathChunks));
    }
}
