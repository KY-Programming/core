using System.Xml;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public static class XElementBoolExtension
    {
        public static bool GetBool(this XElement element, string elementName, string exceptionText = null)
        {
            return XmlConvert.ToBoolean(element.GetElement(elementName, exceptionText).Value);
        }

        public static bool TryGetBool(this XElement element, string elementName, bool defaultValue = false)
        {
            XElement foundElement = element.TryGetElement(elementName);
            if (foundElement == null)
            {
                return defaultValue;
            }
            return XmlConvert.ToBoolean(foundElement.Value);
        }

        public static bool GetBoolAttribute(this XElement element, string attributeName)
        {
            XAttribute attribute = element.GetAttribute(attributeName);
            return XmlConvert.ToBoolean(attribute.Value);
        }

        public static bool TryGetBoolAttribute(this XElement element, string attributeName, bool defaultValue = false)
        {
            XAttribute attribute = element.TryGetAttribute(attributeName);
            if (attribute == null)
            {
                return defaultValue;
            }

            return XmlConvert.ToBoolean(attribute.Value);
        }
    }
}