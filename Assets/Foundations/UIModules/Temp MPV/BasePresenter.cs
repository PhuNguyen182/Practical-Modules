using System;
using UnityEngine;

namespace UISystem.MVP
{
    /// <summary>
    /// Base implementation of IPresenter with common functionality
    /// </summary>
    public abstract class BasePresenter : IPresenter
    {
        public IModel? Model { get; protected set; }
        public IView? View { get; protected set; }
        public bool IsInitialized { get; protected set; }
        
        public event Action<IPresenter>? OnPresenterInitialized;
        
        protected bool _disposed = false;
        
        public virtual void Initialize(IModel model, IView view)
        {
            if (IsInitialized)
            {
                Debug.LogWarning("Presenter is already initialized");
                return;
            }
            
            if (model == null || view == null)
            {
                Debug.LogError("Model and View cannot be null");
                return;
            }
            
            Model = model;
            View = view;
            
            SubscribeToEvents();
            OnInitialize();
            
            IsInitialized = true;
            OnPresenterInitialized?.Invoke(this);
        }
        
        /// <summary>
        /// Override this method to implement custom initialization logic
        /// </summary>
        protected abstract void OnInitialize();
        
        /// <summary>
        /// Subscribe to model and view events
        /// </summary>
        protected virtual void SubscribeToEvents()
        {
            if (Model != null)
            {
                Model.OnModelChanged += OnModelChanged;
            }
            
            if (View != null)
            {
                View.OnViewShown += OnViewShown;
                View.OnViewHidden += OnViewHidden;
            }
        }
        
        /// <summary>
        /// Unsubscribe from model and view events
        /// </summary>
        protected virtual void UnsubscribeFromEvents()
        {
            if (Model != null)
            {
                Model.OnModelChanged -= OnModelChanged;
            }
            
            if (View != null)
            {
                View.OnViewShown -= OnViewShown;
                View.OnViewHidden -= OnViewHidden;
            }
        }
        
        protected virtual void OnModelChanged(IModel model)
        {
            View?.UpdateView(model);
        }
        
        protected virtual void OnViewShown(IView view) { }
        protected virtual void OnViewHidden(IView view) { }
        
        public virtual void Show()
        {
            View?.Show();
        }
        
        public virtual void Hide()
        {
            View?.Hide();
        }
        
        public virtual void UpdatePresenter(object data)
        {
            View?.UpdateView(data);
        }
        
        public virtual void Dispose()
        {
            if (_disposed) return;
            
            UnsubscribeFromEvents();
            OnDispose();
            
            Model?.Dispose();
            View?.Dispose();
            
            Model = null;
            View = null;
            
            OnPresenterInitialized = null;
            _disposed = true;
        }
        
        /// <summary>
        /// Override this method to implement custom disposal logic
        /// </summary>
        protected virtual void OnDispose() { }
    }

    /// <summary>
    /// Generic base presenter implementation
    /// </summary>
    /// <typeparam name="TModel">Type of the model</typeparam>
    /// <typeparam name="TView">Type of the view</typeparam>
    /// <typeparam name="TData">Type of data handled</typeparam>
    public abstract class BasePresenter<TModel, TView, TData> : BasePresenter, IPresenter<TModel, TView, TData>
        where TModel : class, IModel<TData>
        where TView : class, IView<TData>
    {
        public TModel? TypedModel => Model as TModel;
        public TView? TypedView => View as TView;
        
        public virtual void Initialize(TModel model, TView view)
        {
            Initialize(model as IModel, view as IView);
        }
        
        protected override void OnInitialize()
        {
            if (TypedModel != null)
            {
                TypedModel.OnDataChanged += OnDataChanged;
            }
            
            if (TypedView != null)
            {
                TypedView.OnViewDataUpdated += OnViewDataUpdated;
            }
            
            OnTypedInitialize();
        }
        
        /// <summary>
        /// Override this method to implement custom typed initialization logic
        /// </summary>
        protected abstract void OnTypedInitialize();
        
        protected virtual void OnDataChanged(TData data)
        {
            TypedView?.UpdateView(data);
        }
        
        protected virtual void OnViewDataUpdated(TData data) { }
        
        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
            
            if (TypedModel != null)
            {
                TypedModel.OnDataChanged -= OnDataChanged;
            }
            
            if (TypedView != null)
            {
                TypedView.OnViewDataUpdated -= OnViewDataUpdated;
            }
        }
    }
}
