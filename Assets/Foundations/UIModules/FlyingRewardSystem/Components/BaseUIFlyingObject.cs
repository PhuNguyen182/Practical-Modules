using UnityEngine;
using Cysharp.Threading.Tasks;
using Foundations.UIModules.FlyingRewardSystem.Manager;

namespace Foundations.UIModules.FlyingRewardSystem.Components
{
    public abstract class BaseUIFlyingObject : MonoBehaviour, IUIFlyingObject
    {
        [SerializeField] protected string key = "";
        
        private IUIFlyingRewardManager _flyingRewardManager;
        public string Key => this.key;

        public void InjectManager(IUIFlyingRewardManager manager)
        {
            this._flyingRewardManager = manager;
        }

        public abstract UniTask MoveToTarget(Vector3 targetPosition);
        
        public IUITargetObject FindTargetObject()
        {
            return this._flyingRewardManager.GetRewardTargetObject(this.key);
        }
    }
}