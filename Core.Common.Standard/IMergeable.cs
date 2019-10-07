namespace KY.Core
{
    public interface IMergeable
    {
        void Merge(object source);
    }

    public interface IMergeable<in T>
    {
        void Merge(T source);
    }
}