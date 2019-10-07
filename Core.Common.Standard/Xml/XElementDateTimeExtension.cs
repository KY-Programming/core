using System;
using System.Xml;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public static class XElementDateTimeExtension
    {
        public static DateTime GetDateTime(this XElement element, string elementName, string exceptionText = null)
        {
            return ConvertDateTime(element.GetElement(elementName, exceptionText).Value);
        }

        public static DateTime TryGetDateTime(this XElement element, string elementName, DateTime defaultValue = default(DateTime))
        {
            XElement foundElement = element.TryGetElement(elementName);
            if (foundElement == null)
            {
                return defaultValue;
            }
            return ConvertDateTime(foundElement.Value);
        }

        private static DateTime ConvertDateTime(string value)
        {
            if ("now".Equals(value, StringComparison.InvariantCultureIgnoreCase))
            {
                return DateTime.Now;
            }
            if ("today".Equals(value, StringComparison.InvariantCultureIgnoreCase))
            {
                return DateTime.Today;
            }
            return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
        }

        public static DateTime GetDateTimeAttribute(this XElement element, string attributeName, string exceptionText = null)
        {
            XAttribute attribute = element.GetAttribute(attributeName, exceptionText);
            return XmlConvert.ToDateTime(attribute.Value, XmlDateTimeSerializationMode.RoundtripKind);
        }

        public static DateTime TryGetDateTimeAttribute(this XElement element, string attributeName, DateTime defaultValue = default(DateTime))
        {
            XAttribute attribute = element.TryGetAttribute(attributeName);
            if (attribute == null)
            {
                return defaultValue;
            }

            return XmlConvert.ToDateTime(attribute.Value, XmlDateTimeSerializationMode.RoundtripKind);
        }
    }
}