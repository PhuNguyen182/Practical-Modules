using System;
using Foundations.UIModules.UIPresenter;
using UnityEngine;

namespace Foundations.UIModules.UIView
{
    public abstract class BaseUIView : MonoBehaviour, IUIView
    {
        [SerializeField] protected CanvasGroup targetViewGroup;
        
        public void SetInteractable(bool interactable)
            => targetViewGroup.interactable = interactable;
    }
}
