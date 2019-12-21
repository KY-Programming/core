using System;

namespace KY.Core
{
    public interface IExceptionFormatter
    {
        string Format(Exception exception);
    }
}