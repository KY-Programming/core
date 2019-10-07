using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using KY.Core.DataAccess;
using KY.Core.Dependency;

namespace KY.Core.Module
{
    public interface IModuleFinder
    {
        IList<ModuleBase> Modules { get; }

        void LoadFromAssemblies();
        void LoadFrom(Assembly assembly);
        void LoadFrom(string path);
    }

    public class ModuleFinder : IModuleFinder
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly Type baseType;
        private readonly List<Type> loadedModules;

        public IList<ModuleBase> Modules { get; }

        public ModuleFinder(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
            this.Modules = new List<ModuleBase>();
            this.baseType = typeof(ModuleBase);
            this.loadedModules = new List<Type>();

            this.LoadFromAssemblies();
        }

        public void LoadFromAssemblies()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                this.LoadFrom(assembly);
            }
        }

        public void LoadFromApplicationDirectory()
        {
            this.LoadFrom(FileSystem.Parent(Assembly.GetEntryAssembly().Location));
        }

        public void LoadFrom(Assembly assembly)
        {
            try
            {
                IEnumerable<Type> types = assembly.GetTypes().Where(x => x != this.baseType && this.baseType.IsAssignableFrom(x) && !x.IsAbstract);
                foreach (Type type in types)
                {
                    if (this.loadedModules.Contains(type))
                        continue;

                    this.Modules.Add((ModuleBase)Activator.CreateInstance(type, this.dependencyResolver));
                    this.loadedModules.Add(type);
                }
            }
            catch
            { }
        }

        public void LoadFrom(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            if (directory.Exists)
            {
                IEnumerable<FileInfo> areaFileInfos = directory.GetFiles("*Module.dll").Concat(directory.GetFiles("*Modules.dll"));
                foreach (FileInfo areaFileInfo in areaFileInfos)
                {
                    this.LoadSafeFromFile(areaFileInfo);
                }
                return;
            }
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                this.LoadSafeFromFile(file);
            }
        }

        private void LoadSafeFromFile(FileInfo file)
        {
            try
            {
                Assembly areaAssembly = Assembly.LoadFile(file.FullName);
                this.LoadFrom(areaAssembly);
            }
            catch
            { }
        }
    }

    //internal static class PluginLocator
    //{
    //    public static IEnumerable<Type> Locate<TPlugin>(string path, string searchPattern)
    //    {
    //        AppDomain domain = AppDomain.CreateDomain("PluginLocator");
    //        Type instanceType = typeof(Instance<TPlugin>);
    //        Instance<TPlugin> instance = (Instance<TPlugin>)domain.CreateInstanceAndUnwrap(instanceType.Assembly.GetName().Name, instanceType.FullName);
    //        IEnumerable<Type> result = instance.Locate(path, searchPattern);
    //        AppDomain.Unload(domain);
    //        return result;
    //    }

    //    private class Instance<TPlugin> : MarshalByRefObject
    //    {
    //        public IEnumerable<Type> Locate(string path, string searchPattern)
    //        {
    //            Type baseType = typeof(TPlugin);
    //            string[] files = Directory.GetFiles(path, searchPattern);
    //            List<Type> list = new List<Type>();
    //            foreach (string file in files)
    //            {
    //                Assembly.LoadFrom(file)
    //                        .GetTypes()
    //                        .Where(baseType.IsAssignableFrom)
    //                        .ForEach(list.Add);
    //            }
    //            return list;
    //        }
    //    }
    //}
}