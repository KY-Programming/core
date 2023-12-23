using System;
using System.IO;
using KY.Core.DataAccess;
using Newtonsoft.Json;

namespace KY.Core;

public static class Json
{
    public static JsonArray Array { get; } = new();
    public static JsonObject Object { get; } = new();
    
    public static T Deserialize<T>(string path)
    {
        if (!FileSystem.FileExists(path))
        {
            return default;
        }
        using Stream stream = FileSystem.OpenRead(path);
        return Deserialize<T>(stream);
    }

    public static T Deserialize<T>(Stream stream)
    {
        using StreamReader reader = new(stream);
        using JsonTextReader jsonReader = new(reader);
        JsonSerializer serializer = new();
        return serializer.Deserialize<T>(jsonReader);
    }

    public static void Serialize<T>(T data, string path, Action<JsonSerializer> hook = null)
    {
        using Stream stream = FileSystem.OpenWrite(path);
        Serialize(data, stream, hook);
    }

    public static void Serialize<T>(T data, Stream stream, Action<JsonSerializer> hook = null)
    {
        using StreamWriter writer = new(stream);
        using JsonTextWriter jsonWriter = new(writer);
        JsonSerializer serializer = new();
        hook?.Invoke(serializer);
        serializer.Serialize(jsonWriter, data);
        jsonWriter.Flush();
    }

    public static void SerializeIndented<T>(T data, string path)
    {
        Serialize(data, path, serializer => serializer.Formatting = Formatting.Indented);
    }

    public static void SerializeIndented<T>(T data, Stream stream)
    {
        Serialize(data, stream, serializer => serializer.Formatting = Formatting.Indented);
    }
}
