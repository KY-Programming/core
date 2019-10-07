using System.Xml.Linq;

namespace KY.Core.Xml
{
    public static class XElementStringExtension
    {
        public static string GetString(this XElement element, string elementName, string exceptionText = null)
        {
            return element.GetElement(elementName, exceptionText).Value;
        }

        public static string TryGetString(this XElement element, string elementName, string defaultValue = null)
        {
            XElement controllerElement = element.TryGetElement(elementName);
            if (controllerElement == null)
            {
                return defaultValue;
            }

            return controllerElement.Value;
        }

        public static string GetStringAttribute(this XElement element, string attributeName)
        {
            XAttribute attribute = element.GetAttribute(attributeName);
            return attribute.Value;
        }

        public static string TryGetStringAttribute(this XElement element, string attributeName, string defaultValue = null)
        {
            XAttribute attribute = element.TryGetAttribute(attributeName);
            if (attribute == null)
            {
                return defaultValue;
            }

            return attribute.Value;
        }
    }
}