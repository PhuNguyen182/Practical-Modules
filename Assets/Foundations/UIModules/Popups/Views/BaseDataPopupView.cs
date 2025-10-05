using System;
using Foundations.UIModules.Popups.Data;
using Foundations.UIModules.Popups.Interfaces;
using Foundations.UIModules.UIView;
using UnityEngine;

namespace Foundations.UIModules.Popups.Views
{
    /// <summary>
    /// Base class for popup views that can receive data
    /// Inherits from BaseUIViewGeneric and implements IPopup<TData>
    /// </summary>
    /// <typeparam name="TData">Type of data this popup can receive</typeparam>
    public abstract class BaseDataPopupView<TData> : BaseUIView<TData>, IPopup<TData>
        where TData : PopupData
    {
        [Header("Popup Settings")]
        [SerializeField] private bool forceDestroy = false;
        [SerializeField] protected Canvas popupCanvas;
        [SerializeField] protected CanvasGroup popupCanvasGroup;
        [SerializeField] protected GameObject popupPanel;
        [SerializeField] protected bool canCloseOnOutsideClick = true;
        [SerializeField] protected int priority = 0;
        
        public bool IsActive { get; private set; }
        public bool CanCloseOnOutsideClick => canCloseOnOutsideClick;
        public string Id { get; private set; }
        public bool ForceDestroy => forceDestroy;
        public int Priority => priority;
        public Transform Transform => transform;
        public Type PopupType => GetType();
        public TData Data => ViewData;
        
        public event Action<IPopup> OnShown;
        public event Action<IPopup> OnHidden;
        public event Action<IPopup> OnDestroyed;
        public event Action<IPopup<TData>, TData> OnDataUpdated;
        
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
            if (IsActive) 
                return;
            
            IsActive = true;
            OnShown?.Invoke(this);
        }
        
        public virtual void Hide()
        {
            if (!IsActive) 
                return;
            
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
        
        public override void UpdateData(TData data)
        {
            base.UpdateData(data);
            OnDataUpdated?.Invoke(this, data);
            RefreshUI();
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
        
        /// <summary>
        /// Override this method to refresh UI when data changes
        /// </summary>
        protected abstract void RefreshUI();
    }
}
