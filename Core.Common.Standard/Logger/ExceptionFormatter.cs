using System;
using System.Text;

namespace KY.Core
{
    public sealed class ExceptionFormatter : ExceptionFormatter<Exception>
    { }

    public class ExceptionFormatter<T> : IExceptionFormatter
        where T : Exception
    {
        public string Format(T exception)
        {
            StringBuilder builder = new StringBuilder();
            this.FormatMessage(exception, builder);
            this.FormatStackTrace(exception, builder);
            this.FormatInnerException(exception, builder);
            return builder.ToString();
        }

        protected virtual void BeforeFormatMessage(T exception, StringBuilder builder)
        { }

        protected virtual void FormatMessage(T exception, StringBuilder builder)
        {
            this.BeforeFormatMessage(exception, builder);
            builder.AppendLine(exception.Message);
            this.AfterFormatMessage(exception, builder);
        }

        protected virtual void AfterFormatMessage(T exception, StringBuilder builder)
        { }

        protected virtual void BeforeFormatStackTrace(T exception, StringBuilder builder)
        { }

        protected virtual void FormatStackTrace(T exception, StringBuilder builder)
        {
            this.BeforeFormatStackTrace(exception, builder);
            builder.AppendLine(exception.StackTrace);
            this.AfterFormatStackTrace(exception, builder);
        }

        protected virtual void AfterFormatStackTrace(T exception, StringBuilder builder)
        { }

        protected virtual void BeforeFormatInnerException(T exception, StringBuilder builder)
        { }

        protected virtual void FormatInnerException(T exception, StringBuilder builder)
        {
            this.BeforeFormatInnerException(exception, builder);
            if (exception.InnerException != null)
            {
                builder.AppendLine("   === INNER EXCEPTION ===");
                builder.AppendLine($"   {Logger.Extension.Format(exception.InnerException)}");
            }
            this.AfterFormatInnerException(exception, builder);
        }

        protected virtual void AfterFormatInnerException(T exception, StringBuilder builder)
        { }

        string IExceptionFormatter.Format(Exception exception)
        {
            return this.Format((T)exception);
        }
    }
}