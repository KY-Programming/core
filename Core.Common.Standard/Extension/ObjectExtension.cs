using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace KY.Core
{
    public static class ObjectExtension
    {
        [DebuggerHidden]
        public static T CastTo<T>(this object obj)
        {
            return (T)obj;
        }

        [DebuggerHidden]
        public static T CastSafeTo<T>(this object obj)
            where T : class
        {
            return obj as T;
        }

        [DebuggerHidden]
        public static void SetDefaults(this object target, object defaults, params string[] ignoreProperties)
        {
            if (target == null || defaults == null)
            {
                return;
            }
            Type defaultsType = defaults.GetType();
            IEnumerable<PropertyInfo> properties = target.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public)
                                                         .Where(x => x.CanRead && x.CanWrite && !ignoreProperties.Contains(x.Name));
            foreach (PropertyInfo property in properties)
            {
                PropertyInfo defaultsProperty = defaultsType.GetProperty(property.Name, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
                if (defaultsProperty == null || !defaultsProperty.CanRead)
                {
                    continue;
                }
                MethodInfo getMethod = defaultsProperty.GetGetMethod();
                if (getMethod.GetParameters().Length != 0)
                {
                    continue;
                }
                object defaultValue = getMethod.Invoke(defaults, null);
                object value = property.GetGetMethod().Invoke(target, null);
                if (value == property.PropertyType.Default())
                {
                    property.GetSetMethod().Invoke(target, new[] { defaultValue });
                }
                else
                {
                    value.SetDefaults(defaultValue, ignoreProperties);
                }
            }
        }

        [DebuggerHidden]
        public static void SetFrom(this object target, object source, params string[] ignoreProperties)
        {
            if (target == null || source == null)
            {
                return;
            }
            Type targetType = target.GetType();
            Type sourceType = source.GetType();
            IEnumerable<PropertyInfo> sourceProperties = sourceType.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                                                                   .Where(x => x.CanRead && !ignoreProperties.Contains(x.Name));
            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                PropertyInfo targetProperty = targetType.GetProperty(sourceProperty.Name, /*BindingFlags.GetProperty | BindingFlags.SetProperty |*/ BindingFlags.Instance | BindingFlags.Public);
                if (targetProperty == null)
                {
                    continue;
                }
                object sourceValue = sourceProperty.GetGetMethod().Invoke(source, null);
                if (targetProperty.CanWrite)
                {
                    targetProperty.GetSetMethod().Invoke(target, new[] { sourceValue });
                }
                else if (targetProperty.CanRead && typeof(IList).IsAssignableFrom(targetProperty.PropertyType))
                {
                    IList list = (IList)targetProperty.GetGetMethod().Invoke(target, null);
                    list.Clear();
                    if (sourceValue is IEnumerable enumerable)
                    {
                        foreach (object entry in enumerable)
                        {
                            list.Add(entry);
                        }
                    }
                }
                else if (targetProperty.CanRead && !targetProperty.PropertyType.IsValueType)
                {
                    object targetValue = targetProperty.GetGetMethod().Invoke(target, null);
                    if (targetValue != null && sourceValue != null)
                    {
                        targetValue.SetFrom(sourceValue);
                    }
                }
            }
        }

        public static T Clone<T>(this T source, params string[] ignoreProperties)
        {
            return Clone(source, true, ignoreProperties);
        }

        public static T Clone<T>(this T source, bool useICloneable, params string[] ignoreProperties)
        {
            if (useICloneable && source is ICloneable cloneable)
            {
                return (T)cloneable.Clone();
            }
            if (source == null)
            {
                return default;
            }
            T target = (T)Activator.CreateInstance(source.GetType());
            CloneAndSet(source, target, ignoreProperties);
            return target;
        }

        private static void CloneAndSet(object source, object target, params string[] ignoreProperties)
        {
            IEnumerable<PropertyInfo> properties = source.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                                                         .Where(x => x.CanRead && !ignoreProperties.Contains(x.Name));
            foreach (PropertyInfo property in properties)
            {
                object sourceValue = property.GetGetMethod().Invoke(source, null);
                object targetValue = property.GetGetMethod().Invoke(target, null);
                if (targetValue != null && sourceValue != null && !targetValue.GetType().IsPrimitive && targetValue is not string)
                {
                    if (targetValue is IList list)
                    {
                        list.Clear();
                        if (sourceValue is IEnumerable enumerable)
                        {
                            foreach (object entry in enumerable)
                            {
                                list.Add(entry?.Clone());
                            }
                        }
                    }
                    else
                    {
                        CloneAndSet(sourceValue, targetValue, ignoreProperties);
                    }
                }
                else if (property.CanWrite)
                {
                    property.GetSetMethod().Invoke(target, new[] { sourceValue?.Clone() });
                }
            }
        }
    }
}
