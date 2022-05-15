using System.IO;
using KY.Core.DataAccess;

namespace KY.Core;

public static class FileSystemExtension
{
    public static T DeserializeJson<T>(this IFileSystem fileSystem, string path)
    {
        if (!fileSystem.FileExists(path))
        {
            return default;
        }
        using Stream stream = fileSystem.OpenRead(path);
        return Json.Deserialize<T>(path);
    }

    public static void SerializeJson<T>(this IFileSystem fileSystem, T data, string path)
    {
        using Stream stream = fileSystem.OpenWrite(path);
        Json.Serialize(data, path);
    }

    public static void SerializeJsonIndented<T>(this IFileSystem fileSystem, T data, string path)
    {
        Json.SerializeIndented(data, path);
    }
}
