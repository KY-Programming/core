using System;
using DependencyInjection.Consumers;
using DependencyInjection.Services;
using KY.Core.Dependency;

namespace DependencyInjection
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DependencyResolver resolver = new DependencyResolver();

            // Bind a type to an interface
            resolver.Bind<IFirstService>().To<FirstService>();

            // Bind a type as ans singleton
            resolver.Bind<SecondService>().ToSingleton();

            // Create an instance of an object with dependency injection
            Consumer1 consumer1 = resolver.Create<Consumer1>();
            consumer1.Run();

            // Resolve an instance by hand
            resolver.Get<SecondService>().Write();

            // Resolve by type
            Consumer2 consumer2 = (Consumer2)resolver.Create(typeof(Consumer2));
            consumer2.Run();

            // Resolve with additional parameters
            Consumer3 consumer3 = resolver.Create<Consumer3>("abcdefg");
            consumer3.Run();

            Console.ReadLine();
        }
    }
}