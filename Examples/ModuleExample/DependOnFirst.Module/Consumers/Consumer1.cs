using System;
using First.Module.Services;

namespace DependOnFirst.Module.Consumers
{
    internal class Consumer1
    {
        private readonly IFirstServices services;

        public Consumer1(IFirstServices services)
        {
            this.services = services;
        }

        public void Run()
        {
            Console.WriteLine("Call FirstService from Consumer1");
            this.services.Write();
        }
    }
}