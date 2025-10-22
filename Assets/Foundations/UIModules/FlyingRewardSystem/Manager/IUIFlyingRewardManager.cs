using Foundations.UIModules.FlyingRewardSystem.Components;

namespace Foundations.UIModules.FlyingRewardSystem.Manager
{
    public interface IUIFlyingRewardManager
    {
        public IUITargetObject GetRewardTargetObject(string key);
        public bool RegisterRewardTargetObject(string key, IUITargetObject targetObject);
        public bool UnregisterRewardTargetObject(string key);
    }
}