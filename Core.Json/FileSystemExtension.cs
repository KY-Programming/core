using System.IO;
using KY.Core.DataAccess;
using Newtonsoft.Json;

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
        return Json.Deserialize<T>(stream);
    }

    public static void SerializeJson<T>(this IFileSystem fileSystem, T data, string path)
    {
        fileSystem.WriteAllText(path, Json.SerializeToString(data));
    }

    public static void SerializeJsonIndented<T>(this IFileSystem fileSystem, T data, string path)
    {
        fileSystem.WriteAllText(path, Json.SerializeToString(data, serializer => serializer.Formatting = Formatting.Indented));
    }
}
