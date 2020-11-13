using System;
using System.IO;
using System.Reflection;

namespace KY.Core
{
    public class AssemblyLoadContext
    {
        private static readonly Type type = Type.GetType("System.Runtime.Loader.AssemblyLoadContext");
        private readonly object instance;

        public static AssemblyLoadContext Default { get; } = FromDefault();
        //public event Func<AssemblyLoadContext, AssemblyName, Assembly> Resolving;
        //public event Action<AssemblyLoadContext> Unloading;

        private AssemblyLoadContext(object instance)
        {
            this.instance = instance;
        }

        private static AssemblyLoadContext FromDefault()
        {
            return new AssemblyLoadContext(type?.GetProperty(nameof(Default), BindingFlags.Static | BindingFlags.Public)?.GetMethod?.Invoke(null, null));
        }

        public static AssemblyName GetAssemblyName(string assemblyPath)
        {
            return type?.GetMethod(nameof(GetAssemblyName), BindingFlags.Static | BindingFlags.Public)
                       ?.Invoke(null, new object[] { assemblyPath }) as AssemblyName;
        }

        public static AssemblyLoadContext GetLoadContext(Assembly assembly)
        {
            return type?.GetMethod(nameof(GetLoadContext), BindingFlags.Static | BindingFlags.Public)
                       ?.Invoke(null, new object[] { assembly }) as AssemblyLoadContext;
        }

        public Assembly LoadFromAssemblyName(AssemblyName assemblyName)
        {
            return type?.GetMethod(nameof(this.LoadFromAssemblyName), BindingFlags.Instance | BindingFlags.Public)
                       ?.Invoke(this.instance, new object[] { assemblyName }) as Assembly;
        }

        public Assembly LoadFromAssemblyPath(string assemblyPath)
        {
            return type?.GetMethod(nameof(this.LoadFromAssemblyPath), BindingFlags.Instance | BindingFlags.Public)
                       ?.Invoke(this.instance, new object[] { assemblyPath }) as Assembly;
        }

        public Assembly LoadFromNativeImagePath(string nativeImagePath, string assemblyPath)
        {
            return type?.GetMethod(nameof(this.LoadFromNativeImagePath), BindingFlags.Instance | BindingFlags.Public)
                       ?.Invoke(this.instance, new object[] { nativeImagePath, assemblyPath }) as Assembly;
        }

        public Assembly LoadFromStream(Stream assembly)
        {
            return type?.GetMethod(nameof(this.LoadFromStream), BindingFlags.Instance | BindingFlags.Public)
                       ?.Invoke(this.instance, new object[] { assembly }) as Assembly;
        }

        public Assembly LoadFromStream(Stream assembly, Stream assemblySymbols)
        {
            return type?.GetMethod(nameof(this.LoadFromStream), BindingFlags.Instance | BindingFlags.Public)
                       ?.Invoke(this.instance, new object[] { assembly, assemblySymbols }) as Assembly;
        }

        public void SetProfileOptimizationRoot(string directoryPath)
        {
            type?.GetMethod(nameof(this.SetProfileOptimizationRoot), BindingFlags.Instance | BindingFlags.Public)
                ?.Invoke(this.instance, new object[] { directoryPath });
        }

        public void StartProfileOptimization(string profile)
        {
            type?.GetMethod(nameof(this.StartProfileOptimization), BindingFlags.Instance | BindingFlags.Public)
                ?.Invoke(this.instance, new object[] { profile });
        }

        public void Unload()
        {
            type?.GetMethod(nameof(this.Unload), BindingFlags.Instance | BindingFlags.Public)
                ?.Invoke(this.instance.GetType(), null);
        }
    }
}