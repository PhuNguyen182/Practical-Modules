using System;
using UnityEngine;

namespace Foundations.UIModules.UIView
{
    public abstract class BaseUIView : MonoBehaviour, IUIView
    {
        [SerializeField] protected CanvasGroup targetViewGroup;
        
        public event Action OnViewInitialized;
        public event Action OnViewDestroyed;
        
        protected virtual void Awake()
        {
            OnViewInitialized?.Invoke();
        }
        
        protected virtual void OnDestroy()
        {
            OnViewDestroyed?.Invoke();
        }
        
        public void SetInteractable(bool interactable)
            => targetViewGroup.interactable = interactable;
    }
}
