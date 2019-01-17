using System;

namespace DependencyInjection.Services
{
    internal interface IFirstService
    {
        void Write();
    }

    internal class FirstService : IFirstService
    {
        public void Write()
        {
            Console.WriteLine("Hello from FirstService");
        }
    }
}