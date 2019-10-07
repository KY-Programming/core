using System;
using System.Collections.Generic;
using KY.Core.Dependency.Syntax;

namespace KY.Core.Dependency
{
    public interface IDependencyResolver
    {
        T TryGet<T>();
        T Get<T>();
        IBindToSyntax<T> Bind<T>();
        void Bind<TBind, TTo>() where TTo : TBind;
        void Bind<TBind, TTo>(TTo value) where TTo : TBind;
        void BindSingleton<TBind, TTo>() where TTo : TBind;
        void Bind<TBind>(Func<TBind> function);
        object Get(Type type);
        T Create<T>(params object[] arguments);
        object Create(Type type, params object[] arguments);
        bool Contains<T>();
        bool Contains(Type type);
        T TryGet<T>(string name);
        T Get<T>(string name);
    }
}