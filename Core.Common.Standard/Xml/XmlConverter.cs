using System;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public static class XmlConverter
    {
        public static T Deserialize<T>(XElement element)
        {
            XmlSerializer serializer = new XmlSerializer();
            return serializer.Deserialize<T>(element);
        }

        public static void Deserialize<T>(XElement element, T target)
        {
            XmlSerializer serializer = new XmlSerializer();
            serializer.Deserialize<T>(element, target);
        }

        public static XmlSerializer Map(Action<IXmlMappingCreate> action)
        {
            return XmlSerializer.Create().Map(action);
        }
    }
}