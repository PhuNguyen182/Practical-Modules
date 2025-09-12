using UnityEngine;

namespace PracticalModules.Patterns.ServiceLocator
{
    [AddComponentMenu("ServiceLocator/ServiceLocator Scene")]
    public class ServiceLocatorScene : ServiceLocatorBootstrapper
    {
        protected override void Bootstrap() => Container.ConfigureForScene();
    }
}