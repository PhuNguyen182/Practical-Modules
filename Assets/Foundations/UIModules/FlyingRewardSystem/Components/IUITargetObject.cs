using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Foundations.UIModules.FlyingRewardSystem.Components
{
    public interface IUITargetObject
    {
        public string Key { get; }
        public Transform Transform { get; }
        
        public UniTask ReactOnTargetReached();
    }
}