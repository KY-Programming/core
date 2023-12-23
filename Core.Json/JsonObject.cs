using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace KY.Core;

public class JsonObject
{
    public void Set<T>(JToken target, string propertyPath, T value)
    {
        if (value is JToken token)
        {
            this.Set(target, propertyPath, token);
        }
        else
        {
            this.Set(target, propertyPath, (JToken)new JValue(value));
        }
    }

    public void Set(JToken target, string propertyPath, JToken value)
    {
        (string objectPath, string propertyName) = this.GetProperty(propertyPath);
        JToken token = string.IsNullOrEmpty(objectPath) ? target : target.SelectToken(objectPath);
        if (token is not JObject obj)
        {
            throw new InvalidOperationException($"The property {objectPath} is not an object");
        }
        obj[propertyName] = value;
    }

    public void SetFirst<T>(JToken target, string propertyPath, T value)
    {
        if (value is JToken token)
        {
            this.SetFirst(target, propertyPath, token);
        }
        else
        {
            this.SetFirst(target, propertyPath, (JToken)new JValue(value));
        }
    }

    public void SetFirst(JToken target, string propertyPath, JToken value)
    {
        (string objectPath, string propertyName) = this.GetProperty(propertyPath);
        JToken token = string.IsNullOrEmpty(objectPath) ? target : target.SelectToken(objectPath);
        if (token is not JObject obj)
        {
            throw new InvalidOperationException($"The property {objectPath} is not an object");
        }
        if (obj.ContainsKey(propertyName))
        {
            obj[propertyName] = value;
        }
        else
        {
            obj.AddFirst(new JProperty(propertyName, value));
        }
    }

    public void SetAfter<T>(JToken target, string propertyPath, string afterProperty, T value)
    {
        if (value is JToken token)
        {
            this.SetAfter(target, propertyPath, afterProperty, token);
        }
        else
        {
            this.SetAfter(target, propertyPath, afterProperty, (JToken)new JValue(value));
        }
    }

    public void SetAfter(JToken target, string propertyPath, string afterProperty, JToken value)
    {
        (string objectPath, string propertyName) = this.GetProperty(propertyPath);
        JToken token = string.IsNullOrEmpty(objectPath) ? target : target.SelectToken(objectPath);
        if (token is not JObject obj)
        {
            throw new InvalidOperationException($"The property {objectPath} is not an object");
        }
        if (obj.ContainsKey(propertyName) || !obj.ContainsKey(afterProperty))
        {
            obj[propertyName] = value;
        }
        else
        {
            obj[afterProperty]?.Parent?.AddAfterSelf(new JProperty(propertyName, value));
        }
    }

    public void Remove(JToken target, string propertyPath)
    {
        (string objectPath, string propertyName) = this.GetProperty(propertyPath);
        JToken token = string.IsNullOrEmpty(objectPath) ? target : target.SelectToken(objectPath);
        if (token is not JObject obj)
        {
            throw new InvalidOperationException($"The property {objectPath} is not an object");
        }
        obj.Remove(propertyName);
    }

    private Tuple<string, string> GetProperty(string propertyPath)
    {
        string[] chunks = propertyPath.Split('.');
        return new Tuple<string, string>(string.Join(".", chunks.Take(chunks.Length - 1)), chunks.Last());
    }
}
