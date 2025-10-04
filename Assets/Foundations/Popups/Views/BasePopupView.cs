using System;
using UnityEngine;
using Foundations.UIModules.UIView;
using Foundations.Popups.Interfaces;
using Foundations.Popups.Data;

namespace Foundations.Popups.Views
{
    /// <summary>
    /// Base class for all popup views without data
    /// Inherits from BaseUIView and implements IPopup
    /// </summary>
    public abstract class BasePopupView : BaseUIView, IPopup
    {
        [Header("Popup Settings")]
        [SerializeField] protected Canvas popupCanvas;
        [SerializeField] protected CanvasGroup popupCanvasGroup;
        [SerializeField] protected GameObject popupPanel;
        [SerializeField] protected bool canCloseOnOutsideClick = true;
        [SerializeField] protected int priority = 0;
        
        public string Id { get; private set; }
        public Type PopupType => GetType();
        public bool IsActive { get; private set; }
        public int Priority => priority;
        public bool CanCloseOnOutsideClick => canCloseOnOutsideClick;
        
        public event Action<IPopup> OnShown;
        public event Action<IPopup> OnHidden;
        public event Action<IPopup> OnDestroyed;
        
        protected virtual void Awake()
        {
            base.Awake();
            Id = Guid.NewGuid().ToString();
            InitializePopup();
        }
        
        protected virtual void InitializePopup()
        {
            // Set initial state
            SetPopupVisibility(false);
        }
        
        public virtual void Show()
        {
            if (IsActive) return;
            
            SetPopupVisibility(true);
            IsActive = true;
            OnShown?.Invoke(this);
        }
        
        public virtual void Hide()
        {
            if (!IsActive) return;
            
            SetPopupVisibility(false);
            IsActive = false;
            OnHidden?.Invoke(this);
        }
        
        public virtual void Destroy()
        {
            Hide();
            OnDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
        
        protected virtual void SetPopupVisibility(bool visible)
        {
            if (popupPanel != null)
                popupPanel.SetActive(visible);
                
            if (popupCanvas != null)
                popupCanvas.enabled = visible;
                
            if (popupCanvasGroup != null)
            {
                popupCanvasGroup.alpha = visible ? 1f : 0f;
                popupCanvasGroup.interactable = visible;
                popupCanvasGroup.blocksRaycasts = visible;
            }
        }
        
        /// <summary>
        /// Override this method to handle outside click detection
        /// </summary>
        protected virtual void OnBackgroundClicked()
        {
            if (canCloseOnOutsideClick)
            {
                Hide();
            }
        }
    }
}
