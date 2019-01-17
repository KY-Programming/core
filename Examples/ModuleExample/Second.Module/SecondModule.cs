using KY.Core.Dependency;
using KY.Core.Module;
using Second.Module.Consumers;
using Second.Module.Services;

namespace Second.Module
{
    public class SecondModule : ModuleBase
    {
        public SecondModule(IDependencyResolver dependencyResolver)
            : base(dependencyResolver)
        { }

        public override void Initialize()
        {
            base.Initialize();
            this.DependencyResolver.Bind<SecondService>().ToSingleton();

            Consumer2 consumer2 = this.DependencyResolver.Create<Consumer2>();
            consumer2.Run();
        }
    }
}