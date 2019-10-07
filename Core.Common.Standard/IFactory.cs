namespace KY.Core
{
    public interface IFactory<out T>
    {
        T Create();
    }

    public interface IFactory<out T, in T1>
    {
        T Create(T1 p1);
    }

    public interface IFactory<out T, in T1, in T2>
    {
        T Create(T1 p1, T2 p2);
    }

    public interface IFactory<out T, in T1, in T2, in T3>
    {
        T Create(T1 p1, T2 p2, T3 p3);
    }
}