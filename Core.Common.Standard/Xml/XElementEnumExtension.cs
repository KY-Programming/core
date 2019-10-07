using System;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public static class XElementEnumExtension
    {
        public static T GetEnum<T>(this XElement element, string elementName)
        {
            string value = element.GetString(elementName);
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T TryGetEnum<T>(this XElement element, string elementName, T defaultValue = default(T))
        {
            string value = element.TryGetString(elementName);
            if (value == null)
            {
                return defaultValue;
            }
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T GetEnumAttribute<T>(this XElement element, string attributeName)
        {
            XAttribute attribute = element.GetAttribute(attributeName);
            return (T)Enum.Parse(typeof(T), attribute.Value, true);
        }

        public static T TryGetEnumAttribute<T>(this XElement element, string attributeName, T defaultValue = default(T), bool ignoreCase = false)
        {
            XAttribute attribute = element.TryGetAttribute(attributeName);
            if (attribute == null)
            {
                return defaultValue;
            }

            return (T)Enum.Parse(typeof(T), attribute.Value, ignoreCase);
        }
    }
}