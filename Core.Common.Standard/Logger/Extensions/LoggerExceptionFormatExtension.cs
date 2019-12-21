using System;

namespace KY.Core
{
    public static class LoggerExceptionFormatExtension
    {
        public static string Format(this LoggerExtension extension, Exception exception)
        {
            Type type = exception.GetType();
            IExceptionFormatter formatter = Logger.ExceptionFormatters.ContainsKey(type) ? Logger.ExceptionFormatters[type] : Logger.ExceptionFormatters[typeof(Exception)];
            return formatter.Format(exception);
        }
    }
}