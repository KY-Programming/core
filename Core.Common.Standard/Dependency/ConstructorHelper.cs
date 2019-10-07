using System;
using System.Collections.Generic;
using System.Reflection;

namespace KY.Core.Dependency
{
    internal class ConstructorHelper
    {
        private readonly ConstructorInfo info;
        private readonly DependencyResolver dependencyResolver;
        private readonly object[] arguments;

        public ParameterInfo[] Parameters { get; }

        public ConstructorHelper(ConstructorInfo info, DependencyResolver dependencyResolver, object[] arguments = null)
        {
            this.info = info;
            this.dependencyResolver = dependencyResolver;
            this.arguments = arguments ?? new object[0];
            this.Parameters = info.GetParameters();
        }

        public bool CanCreate()
        {
            foreach (ParameterInfo parameterInfo in this.Parameters)
            {
                int index = Array.IndexOf(this.Parameters, parameterInfo);
                if (!this.MatchesArgument(index, parameterInfo) && !this.dependencyResolver.Contains(parameterInfo.ParameterType) && !parameterInfo.IsOptional)
                    return false;
            }
            return true;
        }

        private bool MatchesArgument(int index, ParameterInfo parameterInfo)
        {
            bool matchesArgument = this.arguments?.Length > index && parameterInfo.ParameterType.IsInstanceOfType(this.arguments[index]);
            return matchesArgument;
        }

        public T Create<T>()
        {
            return this.Create().CastTo<T>();
        }

        public object Create()
        {
            List<object> parameters = new List<object>();
            foreach (ParameterInfo parameterInfo in this.Parameters)
            {
                int index = Array.IndexOf(this.Parameters, parameterInfo);
                if (this.MatchesArgument(index, parameterInfo))
                {
                    parameters.Add(this.arguments[index]);
                }
                else
                {
                    parameters.Add(this.dependencyResolver.Get(parameterInfo.ParameterType));
                }

            }
            return this.info.Invoke(parameters.ToArray());
        }
    }
}