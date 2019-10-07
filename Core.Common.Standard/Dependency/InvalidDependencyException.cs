using System;
using KY.Core.Properties;

namespace KY.Core.Dependency
{
    public class InvalidDependencyException<T> : Exception
    {
        public InvalidDependencyException()
            : base(string.Format(Resources.CanNotResolveDependency, typeof(T).Name))
        { }
    }

    public class InvalidDependencyException : Exception
    {
        public InvalidDependencyException(Type type)
            : base(string.Format(Resources.CanNotResolveDependency, type.Name))
        { }
    }
}