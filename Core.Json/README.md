# KY.Core.Json

JSON serialization helpers built on [Newtonsoft.Json](https://www.newtonsoft.com/json).

## Read and write files

```csharp
MySettings settings = Json.Deserialize<MySettings>("settings.json"); // default(T) if the file does not exist
Json.SerializeIndented(settings, "settings.json");
```

Relative paths are resolved through `KY.Core.DataAccess.FileSystem`, i.e. against the application base directory. To go through a specific `IFileSystem` instead, use the extension methods `DeserializeJson`, `SerializeJson` and `SerializeJsonIndented`.

## Keep properties out of files

Properties marked with `[DoNotSerializeToFileSystem]` are skipped by all `Json.Serialize*` methods. To get the same behavior with plain Newtonsoft.Json, pass `JsonSettings.Default` or `JsonSettings.Indented`:

```csharp
string json = JsonConvert.SerializeObject(value, JsonSettings.Default);
```

## Edit JSON trees

`Json.Object` and `Json.Array` change a `JToken` at a JSON path: set a property (also first or after a given property), remove it, or add, insert and remove array items.

```csharp
JToken root = JToken.Parse(text);
Json.Object.Set(root, "server.port", 8080);
Json.Array.Add(root, "server.hosts", "localhost");
```
