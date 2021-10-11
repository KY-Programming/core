using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using KY.Core.Clone;

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
                else if (targetProperty.CanRead && IsCloneable(targetProperty.PropertyType))
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
            where T : class
        {
            return Clone(source, true, ignoreProperties);
        }

        public static T Clone<T>(this T source, bool useICloneable, params string[] ignoreProperties)
            where T : class
        {
            return Clone(source, null, useICloneable, ignoreProperties);
        }

        private static T Clone<T>(this T source, CloneStack stack, bool useICloneable = true, params string[] ignoreProperties)
            where T : class
        {
            if (source == null)
            {
                return default;
            }
            stack ??= CloneStack.Continue() ?? new CloneStack();
            if (stack.Mapping.ContainsKey(source))
            {
                return (T)stack.Mapping[source];
            }
            if (useICloneable && source is ICloneable cloneable)
            {
                try
                {
                    stack.Open();
                    stack.Mapping.Add(cloneable, null);
                    T clone = (T)cloneable.Clone();
                    stack.Mapping[cloneable] = clone;
                    List<DelayedSetter> delayedSetters = stack.DelayedSetters.Where(x => x.Source == source).ToList();
                    foreach (DelayedSetter setter in delayedSetters)
                    {
                        setter.Setter.Invoke(setter.Target, new object[] { clone });
                        stack.DelayedSetters.Remove(setter);
                    }
                    return clone;
                }
                finally
                {
                    stack.Close();
                }
            }
            if (!IsCloneable(source))
            {
                return source;
            }
            T target = (T)Activator.CreateInstance(source.GetType());
            stack.Mapping.Add(source, target);
            CloneAndSet(source, target, stack, useICloneable, ignoreProperties);
            return target;
        }

        private static void CloneAndSet(object source, object target, CloneStack stack, bool useICloneable = true, params string[] ignoreProperties)
        {
            if (target is IList list && source is IEnumerable enumerable)
            {
                list.Clear();
                foreach (object entry in enumerable)
                {
                    list.Add(entry?.Clone(stack));
                }
                return;
            }
            IEnumerable<PropertyInfo> properties = source.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                                                         .Where(x => x.CanRead && !ignoreProperties.Contains(x.Name) && x.GetMethod.GetParameters().Length == 0);
            foreach (PropertyInfo property in properties)
            {
                object sourceValue = property.GetMethod.Invoke(source, null);
                object targetValue = property.GetMethod.Invoke(target, null);
                bool isMarkedAsNotCloneable = property.GetCustomAttribute<NotCloneableAttribute>() != null;
                bool isAlreadyCloned = sourceValue != null && stack.Mapping.ContainsKey(sourceValue);
                if (isMarkedAsNotCloneable && property.CanWrite)
                {
                    property.SetMethod.Invoke(target, new[] { sourceValue });
                }
                else if (isMarkedAsNotCloneable && targetValue != null && sourceValue != null)
                {
                    targetValue.SetFrom(sourceValue);
                }
                else if (isAlreadyCloned && property.CanWrite)
                {
                    property.SetMethod.Invoke(target, new[] { stack.Mapping[sourceValue] });
                }
                else if (targetValue != null && sourceValue != null && IsCloneable(targetValue))
                {
                    stack.Mapping.AddIfNotExists(sourceValue, targetValue);
                    CloneAndSet(sourceValue, targetValue, stack, useICloneable, ignoreProperties);
                }
                else if (property.CanWrite)
                {
                    object clone = sourceValue?.Clone(stack, useICloneable, ignoreProperties);
                    if (clone == null)
                    {
                        stack.DelayedSetters.Add(new DelayedSetter(property.SetMethod, sourceValue, target));
                    }
                    else
                    {
                        property.SetMethod.Invoke(target, new[] { clone });
                    }
                }
            }
        }

        private static bool IsCloneable(Type type)
        {
            return !type.IsPrimitive && type != typeof(string);
        }

        private static bool IsCloneable(object obj)
        {
            return obj != null && IsCloneable(obj.GetType());
        }
    }
}
