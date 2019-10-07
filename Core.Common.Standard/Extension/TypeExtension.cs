using System;
using System.Collections.Generic;
using System.Linq;

namespace KY.Core
{
    public static class TypeExtension
    {
        public static object Default(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static IEnumerable<Type> GetInterfaces(this Type type, bool includeInherited)
        {
            if (includeInherited || type.BaseType == null)
                return type.GetInterfaces();
            return type.GetInterfaces().Except(type.BaseType.GetInterfaces().Concat(type.GetInterfaces().SelectMany(x => x.GetInterfaces())));
        }
    }
}