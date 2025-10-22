using UnityEngine;
using Cysharp.Threading.Tasks;
using Foundations.UIModules.FlyingRewardSystem.Manager;

namespace Foundations.UIModules.FlyingRewardSystem.Components
{
    public interface IUIFlyingObject
    {
        public string Key { get; }

        public void InjectManager(IUIFlyingRewardManager manager);
        public UniTask MoveToTarget(Vector3 targetPosition);
        public IUITargetObject FindTargetObject();
    }
}