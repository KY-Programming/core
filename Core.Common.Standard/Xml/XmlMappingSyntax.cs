using System;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public interface IXmlMappingCreate
    {
        IXmlMappingAfterCreate Name(string name);
        IXmlMappingAfterCreate Path(string path);
        IXmlMappingAfterCreate Type(Type type);
        IXmlMappingAfterCreate Type<T>();
    }

    public interface IXmlMappingAfterCreate : IXmlMappingAs
    {
        IXmlMappingAs ToList(string name);
        IXmlMappingAs To(string name);
    }

    public interface IXmlMappingAs
    {
        void As(Type type);
        void As<T>();
        void As(Func<XElement, object> action);
    }

    public class XmlMappingSyntax : IXmlMappingCreate, IXmlMappingAfterCreate
    {
        private readonly XmlMappingList mappingList;
        private XmlMapping mapping;

        public XmlMappingSyntax(XmlMappingList mappingList)
        {
            this.mappingList = mappingList;
        }

        public IXmlMappingAfterCreate Name(string name)
        {
            this.mapping = XmlMapping.FromName(name);
            this.mappingList.Add(this.mapping);
            return this;
        }

        public IXmlMappingAfterCreate Path(string path)
        {
            this.mapping = XmlMapping.FromPath(path);
            this.mappingList.Add(this.mapping);
            return this;
        }

        public IXmlMappingAfterCreate Type(Type type)
        {
            this.mapping = XmlMapping.FromType(type);
            this.mappingList.Add(this.mapping);
            return this;
        }

        public IXmlMappingAfterCreate Type<T>()
        {
            return this.Type(typeof(T));
        }

        public void As(Type type)
        {
            this.mapping.TargetType = type;
        }

        public void As<T>()
        {
            this.As(typeof(T));
        }

        public void As(Func<XElement, object> action)
        {
            this.mapping.Constructor = action;
        }

        public IXmlMappingAs ToList(string name)
        {
            this.mapping.TargetIsList = true;
            this.mapping.TargetName = name;
            return this;
        }

        public IXmlMappingAs To(string name)
        {
            this.mapping.TargetName = name;
            return this;
        }
    }
}