using System;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public class XmlSerializer
    {
        private readonly XmlMappingList mapping;
        private readonly XmlDeserializer deserializer;

        public XmlSerializer()
        {
            this.mapping = new XmlMappingList();
            this.deserializer = new XmlDeserializer(this.mapping);
        }

        public T Deserialize<T>(XElement element)
        {
            return this.deserializer.Deserialize<T>(element);
        }

        public void Deserialize<T>(XElement element, T target)
        {
            this.Deserialize(typeof(T), element, target);
        }

        public object Deserialize(Type type, XElement element)
        {
            return this.deserializer.Deserialize(type, element);
        }

        public void Deserialize(Type type, XElement element, object target)
        {
            this.deserializer.Deserialize(type, element, target);
        }

        public static XmlSerializer Create()
        {
            return new XmlSerializer();
        }

        public XmlSerializer Map(Action<IXmlMappingCreate> action)
        {
            action(new XmlMappingSyntax(this.mapping));
            return this;
        }
    }
}