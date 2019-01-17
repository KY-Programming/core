using First.Module.Services;
using KY.Core.Dependency;
using KY.Core.Module;

namespace First.Module
{
    public class FirstModule : ModuleBase
    {
        public FirstModule(IDependencyResolver dependencyResolver)
            : base(dependencyResolver)
        {
            this.DependencyResolver.Bind<IFirstServices>().To<FirstServices>();
        }
    }
}