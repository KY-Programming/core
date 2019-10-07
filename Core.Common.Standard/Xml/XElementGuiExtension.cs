using System;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public static class XElementGuiExtension
    {
        public static Guid GetGuid(this XElement element, string elementName, string exceptionText = null)
        {
            return new Guid(element.GetElement(elementName, exceptionText).Value);
        }

        public static Guid TryGetGuid(this XElement element, string elementName, Guid defaultValue = default(Guid))
        {
            XElement controllerElement = element.TryGetElement(elementName);
            if (controllerElement == null)
            {
                return defaultValue;
            }

            return new Guid(controllerElement.Value);
        }
    }
}