namespace KY.Core
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}