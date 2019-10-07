using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KY.Core
{
    public class ServiceFactoryBase<TCore> : IDisposable
        where TCore : ServiceFactoryBase<TCore>
    {
        #region Fields
        private static TCore _NOTUSE_Instance;
        private readonly KeyedByTypeCollection<object> allServices;
        private static object createMutex = new object();
        #endregion

        #region Properties
        protected static TCore Instance
        {
            get
            {
                lock (createMutex)
                {
                    if (_NOTUSE_Instance == null)
                    {
                        _NOTUSE_Instance = Activator.CreateInstance<TCore>();
                        _NOTUSE_Instance.Initialize();
                    }
                }
                return _NOTUSE_Instance;
            }
        }

        public static ReadOnlyCollection<object> Services
        {
            get { return Instance.ServicesInternal; }
        }

        protected ReadOnlyCollection<object> ServicesInternal
        {
            get { return new ReadOnlyCollection<object>(Instance.allServices); }
        }

        #endregion

        #region Constructor

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht")]
        protected ServiceFactoryBase()
        {
            this.allServices = new KeyedByTypeCollection<object>();
        }

        ~ServiceFactoryBase()
        {
            this.Dispose(false);
        }
        #endregion

        protected virtual void Initialize()
        { }

        protected void Add(object service)
        {
            this.allServices.Add(service);
        }

        public static T Get<T>()
        {
            if (!Instance.allServices.Contains(typeof(T)))
                return default(T);

            return (T)Instance.allServices[typeof(T)];
        }

        public static void Close(object service)
        {
            Instance.allServices.Remove(service);
        }

        #region IDisposable Member

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.allServices != null)
                {
                    foreach (object serivce in this.allServices)
                    {
                        if (serivce is IDisposable)
                            (serivce as IDisposable).Dispose();
                    }
                }
            }
        }

        #endregion
    }
}
