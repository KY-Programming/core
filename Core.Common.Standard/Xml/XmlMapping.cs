using System;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public class XmlMapping
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public Type Type { get; private set; }
        public Type TargetType { get; set; }
        public Func<XElement, object> Constructor { get; set; }
        public string TargetName { get; set; }
        public bool TargetIsList { get; set; }

        public static XmlMapping FromName(string name)
        {
            return new XmlMapping { Name = name };
        }

        public static XmlMapping FromPath(string path)
        {
            return new XmlMapping { Path = path };
        }

        public static XmlMapping FromType<T>()
        {
            return FromType(typeof(T));
        }

        public static XmlMapping FromType(Type type)
        {
            return new XmlMapping { Type = type };
        }
    }
}