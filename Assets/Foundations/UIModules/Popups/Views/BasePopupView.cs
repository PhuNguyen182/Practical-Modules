using System;
using Foundations.UIModules.Popups.Interfaces;
using Foundations.UIModules.UIView;
using UnityEngine;

namespace Foundations.UIModules.Popups.Views
{
    /// <summary>
    /// Base class for all popup views without data
    /// Inherits from BaseUIView and implements IPopup
    /// </summary>
    public abstract class BasePopupView : BaseUIView, IPopup
    {
        [Header("Popup Settings")]
        [SerializeField] private bool forceDestroy = false;
        [SerializeField] protected Canvas popupCanvas;
        [SerializeField] protected CanvasGroup popupCanvasGroup;
        [SerializeField] protected GameObject popupPanel;
        [SerializeField] protected bool canCloseOnOutsideClick = true;
        [SerializeField] protected int priority = 0;
        
        public string Id { get; private set; }
        public Type PopupType => GetType();
        public bool IsActive { get; private set; }
        public bool ForceDestroy => forceDestroy;
        public int Priority => priority;
        public Transform Transform => transform;
        public bool CanCloseOnOutsideClick => canCloseOnOutsideClick;
        
        public event Action<IPopup> OnShown;
        public event Action<IPopup> OnHidden;
        public event Action<IPopup> OnDestroyed;
        
        protected override void Awake()
        {
            base.Awake();
            Id = Guid.NewGuid().ToString();
            InitializePopup();
        }
        
        protected virtual void InitializePopup()
        {
            // Set initial state
        }
        
        public virtual void Show()
        {
            if (IsActive) return;
            
            IsActive = true;
            OnShown?.Invoke(this);
        }
        
        public virtual void Hide()
        {
            if (!IsActive) return;
            
            ObjectPoolManager.Despawn(this.gameObject);
            IsActive = false;
            OnHidden?.Invoke(this);
        }
        
        public virtual void Destroy()
        {
            Hide();
            OnDestroyed?.Invoke(this);
            Destroy(gameObject);
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
