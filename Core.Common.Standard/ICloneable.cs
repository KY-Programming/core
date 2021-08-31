using System;

namespace KY.Core
{
    [Obsolete("Use ICloneable instead")]
    public interface ICloneable<out T>
    {
        T Clone();
    }
}
