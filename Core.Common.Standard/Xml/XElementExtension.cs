using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace KY.Core.Xml
{
    public static class XElementExtension
    {
        public static void SetEValue<T>(this XElement element, string elementName, T value)
        {
            element.TryGetOrAddElement(elementName).Value = Equals(value, default(T)) ? string.Empty : value.ToString();
        }

        public static void SetElementValue<T>(this XElement element, string elementName, T value)
        {
            element.SetEValue(elementName, value);
        }

        public static XElement TryGetElement(this XElement element, string elementName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (elementName == null)
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            if (elementName.Contains("/"))
            {
                return element.XPathSelectElement(elementName);
            }
            return element.Element(elementName);
        }

        public static XElement GetElement(this XElement element, string elementName, string exceptionText = null)
        {
            XElement foundElement = element.TryGetElement(elementName);
            if (foundElement == null)
            {
                throw new XmlException(exceptionText ?? elementName + " not found");
            }

            return foundElement;
        }

        public static IEnumerable<XElement> GetElementsIgnoreNamespace(this XElement element, string name)
        {
            IEnumerable<XElement> elements = element.Elements();
            return elements.Where(x => x.Name.LocalName == name);
        }

        public static XElement GetElementIgnoreNamespace(this XElement element, string name)
        {
            IEnumerable<XElement> elements = element.Elements();
            return elements.FirstOrDefault(x => x.Name.LocalName == name);
        }

        public static XElement AddElement(this XElement element, string elementName)
        {
            XElement newElement = new XElement(elementName);
            element.Add(newElement);
            return newElement;
        }

        public static XElement AddElement<T>(this XElement element, string elementName, T value)
        {
            XElement newElement = element.AddElement(elementName);
            if (value == null)
            {
                Logger.Error($"Can not set value 'null' at {element.Name}.{elementName}");
            }
            else
            {
                newElement.SetValue(value);
            }
            return newElement;
        }

        public static XElement TryAddElement<T>(this XElement element, string elementName, T value)
        {
            return value == null ? null : element.AddElement(elementName, value);
        }

        public static XElement TryGetOrAddElement(this XElement element, string elementName)
        {
            return element.TryGetElement(elementName) ?? element.AddElement(elementName);
        }

        public static bool Exists(this XElement element, string elementName)
        {
            return element.TryGetElement(elementName) != null;
        }

        public static XAttribute TryGetAttribute(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (attributeName == null)
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            return element.Attribute(attributeName);
        }

        public static XAttribute GetAttribute(this XElement element, string attributeName, string exceptionText = null)
        {
            XAttribute foundAttribute = element.TryGetAttribute(attributeName);
            if (foundAttribute == null)
            {
                throw new XmlException(exceptionText ?? attributeName + " not found");
            }

            return foundAttribute;
        }

        public static string GetPath(this XElement element)
        {
            return $"{(element.Parent?.Parent != null ? element.Parent?.GetPath() : string.Empty)}/{element.Name.LocalName}";
        }
    }
}