using System;
using System.Reflection;

namespace KY.Core
{
    public static class AssemblyHelper
    {
        public static Assembly LoadInSameContext(string path)
        {
            Type loaderType = Type.GetType("System.Runtime.Loader.AssemblyLoadContext");
            if (loaderType != null)
            {
                PropertyInfo defaultProperty = loaderType.GetProperty("Default", BindingFlags.Static | BindingFlags.Public);
                if (defaultProperty != null && defaultProperty.CanRead)
                {
                    object defaultContext = defaultProperty.GetMethod.Invoke(null, new object[0]);
                    if (defaultContext != null)
                    {
                        MethodInfo loadMethod = defaultContext.GetType().GetMethod("LoadFromAssemblyPath");
                        if (loadMethod != null)
                        {
                            return (Assembly)loadMethod.Invoke(defaultContext, new object[] { path });
                        }
                    }
                }
            }
            return Assembly.LoadFrom(path);
        }
    }
}