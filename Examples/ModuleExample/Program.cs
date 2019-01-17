using System;
using KY.Core;
using KY.Core.Dependency;
using KY.Core.Module;

namespace ModuleExample
{
    // Build everything to the same bin folder, oder reference the assembly
    // Not referenced assemblys are not build automatically from VS. So build by hand or disable option Tools > Options > Projects and Solutions > Build and Run > Only build startup projects and dependencies on Run
    internal class Program
    {
        private static void Main(string[] args)
        {
            DependencyResolver resolver = new DependencyResolver();
            ModuleFinder finder = new ModuleFinder(resolver);
            // Load all assemblies with names like *.Module.dll or *.Modules.dll
            finder.LoadFrom(AppDomain.CurrentDomain.BaseDirectory);
            // If all modules are in the same assembly or all assemblies are referenced, you can use LoadFromAssemblies instead of LoadFrom
            //finder.LoadFromAssemblies();
            finder.Modules.ForEach(module => module.Initialize());

            Console.ReadLine();
        }
    }
}