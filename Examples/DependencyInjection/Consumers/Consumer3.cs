using System;
using DependencyInjection.Services;

namespace DependencyInjection.Consumers
{
    internal class Consumer3
    {
        private readonly FirstService service;
        private readonly string additional;

        // At the moment the order of the paramaters is relevant. Add the custom ones (provided by the Create method) before the ones from the resolver
        public Consumer3(string additional, FirstService service)
        {
            this.service = service;
            this.additional = additional;
        }

        public void Run()
        {
            this.service.Write();
            Console.WriteLine($"Consumer 3 with addition info: {this.additional}");
        }
    }
}