using Second.Module.Services;

namespace Second.Module.Consumers
{
    internal class Consumer2
    {
        private readonly SecondService service;

        public Consumer2(SecondService service)
        {
            this.service = service;
        }

        public void Run()
        {
            this.service.Write();
        }
    }
}