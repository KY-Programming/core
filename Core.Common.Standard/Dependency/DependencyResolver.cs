using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KY.Core.Dependency.Syntax;

namespace KY.Core.Dependency
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly Dictionary<Type, Func<DependencyResolver, object>> dictionary;
        private readonly Dictionary<Type, List<Func<DependencyResolver, object>>> lists;
        private readonly Dictionary<Type, object> singletons;
        private readonly Dictionary<string, Func<DependencyResolver, object>> names;

        public DependencyResolver(IDependencyResolver dependencyResolver = null)
        {
            this.dependencyResolver = dependencyResolver;
            this.dictionary = new Dictionary<Type, Func<DependencyResolver, object>>();
            this.lists = new Dictionary<Type, List<Func<DependencyResolver, object>>>();
            this.singletons = new Dictionary<Type, object>();
            this.names = new Dictionary<string, Func<DependencyResolver, object>>();
            this.Bind<IDependencyResolver>().To(this);
        }

        public IBindToSyntax<T> Bind<T>()
        {
            return new BindToSyntax<T>(this);
        }

        [DebuggerHidden]
        public T TryGet<T>()
        {
            lock (this.dictionary)
            {
                if (this.dictionary.ContainsKey(typeof(T)))
                    return this.dictionary[typeof(T)](this).CastTo<T>();
            }
            if (this.dependencyResolver == null)
                return default;
            return this.dependencyResolver.TryGet<T>();
        }

        [DebuggerHidden]
        public T TryGet<T>(string name)
        {
            if (this.names.ContainsKey(name))
                return (T)this.names[name](this);
            return default;
        }

        [DebuggerHidden]
        public T Get<T>()
        {
            return this.Get(typeof(T)).CastTo<T>();
        }

        [DebuggerHidden]
        public object Get(Type type)
        {
            return this.Get(type, null);
        }

        [DebuggerHidden]
        public object Get(Type type, DependencyResolver resolver)
        {
            lock (this.dictionary)
            {
                if (this.dictionary.ContainsKey(type))
                {
                    return this.dictionary[type](resolver ?? this);
                }
            }
            if (this.dependencyResolver is DependencyResolver resolverInstance)
            {
                return resolverInstance.Get(type, resolver ?? this);
            }
            if (this.dependencyResolver != null)
            {
                return this.dependencyResolver.Get(type);
            }
            Type alternativeType = this.GetAlternativeType(type);
            if (alternativeType != null)
            {
                object entry = this.Get(alternativeType);
                IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(alternativeType));
                list.Add(entry);
                return list;
            }
            throw new InvalidDependencyException(type);
        }

        [DebuggerHidden]
        public T Get<T>(string name)
        {
            if (this.names.ContainsKey(name))
                return (T)this.names[name](this);
            throw new InvalidDependencyException<T>();
        }

        private void Add<TBind>(Func<DependencyResolver, object> function, Type targetType = null)
        {
            lock (this.dictionary)
            {
                Type type = typeof(TBind);
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    type = type.GetGenericArguments()[0];
                    this.dictionary.AddIfNotExists(typeof(IEnumerable<>).MakeGenericType(type), function);
                    this.dictionary.AddIfNotExists(typeof(IList<>).MakeGenericType(type), function);
                    this.dictionary.AddIfNotExists(typeof(List<>).MakeGenericType(type), function);
                    return;
                }
                this.lists.AddIfNotExists(type, () => new List<Func<DependencyResolver, object>>());
                this.lists[type].Add(function);
                if (this.lists[type].Count > 1)
                {
                    this.dictionary.Remove(type);
                }
                else
                {
                    this.dictionary.Add(type, function);
                    this.dictionary.AddIfNotExists(typeof(IEnumerable<TBind>), () => this.CreateFromList<TBind>);
                    this.dictionary.AddIfNotExists(typeof(IList<TBind>), () => resolver => this.CreateFromList<TBind>(resolver).ToList());
                    this.dictionary.AddIfNotExists(typeof(List<TBind>), () => resolver => this.CreateFromList<TBind>(resolver).ToList());
                }
                if (type != targetType && targetType != null)
                {
                    this.dictionary.AddIfNotExists(targetType, function);
                    this.dictionary.AddIfNotExists(typeof(IEnumerable<>).MakeGenericType(targetType), () => this.CreateFromList<TBind>);
                    this.dictionary.AddIfNotExists(typeof(IList<>).MakeGenericType(targetType), () => resolver => this.CreateFromList<TBind>(resolver).ToList());
                    this.dictionary.AddIfNotExists(typeof(List<>).MakeGenericType(targetType), () => resolver => this.CreateFromList<TBind>(resolver).ToList());
                }
            }
        }

        [DebuggerHidden]
        public void Bind<TBind>(Func<TBind> function)
        {
            this.Add<TBind>(_ => function());
        }

        [DebuggerHidden]
        public void Bind<TBind>(Func<IDependencyResolver, TBind> function)
        {
            this.Add<TBind>(resolver => function(resolver));
        }

        [DebuggerHidden]
        void IDependencyResolver.Bind<TBind, TTo>()
        {
            this.Bind<TBind, TTo>();
        }

        internal Func<DependencyResolver, object> Bind<TBind, TTo>() where TTo : TBind
        {
            return this.Bind<TBind>(typeof(TTo));
        }

        internal Func<DependencyResolver, object> Bind<TBind>(Type type)
        {
            object create(DependencyResolver resolver) => resolver.Create(type);
            this.Add<TBind>(create, type);
            return create;
        }

        [DebuggerHidden]
        void IDependencyResolver.Bind<TBind, TTo>(TTo value)
        {
            this.Bind<TBind, TTo>(value);
        }

        internal Func<DependencyResolver, object> Bind<TBind, TTo>(TTo value) where TTo : TBind
        {
            object create(DependencyResolver resolver) => value;
            this.Add<TBind>(create, typeof(TTo));
            return create;
        }

        [DebuggerHidden]
        void IDependencyResolver.BindSingleton<TBind, TTo>()
        {
            this.BindSingleton<TBind, TTo>();
        }

        internal Func<DependencyResolver, object> BindSingleton<TBind, TTo>() where TTo : TBind
        {
            object create(DependencyResolver resolver) => resolver.CreateSingleton<TTo>();
            this.Add<TBind>(create, typeof(TTo));
            return create;
        }

        [DebuggerHidden]
        public T Create<T>(params object[] arguments)
        {
            return this.Create(typeof(T), arguments).CastTo<T>();
        }

        //[DebuggerHidden]
        public object Create(Type type, params object[] arguments)
        {
            if (type.IsAbstract)
                throw new InvalidOperationException("Can not create abstract type " + type.Name);
            if (!type.IsClass)
                throw new InvalidOperationException("Only classes can be created " + type.Name);

            ConstructorHelper constructor = type.GetConstructors().Select(x => new ConstructorHelper(x, this, arguments)).OrderByDescending(x => x.Parameters.Length).FirstOrDefault(x => x.CanCreate());
            if (constructor == null)
                throw new InvalidOperationException("No matching constructor found. Provide default constructor or bind all required parameter types " + type.Name);

            return constructor.Create();
        }

        private T CreateSingleton<T>()
        {
            DependencyResolver current = this;
            while (current != null)
            {
                if (current.singletons.ContainsKey(typeof(T)))
                    return current.singletons[typeof(T)].CastTo<T>();

                current = current.dependencyResolver as DependencyResolver;
            }

            T instance = this.Create<T>();
            this.singletons.Add(typeof(T), instance);
            return instance;
        }

        private IEnumerable<T> CreateFromList<T>(DependencyResolver resolver)
        {
            List<Func<DependencyResolver, object>> list = this.lists[typeof(T)];
            foreach (Func<DependencyResolver, object> action in list)
            {
                yield return (T)action(resolver);
            }
        }

        [DebuggerHidden]
        public bool Contains<T>()
        {
            return this.Contains(typeof(T));
        }

        [DebuggerHidden]
        public bool Contains(Type type)
        {
            Type alternativeType = this.GetAlternativeType(type);
            lock (this.dictionary)
            {
                return this.dictionary.ContainsKey(type)
                       || this.dependencyResolver != null && this.dependencyResolver.Contains(type)
                       || alternativeType != null && this.dictionary.ContainsKey(alternativeType)
                       || this.dependencyResolver != null && alternativeType != null && this.dependencyResolver.Contains(alternativeType);
            }
        }

        internal void Name(Func<DependencyResolver, object> function, string name)
        {
            this.names[name] = function;
        }

        private Type GetAlternativeType(Type type)
        {
            if (!type.IsGenericType)
            {
                return null;
            }
            Type definition = type.GetGenericTypeDefinition();
            if (definition == typeof(List<>) || definition == typeof(IEnumerable<>) || definition == typeof(IList<>))
            {
                return type.GetGenericArguments().First();
            }
            return null;
        }
    }
}
