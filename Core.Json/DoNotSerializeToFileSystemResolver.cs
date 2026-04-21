using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KY.Core;

public class DoNotSerializeToFileSystemResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);
        if (member is PropertyInfo info && info.GetCustomAttribute<DoNotSerializeToFileSystemAttribute>() != null)
        {
            property.ShouldSerialize = _ => false;
        }
        return property;
    }
}
