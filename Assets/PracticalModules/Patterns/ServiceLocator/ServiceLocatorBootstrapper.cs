using UnityEngine;

namespace PracticalModules.Patterns.ServiceLocator
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class ServiceLocatorBootstrapper : MonoBehaviour
    {
        private bool _hasBeenBootstrapped;
        private ServiceLocator _container;

        internal ServiceLocator Container =>
            _container.OrNull() ?? (_container = GetComponent<ServiceLocator>());

        private void Awake() => BootstrapOnDemand();

        public void BootstrapOnDemand()
        {
            if (_hasBeenBootstrapped) 
                return;
            
            _hasBeenBootstrapped = true;
            Bootstrap();
        }

        protected abstract void Bootstrap();
    }
}
