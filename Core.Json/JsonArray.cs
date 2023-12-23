using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace KY.Core;

public class JsonArray
{
    public void Add<T>(JToken target, string propertyPath, T value)
    {
        if (value is JToken token)
        {
            this.Add(target, propertyPath, token);
        }
        else
        {
            this.Add(target, propertyPath, new JValue(value));
        }
    }

    public void Add(JToken target, string propertyPath, JToken value)
    {
        if (target.SelectToken(propertyPath) is not JArray array)
        {
            throw new InvalidOperationException($"The property {propertyPath} is not an array");
        }
        array.Add(value);
    }
    
    public void Insert<T>(JToken target, string propertyPath, int index, T value)
    {
        if (value is JToken token)
        {
            this.Insert(target, propertyPath, index, token);
        }
        else
        {
            this.Insert(target, propertyPath, index, new JValue(value));
        }
    }

    public void Insert(JToken target, string propertyPath, int index, JToken value)
    {
        if (target.SelectToken(propertyPath) is not JArray array)
        {
            throw new InvalidOperationException($"The property {propertyPath} is not an array");
        }
        array.Insert(index, value);
    }
    
    public void Remove<T>(JToken target, string propertyPath, T value)
    {
        if (value is JToken token)
        {
            this.Remove(target, propertyPath, token);
        }
        else
        {
            this.Remove(target, propertyPath, new JValue(value));
        }
    }

    public void Remove(JToken target, string propertyPath, JToken value)
    {
        if (target.SelectToken(propertyPath) is not JArray array)
        {
            throw new InvalidOperationException($"The property {propertyPath} is not an array");
        }
        if (array.Contains(value))
        {
            array.Remove(value);
        }
        else
        {
            array.Remove(array.First(x => x.ToString() == value.ToString()));
        }
    }
}
