using System;

namespace Foundations.UIModules.UIView
{
    public interface IUIView
    {
        public void SetInteractable(bool interactable);
        
        // Events for user interactions that Presenter can listen to
        public event Action OnViewInitialized;
        public event Action OnViewDestroyed;
    }
}
