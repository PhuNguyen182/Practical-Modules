using System;
using UnityEngine;

namespace UISystem.MVP
{
    /// <summary>
    /// Base implementation of IView with common functionality
    /// </summary>
    public abstract class BaseView : MonoBehaviour, IView
    {
        public GameObject GameObject => gameObject;
        public RectTransform? RectTransform => transform as RectTransform;
        public bool IsActive => gameObject.activeInHierarchy;
        
        public event Action<IView>? OnViewShown;
        public event Action<IView>? OnViewHidden;
        
        protected bool _isInitialized = false;
        protected bool _disposed = false;
        
        protected virtual void Awake()
        {
            Initialize();
        }
        
        public virtual void Initialize()
        {
            if (_isInitialized) return;
            
            OnInitialize();
            _isInitialized = true;
        }
        
        /// <summary>
        /// Override this method to implement custom initialization logic
        /// </summary>
        protected abstract void OnInitialize();
        
        public virtual void Show()
        {
            SetActive(true);
            OnViewShown?.Invoke(this);
            OnShow();
        }
        
        public virtual void Hide()
        {
            SetActive(false);
            OnViewHidden?.Invoke(this);
            OnHide();
        }
        
        public virtual void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
        public virtual void UpdateView(object data)
        {
            OnUpdateView(data);
        }
        
        /// <summary>
        /// Override this method to implement custom show logic
        /// </summary>
        protected virtual void OnShow() { }
        
        /// <summary>
        /// Override this method to implement custom hide logic
        /// </summary>
        protected virtual void OnHide() { }
        
        /// <summary>
        /// Override this method to implement custom update logic
        /// </summary>
        /// <param name="data">Data to display</param>
        protected abstract void OnUpdateView(object data);
        
        public virtual void Dispose()
        {
            if (_disposed) return;
            
            OnDispose();
            OnViewShown = null;
            OnViewHidden = null;
            _disposed = true;
        }
        
        /// <summary>
        /// Override this method to implement custom disposal logic
        /// </summary>
        protected virtual void OnDispose() { }
        
        protected virtual void OnDestroy()
        {
            Dispose();
        }
    }

    /// <summary>
    /// Generic base view implementation
    /// </summary>
    /// <typeparam name="T">Type of data displayed</typeparam>
    public abstract class BaseView<T> : BaseView, IView<T>
    {
        public event Action<T>? OnViewDataUpdated;
        
        private T _currentData = default!;
        public T CurrentData => _currentData;
        
        public virtual void UpdateView(T data)
        {
            _currentData = data;
            OnViewDataUpdated?.Invoke(data);
            OnUpdateView(data);
        }
        
        protected override void OnUpdateView(object data)
        {
            if (data is T typedData)
            {
                UpdateView(typedData);
            }
        }
        
        /// <summary>
        /// Override this method to implement custom typed update logic
        /// </summary>
        /// <param name="data">Typed data to display</param>
        protected abstract void OnUpdateView(T data);
        
        protected override void OnDispose()
        {
            OnViewDataUpdated = null;
            base.OnDispose();
        }
    }
}
