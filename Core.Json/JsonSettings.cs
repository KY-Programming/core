using Newtonsoft.Json;

namespace KY.Core;

public static class JsonSettings
{
    /// <summary>
    /// Use these settings to apply the default JSON style e.g., to ignore all properties marked with [DoNotSerializeToFileSystem].
    /// </summary>
    /// <example>
    /// JsonConvert.SerializeObject(value, JsonSettings.Default);
    /// </example>   
    public static readonly JsonSerializerSettings Default = new()
    {
        ContractResolver = new DoNotSerializeToFileSystemResolver()
    };

    public static readonly JsonSerializerSettings Indented = new()
    {
        ContractResolver = new DoNotSerializeToFileSystemResolver(),
        Formatting = Formatting.Indented
    };
}
