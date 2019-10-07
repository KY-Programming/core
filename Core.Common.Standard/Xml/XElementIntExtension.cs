using System.Xml;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public static class XElementIntExtension
    {
        public static int GetInt(this XElement element, string elementName, string exceptionText = null)
        {
            return XmlConvert.ToInt32(element.GetElement(elementName, exceptionText).Value);
        }

        public static int TryGetInt(this XElement element, string elementName, int defaultValue = 0)
        {
            XElement foundElement = element.TryGetElement(elementName);
            if (foundElement == null)
            {
                return defaultValue;
            }
            return XmlConvert.ToInt32(foundElement.Value);
        }

        public static int GetIntAttribute(this XElement element, string attributeName)
        {
            XAttribute attribute = element.GetAttribute(attributeName);
            return XmlConvert.ToInt32(attribute.Value);
        }

        public static int TryGetIntAttribute(this XElement element, string attributeName, int defaultValue = 0)
        {
            XAttribute attribute = element.TryGetAttribute(attributeName);
            if (attribute == null)
            {
                return defaultValue;
            }

            return XmlConvert.ToInt32(attribute.Value);
        }
    }
}