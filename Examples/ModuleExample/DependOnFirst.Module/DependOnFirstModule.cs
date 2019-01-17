using DependOnFirst.Module.Consumers;
using KY.Core.Dependency;
using KY.Core.Module;

namespace DependOnFirst.Module
{
    public class DependOnFirstModule : ModuleBase
    {
        public DependOnFirstModule(IDependencyResolver dependencyResolver)
            : base(dependencyResolver)
        { }

        public override void Initialize()
        {
            base.Initialize();
            Consumer1 consumer1 = this.DependencyResolver.Create<Consumer1>();
            consumer1.Run();
        }
    }
}