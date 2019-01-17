using DependencyInjection.Services;

namespace DependencyInjection.Consumers
{
    internal class Consumer1
    {
        private readonly IFirstService service;

        public Consumer1(IFirstService service)
        {
            this.service = service;
        }

        public void Run()
        {
            this.service.Write();
        }
    }
}