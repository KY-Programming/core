using KY.Core.Dependency;

namespace KY.Core.Module
{
    public abstract class ModuleBase
    {
        protected IDependencyResolver DependencyResolver { get; }

        protected ModuleBase(IDependencyResolver dependencyResolver)
        {
            this.DependencyResolver = dependencyResolver;
        }

        public virtual void Initialize()
        { }
    }
}