using System;

namespace KY.Core
{
    [Obsolete("Use System.ICloneable instead")]
    public interface ICloneable<out T>
    {
        T Clone();
    }
}
