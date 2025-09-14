using System;
using Foundations.UIModules.UIPresenter;
using UnityEngine;

namespace Foundations.UIModules.UIView
{
    public abstract class BaseUIView : MonoBehaviour, IUIView
    {
        [SerializeField] protected CanvasGroup targetViewGroup;
        
        public Action OnViewShow { get; set; }
        public Action OnViewHide { get; set; }

        public virtual void Show()
        {
            OnViewShow?.Invoke();
        }

        public virtual void Hide()
        {
            OnViewHide?.Invoke();
        }

        public void SetActive(bool active)
            => gameObject.SetActive(active);

        public void SetInteractable(bool interactable)
            => targetViewGroup.interactable = interactable;
    }
}
