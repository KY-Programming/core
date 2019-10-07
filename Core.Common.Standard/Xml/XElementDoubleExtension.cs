using System.Xml;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public static class XElementDoubleExtension
    {
        public static double GetDouble(this XElement element, string elementName, string exceptionText = null)
        {
            return XmlConvert.ToDouble(element.GetElement(elementName, exceptionText).Value);
        }

        public static double TryGetDouble(this XElement element, string elementName, double defaultValue = default(double))
        {
            XElement controllerElement = element.TryGetElement(elementName);
            if (controllerElement == null)
            {
                return defaultValue;
            }
            return XmlConvert.ToDouble(controllerElement.Value);
        }

        public static double GetDoubleAttribute(this XElement element, string attributeName)
        {
            XAttribute attribute = element.GetAttribute(attributeName);
            return XmlConvert.ToDouble(attribute.Value);
        }

        public static double TryGetDoubleAttribute(this XElement element, string attributeName, double defaultValue = 0)
        {
            XAttribute attribute = element.TryGetAttribute(attributeName);
            if (attribute == null)
            {
                return defaultValue;
            }
            return XmlConvert.ToDouble(attribute.Value);
        }

        public static double? GetNullableDouble(this XElement element, string elementName, string exceptionText = null)
        {
            return XmlConvert.ToDouble(element.GetElement(elementName, exceptionText).Value);
        }

        public static double? TryGetNullableDouble(this XElement element, string elementName, double? defaultValue = null)
        {
            XElement controllerElement = element.TryGetElement(elementName);
            if (controllerElement == null)
            {
                return defaultValue;
            }
            return XmlConvert.ToDouble(controllerElement.Value);
        }

        public static double? GetNullableDoubleAttribute(this XElement element, string attributeName)
        {
            XAttribute attribute = element.GetAttribute(attributeName);
            return XmlConvert.ToDouble(attribute.Value);
        }

        public static double? TryGetNullableDoubleAttribute(this XElement element, string attributeName, double? defaultValue = null)
        {
            XAttribute attribute = element.TryGetAttribute(attributeName);
            if (attribute == null)
            {
                return defaultValue;
            }
            return XmlConvert.ToDouble(attribute.Value);
        }
    }
}