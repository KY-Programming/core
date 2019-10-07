using System;

namespace KY.Core
{
    public static class EnumHelper
    {
        public static T Parse<T>(string value, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}