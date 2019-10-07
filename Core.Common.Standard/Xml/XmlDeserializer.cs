using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

namespace KY.Core.Xml
{
    public class XmlDeserializer
    {
        private readonly XmlMappingList mapping;

        public XmlDeserializer(XmlMappingList mapping)
        {
            this.mapping = mapping;
        }

        public T Deserialize<T>(XElement element)
        {
            T target = Activator.CreateInstance<T>();
            this.Deserialize(typeof(T), element, target);
            return target;
        }

        public void Deserialize<T>(XElement element, T target)
        {
            this.Deserialize(typeof(T), element, target);
        }

        public object Deserialize(Type type, XElement element)
        {
            if (this.mapping.TryGetType(element, out Type mappedType) && mappedType != type)
            {
                return this.Deserialize(mappedType, element);
            }
            if (type == typeof(string))
            {
                return element.Value;
            }
            if (type.IsValueType)
            {
                return this.DeserializeSimple(type, element.Value);
            }
            object target = this.Create(type, element);
            this.Deserialize(type, element, target);
            return target;
        }

        public void Deserialize(Type type, XElement element, object target)
        {
            if (element == null || target == null)
            {
                return;
            }
            if (type.IsGenericType)
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                Type entryType = type.GetGenericArguments()[0];
                if (typeof(IList).IsAssignableFrom(genericTypeDefinition))
                {
                    IList list = (IList)target;
                    foreach (XElement entryElement in element.Elements())
                    {
                        object entry = this.Deserialize(entryType, entryElement);
                        list.Add(entry);
                    }
                    return;
                }
            }
            IEnumerable<XElement> propertieElements = element.Elements();
            foreach (XElement propertyElement in propertieElements)
            {
                XmlMapping entry = this.mapping.Get(propertyElement);
                string propertyName = entry?.TargetName ?? propertyElement.Name.LocalName;
                PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public);
                if (propertyInfo == null)
                {
                    continue;
                }
                object value = propertyInfo.CanRead ? propertyInfo.GetGetMethod()?.Invoke(target, null) : null;
                if (entry != null && entry.TargetIsList)
                {
                    if (value == null)
                    {
                        this.CheckCanWriteProperty(propertyInfo);
                        value = this.Create(propertyInfo.PropertyType, propertyElement);
                        propertyInfo.GetSetMethod(true).Invoke(target, new[] { value });
                    }
                    Type entryType = entry.TargetType ?? propertyInfo.PropertyType.GetGenericArguments()[0];
                    IList list = (IList)value;
                    list.Add(this.Deserialize(entryType, propertyElement));
                }
                else if (entry?.Constructor != null)
                {
                    this.CheckCanWriteProperty(propertyInfo);
                    value = entry.Constructor.Invoke(propertyElement);
                    propertyInfo.GetSetMethod(true).Invoke(target, new[] { value });
                }
                else
                {
                    Type valueType = entry?.TargetType ?? propertyInfo.PropertyType;
                    if (value == null || valueType.IsValueType)
                    {
                        this.CheckCanWriteProperty(propertyInfo);
                        value = this.Deserialize(valueType, propertyElement);
                        propertyInfo.GetSetMethod(true).Invoke(target, new[] { value });
                    }
                    else
                    {
                        this.Deserialize(valueType, propertyElement, value);
                    }
                }
            }
        }

        [DebuggerHidden]
        private void CheckCanWriteProperty(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite)
            {
                throw new Exception($"Can not write {propertyInfo.DeclaringType?.Name}.{propertyInfo.Name}. Add setter or make it public");
            }
        }

        private object DeserializeSimple(Type type, string value)
        {
            if (type == typeof(short))
            {
                return string.IsNullOrEmpty(value) ? (short?)null :  short.Parse(value);
            }
            if (type == typeof(short?))
            {
                return short.Parse(value);
            }
            if (type == typeof(int))
            {
                return int.Parse(value);
            }
            if (type == typeof(int?))
            {
                return string.IsNullOrEmpty(value) ? (int?)null : int.Parse(value);
            }
            if (type == typeof(long))
            {
                return long.Parse(value);
            }
            if (type == typeof(long?))
            {
                return string.IsNullOrEmpty(value) ? (long?)null :  long.Parse(value);
            }
            if (type == typeof(Guid))
            {
                return Guid.Parse(value);
            }
            if (type == typeof(DateTime))
            {
                return DateTime.Parse(value);
            }
            if (type == typeof(bool))
            {
                return string.IsNullOrEmpty(value) || bool.TrueString.Equals(value, StringComparison.CurrentCultureIgnoreCase);
            }
            throw new NotImplementedException();
        }

        private object Create(Type type, XElement element)
        {
            if (this.mapping.TryGetConstructor(element, out Func<XElement, object> constructor))
            {
                return constructor.Invoke(element);
            }
            if (this.mapping.TryGetConstructor(type, out Func<XElement, object> typeConstructor))
            {
                return typeConstructor.Invoke(element);
            }
            return Activator.CreateInstance(type);
        }
    }
}