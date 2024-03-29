﻿using System;

namespace KY.Core.Dependency.Syntax
{
    public interface IAfterBindSyntax
    {
        void Named(string name);
    }

    public class AfterBindSyntax : IAfterBindSyntax
    {
        private readonly DependencyResolver resolver;
        private readonly Func<DependencyResolver, object> function;

        public AfterBindSyntax(DependencyResolver resolver, Func<DependencyResolver, object> function)
        {
            this.resolver = resolver;
            this.function = function;
        }

        public void Named(string name)
        {
            this.resolver.Name(this.function, name);
        }
    }
}
