using System;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public static class XElementUriExtension
    {
        public static Uri GetUri(this XElement element, string elementName, string exceptionText = null)
        {
            return new Uri(element.GetElement(elementName, exceptionText).Value);
        }

        public static Uri TryGetUri(this XElement element, string elementName, Uri defaultValue = default(Uri))
        {
            XElement controllerElement = element.TryGetElement(elementName);
            if (controllerElement == null)
            {
                return defaultValue;
            }

            return new Uri(controllerElement.Value);
        }
    }
}