using UnityEngine;
using Cysharp.Threading.Tasks;
using Foundations.UIModules.FlyingRewardSystem.Manager;

namespace Foundations.UIModules.FlyingRewardSystem.Components
{
    public abstract class BaseUIFlyingObject : MonoBehaviour, IUIFlyingObject
    {
        [SerializeField] protected string key = "";
        [SerializeField] protected UIFlyingObjectMovement flyingObjectMovement;
        
        private IUIFlyingRewardManager _flyingRewardManager;
        public string Key => this.key;

        public void InjectManager(IUIFlyingRewardManager manager)
        {
            this._flyingRewardManager = manager;
        }

        public async UniTask MoveToTarget()
        {
            var targetObject = this.FindTargetObject();
            var targetPosition = targetObject.Transform.position;
            
            await this.flyingObjectMovement.PreMoveToTarget();
            await this.flyingObjectMovement.MoveToTarget(targetPosition);
            await this.flyingObjectMovement.PostMoveToTarget();
        }

        public IUITargetObject FindTargetObject()
        {
            return this._flyingRewardManager.GetRewardTargetObject(this.key);
        }
    }
}