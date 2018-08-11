using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Logic.DefaultImpl
{
    internal sealed class ServiceHub : IServiceProvider
    {
        private readonly IDictionary<Type, IServiceEntry> _dictionary = new Dictionary<Type, IServiceEntry>();

        internal static ServiceHub Instance { get; }

        static ServiceHub()
        {
            Instance = new ServiceHub();
        }

        [DebuggerStepThrough]
        public TService GetService<TService>()
        {
            return (TService) this.GetService(typeof(TService));
        }

        public TService Set<TService>(TService serviceInstance)
        {
            lock (_dictionary)
            {
                _dictionary[typeof(TService)] = new StaticEntry(serviceInstance);
            }

            return serviceInstance;
        }

        interface IServiceEntry
        {
            object GetInstance();
        }

        class StaticEntry : IServiceEntry
        {
            private readonly object _instance;

            public StaticEntry(object instance)
            {
                _instance = instance;
            }

            #region Implementation of IServiceEntry

            public object GetInstance()
            {
                return _instance;
            }

            #endregion
        }

        #region Implementation of IServiceProvider

        public object GetService(Type serviceType)
        {
            lock (_dictionary)
            {
                if (!_dictionary.TryGetValue(serviceType, out IServiceEntry entry))
                {
                    throw new KeyNotFoundException();
                }

                return entry.GetInstance();
            }
        }

        #endregion

    }
}
