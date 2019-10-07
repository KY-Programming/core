using System;
using System.Collections.Generic;

namespace KY.Core
{
    public static class DictionaryExtension
    {
        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> list, IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            Dictionary<TKey, TValue> remainingItems = new Dictionary<TKey, TValue>(list);
            foreach (KeyValuePair<TKey, TValue> item in source)
            {
                if (remainingItems.ContainsKey(item.Key) && remainingItems[item.Key] is IMergeable)
                {
                    ((IMergeable)remainingItems[item.Key]).Merge(item.Value);
                }
                else
                {
                    // Hinzufügen bzw ersetzten falls gefunden
                    list[item.Key] = item.Value;
                }
                remainingItems.Remove(item.Key);
            }
            // Alle Elemente die nicht in source enthalten waren, müssen entfernt werden
            foreach (TKey key in remainingItems.Keys)
            {
                list.Remove(key);
            }
        }

        public static void Remove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                if (pair.Value.Equals(value))
                {
                    dictionary.Remove(pair.Key);
                    return;
                }
            }
        }

        public static void AddIfNotExists<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }

        public static void AddIfNotExists<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> action)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, action());
            }
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            return defaultValue;
        }
    }
}