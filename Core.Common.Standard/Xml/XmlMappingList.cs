using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public class XmlMappingList : Collection<XmlMapping>
    {
        private readonly Dictionary<string, XmlMapping> mapping;

        public XmlMappingList()
        {
            this.mapping = new Dictionary<string, XmlMapping>();
        }

        protected override void ClearItems()
        {
            this.mapping.Clear();
            base.ClearItems();
        }

        protected override void InsertItem(int index, XmlMapping item)
        {
            if (!string.IsNullOrEmpty(item.Name))
            {
                this.mapping.Add(item.Name, item);
            }
            if (!string.IsNullOrEmpty(item.Path))
            {
                this.mapping.Add(item.Path, item);
            }
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            XmlMapping item = this[index];
            if (!string.IsNullOrEmpty(item.Name))
            {
                this.mapping.Remove(item.Name);
            }
            if (!string.IsNullOrEmpty(item.Path))
            {
                this.mapping.Remove(item.Path);
            }
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, XmlMapping item)
        {
            if (!string.IsNullOrEmpty(item.Name))
            {
                this.mapping[item.Name] = item;
            }
            if (!string.IsNullOrEmpty(item.Path))
            {
                this.mapping[item.Path] = item;
            }
            base.SetItem(index, item);
        }

        public bool TryGetType(XElement element, out Type type)
        {
            string path = element.GetPath();
            if (this.mapping.ContainsKey(path))
            {
                type = this.mapping[path].TargetType;
                return type != null;
            }
            string name = element.Name.LocalName;
            if (this.mapping.ContainsKey(name))
            {
                type = this.mapping[name].TargetType;
                return type != null;
            }
            type = null;
            return false;
        }
        

        public XmlMapping Get(XElement element)
        {
            return this.TryGet(element, out XmlMapping entry) ? entry : null;
        }

        public bool TryGet(XElement element, out XmlMapping entry)
        {
            string path = element.GetPath();
            if (this.mapping.ContainsKey(path))
            {
                entry = this.mapping[path];
                return true;
            }
            string name = element.Name.LocalName;
            if (this.mapping.ContainsKey(name))
            {
                entry = this.mapping[name];
                return true;
            }
            entry = null;
            return false;
        }

        public bool TryGetConstructor(XElement element, out Func<XElement, object> constructor)
        {
            string path = element.GetPath();
            if (this.mapping.ContainsKey(path))
            {
                constructor =  this.mapping[path].Constructor;
                return constructor != null;
            }
            string name = element.Name.LocalName;
            if (this.mapping.ContainsKey(name))
            {
                constructor = this.mapping[name].Constructor;
                return constructor != null;
            }
            constructor = null;
            return false;
        }

        public bool TryGetConstructor(Type type, out Func<XElement, object> constructor)
        {
            constructor = this.FirstOrDefault(x => x.Type == type)?.Constructor;
            return constructor != null;
        }
    }
}