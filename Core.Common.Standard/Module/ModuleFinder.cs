using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using KY.Core.DataAccess;
using KY.Core.Dependency;

namespace KY.Core.Module;

public interface IModuleFinder
{
    IList<ModuleBase> Modules { get; }

    List<ModuleBase> LoadFromAssemblies();
    List<ModuleBase> LoadFrom(Assembly assembly);
    List<ModuleBase> LoadFrom(string path, string moduleFileNameSearchPattern = default);
}

public class ModuleFinder : IModuleFinder
{
    private readonly IDependencyResolver dependencyResolver;
    private readonly Type baseType;
    private readonly List<Type> loadedModules;

    public IList<ModuleBase> Modules { get; }

    public static List<string> ModuleLoadPattern { get; } = new()
    {
        "*Module.dll",
        "*Modules.dll"
    };

    public bool SeparateContext { get; set; }

    public ModuleFinder(IDependencyResolver dependencyResolver)
    {
        this.dependencyResolver = dependencyResolver;
        this.Modules = new List<ModuleBase>();
        this.baseType = typeof(ModuleBase);
        this.loadedModules = new List<Type>();

        this.LoadFromAssemblies();
    }

    public List<ModuleBase> LoadFromAssemblies()
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(this.LoadFrom).ToList();
    }

    public List<ModuleBase> LoadFromApplicationDirectory()
    {
        return this.LoadFrom(FileSystem.Parent(Assembly.GetEntryAssembly().Location));
    }

    public List<ModuleBase> LoadFrom(Assembly assembly)
    {
        List<ModuleBase> newModules = new();
        try
        {
            IEnumerable<Type> types = assembly.GetTypes().Where(x => x != this.baseType && this.baseType.IsAssignableFrom(x) && !x.IsAbstract);
            foreach (Type type in types)
            {
                if (this.loadedModules.Contains(type))
                {
                    continue;
                }

                ModuleBase newModule = (ModuleBase)Activator.CreateInstance(type, this.dependencyResolver);
                this.Modules.Add(newModule);
                this.loadedModules.Add(type);
                newModules.Add(newModule);
            }
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
        }
        return newModules;
    }

    public List<ModuleBase> LoadFrom(string path, string moduleFileNameSearchPattern = default)
    {
        DirectoryInfo directory = new(path);
        if (directory.Exists)
        {
            IEnumerable<FileInfo> areaFileInfos = ModuleLoadPattern.Concat(moduleFileNameSearchPattern.Yield()).Where(x => x != null).SelectMany(pattern => directory.GetFiles(pattern));
            List<string> alreadyLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic).Select(x => Path.GetFileName(x.Location)).ToList();
            IEnumerable<FileInfo> filesToLoad = areaFileInfos.Where(x => alreadyLoadedAssemblies.All(y => !y.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase)));
            return filesToLoad.SelectMany(this.LoadSafeFromFile).ToList();
        }
        FileInfo file = new(path);
        if (file.Exists)
        {
            return this.LoadSafeFromFile(file);
        }
        return new List<ModuleBase>();
    }

    private List<ModuleBase> LoadSafeFromFile(FileInfo file)
    {
        try
        {
            Assembly assembly = null;
            if (!this.SeparateContext)
            {
                assembly = AssemblyLoadContext.Default?.LoadFromAssemblyPath(file.FullName);
            }
            assembly = assembly ?? Assembly.LoadFile(file.FullName);
            return this.LoadFrom(assembly);
        }
        catch
        { }
        return new List<ModuleBase>();
    }
}
