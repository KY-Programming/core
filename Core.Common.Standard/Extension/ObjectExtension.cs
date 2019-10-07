using System;
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
    }
}