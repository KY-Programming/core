using System;

namespace KY.Core.Dependency.Syntax
{
    public interface IBindToSyntax<in TBind>
    {
        IAfterBindSyntax To<TTo>() where TTo : TBind;
        IAfterBindSyntax ToSingleton<TTo>() where TTo : TBind;
        IAfterBindSyntax ToSingleton();
        IAfterBindSyntax ToSelf();
        void To(Func<TBind> function);
        IAfterBindSyntax To(TBind value);
        IAfterBindSyntax To(Type type);
    }

    public class BindToSyntax<TBind> : IBindToSyntax<TBind>
    {
        private readonly DependencyResolver resolver;

        public BindToSyntax(DependencyResolver resolver)
        {
            this.resolver = resolver;
        }

        public IAfterBindSyntax To<TTo>() where TTo : TBind
        {
            Func<DependencyResolver, object> function = this.resolver.Bind<TBind, TTo>();
            return new AfterBindSyntax(this.resolver, function);
        }

        public IAfterBindSyntax ToSingleton<TTo>() where TTo : TBind
        {
            Func<DependencyResolver, object> function = this.resolver.BindSingleton<TBind, TTo>();
            return new AfterBindSyntax(this.resolver, function);
        }

        public IAfterBindSyntax ToSingleton()
        {
            return this.ToSingleton<TBind>();
        }

        public IAfterBindSyntax To(TBind value)
        {
            Func<DependencyResolver, object> function = this.resolver.Bind<TBind, TBind>(value);
            return new AfterBindSyntax(this.resolver, function);
        }

        public IAfterBindSyntax ToSelf()
        {
            Func<DependencyResolver, object> function = this.resolver.Bind<TBind, TBind>();
            return new AfterBindSyntax(this.resolver, function);
        }

        public void To(Func<TBind> function)
        {
            this.resolver.Bind(function);
        }

        public IAfterBindSyntax To(Type type)
        {
            Func<DependencyResolver, object> function = this.resolver.Bind<TBind>(type);
            return new AfterBindSyntax(this.resolver, function);
        }
    }
}
