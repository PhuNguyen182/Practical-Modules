using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Foundations.UIModules.FlyingRewardSystem.Components
{
    public class UIFlyingObjectMovement : MonoBehaviour
    {
        public async UniTask PreMoveToTarget(Vector3 aroundPosition = default, float duration = 0)
        {
            await UniTask.CompletedTask;
        }
        
        public async UniTask MoveToTarget(Vector3 targetPosition)
        {
            // To do: Use DOTween to move the object to the target position
            await UniTask.CompletedTask;
        }
        
        public async UniTask PostMoveToTarget()
        {
            await UniTask.CompletedTask;
        }
    }
}