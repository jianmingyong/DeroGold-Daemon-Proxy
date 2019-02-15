using System;
using System.Collections.Generic;
using System.Linq;

namespace DeroGoldRemoteDaemonProxy.DependencyInjection
{
    public sealed class DependencyInjectionManager
    {
        private IEnumerable<IInitializable> InitializableServices { get; }

        private IEnumerable<IDisposable> DisposableServices { get; }

        private bool Initialized { get; set; }

        public DependencyInjectionManager(IServiceProvider serviceProvider)
        {
            InitializableServices = serviceProvider.GetService(typeof(IEnumerable<IInitializable>)) as IEnumerable<IInitializable>;
            DisposableServices = serviceProvider.GetService(typeof(IEnumerable<IDisposable>)) as IEnumerable<IDisposable>;
        }

        public void StartProgramLoop()
        {
            if (Initialized)
                return;

            Initialized = true;

            if (InitializableServices == null)
                return;

            foreach (var initializableService in InitializableServices)
                initializableService.Initialize();
        }

        public void StopProgramLoop()
        {
            if (!Initialized)
                return;

            Initialized = false;

            if (DisposableServices == null)
                return;

            foreach (var disposableService in DisposableServices.Reverse())
                disposableService.Dispose();
        }
    }
}