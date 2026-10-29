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
        FileSystem.WriteAllText(path, SerializeToString(data, hook));
    }

    internal static string SerializeToString<T>(T data, Action<JsonSerializer> hook = null)
    {
        using StringWriter writer = new();
        using JsonTextWriter jsonWriter = new(writer);
        CreateSerializer(hook).Serialize(jsonWriter, data);
        jsonWriter.Flush();
        return writer.ToString();
    }

    public static void Serialize<T>(T data, Stream stream, Action<JsonSerializer> hook = null)
    {
        using StreamWriter writer = new(stream);
        using JsonTextWriter jsonWriter = new(writer);
        CreateSerializer(hook).Serialize(jsonWriter, data);
        jsonWriter.Flush();
    }

    private static JsonSerializer CreateSerializer(Action<JsonSerializer> hook)
    {
        JsonSerializer serializer = new()
        {
            ContractResolver = new DoNotSerializeToFileSystemResolver()
        };
        hook?.Invoke(serializer);
        return serializer;
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
