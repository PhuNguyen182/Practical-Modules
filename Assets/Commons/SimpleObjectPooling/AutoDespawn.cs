using UnityEngine;
using PracticalModules.PlayerLoopServices.Core.Handlers;
using PracticalModules.PlayerLoopServices.UpdateServices;

namespace PracticalModules.Patterns.SimpleObjectPooling
{
    /// <summary>
    /// This component is used to return to the object pool if it is spawned from an object pool.
    /// Attach this component in to the game object need to be despawned automatically.
    /// </summary>
    public class AutoDespawn : MonoBehaviour, IUpdateHandler
    {
        [SerializeField] private float duration = 1;

        private float _timer;

        private void OnEnable()
        {
            _timer = 0;
            UpdateServiceManager.RegisterUpdateHandler(this);
        }

        public void Tick(float deltaTime)
        {
            _timer += deltaTime;
            
            if (_timer >= duration)
            {
                _timer = 0;
                ObjectPoolManager.Despawn(this.gameObject);
            }
        }

        public void SetDuration(float despawnDuration) => this.duration = despawnDuration;

        private void OnDisable() => UpdateServiceManager.DeregisterUpdateHandler(this);
    }
}
