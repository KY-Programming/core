using System.IO;

namespace KY.Core.Extension;

public static class FileSystemWatcherExtension
{
    public static FileSystemWatcher OnChanged(this FileSystemWatcher watcher, FileSystemEventHandler handler)
    {
        watcher.Changed += handler;
        return watcher;
    }

    public static FileSystemWatcher OnCreated(this FileSystemWatcher watcher, FileSystemEventHandler handler)
    {
        watcher.Created += handler;
        return watcher;
    }

    public static FileSystemWatcher OnDeleted(this FileSystemWatcher watcher, FileSystemEventHandler handler)
    {
        watcher.Deleted += handler;
        return watcher;
    }

    public static FileSystemWatcher OnRenamed(this FileSystemWatcher watcher, RenamedEventHandler handler)
    {
        watcher.Renamed += handler;
        return watcher;
    }

    public static FileSystemWatcher OnError(this FileSystemWatcher watcher, ErrorEventHandler handler)
    {
        watcher.Error += handler;
        return watcher;
    }

    public static FileSystemWatcher IncludeSubdirectories (this FileSystemWatcher watcher)
    {
        watcher.IncludeSubdirectories = true;
        return watcher;
    }
}
