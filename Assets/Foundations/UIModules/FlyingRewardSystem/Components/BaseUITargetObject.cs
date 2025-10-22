using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Foundations.UIModules.FlyingRewardSystem.Components
{
    public abstract class BaseUITargetObject : MonoBehaviour, IUITargetObject
    {
        [SerializeField] protected string key = "";
        
        public string Key => this.key;
        public Transform Transform => this.transform;
        
        public abstract UniTask ReactOnTargetReached();
    }
}