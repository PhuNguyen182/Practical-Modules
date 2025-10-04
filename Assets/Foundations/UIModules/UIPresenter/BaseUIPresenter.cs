using System;
using Foundations.UIModules.UIView;
using UnityEngine;

namespace Foundations.UIModules.UIPresenter
{
    public abstract class BaseUIPresenter<TPresenterData, TViewData> : MonoBehaviour, 
        IUIPresenter<TViewData, TPresenterData>
    {
        public abstract IUIView<TViewData> View { get; }
        public TPresenterData PresenterData { get; private set; }
        public Action<TPresenterData> OnPresenterDataUpdated { get; set; }
        public Action OnShow { get; set; }
        public Action OnHide { get; set; }
        
        public abstract TViewData ConvertToView(TPresenterData presenterData);

        public void SetActive(bool active) => gameObject.SetActive(active);

        public virtual void Initialize()
        {
            SubscribeToViewEvents();
        }

        public virtual void Dispose()
        {
            UnsubscribeFromViewEvents();
        }

        public virtual void SubscribeToViewEvents()
        {
            if (View != null)
            {
                View.OnViewInitialized += OnViewInitialized;
                View.OnViewDestroyed += OnViewDestroyed;
                
                if (View is { } genericView)
                {
                    genericView.OnUserInteraction += OnUserInteraction;
                }
            }
        }

        public virtual void UnsubscribeFromViewEvents()
        {
            if (View != null)
            {
                View.OnViewInitialized -= OnViewInitialized;
                View.OnViewDestroyed -= OnViewDestroyed;
                
                if (View is { } genericView)
                {
                    genericView.OnUserInteraction -= OnUserInteraction;
                }
            }
        }

        public void UpdatePresenter(TPresenterData presenterData)
        {
            PresenterData = presenterData;
            OnPresenterDataUpdated?.Invoke(PresenterData);
            TViewData viewData = ConvertToView(presenterData);
            View.UpdateData(viewData);
        }

        public virtual void Show()
        {
            OnShow?.Invoke();
        }

        public virtual void Hide()
        {
            OnHide?.Invoke();
        }

        protected virtual void OnViewInitialized()
        {
            // Override in derived classes to handle view initialization
        }

        protected virtual void OnViewDestroyed()
        {
            // Override in derived classes to handle view destruction
        }

        protected virtual void OnUserInteraction(TViewData viewData)
        {
            // Override in derived classes to handle user interactions
            // This is where business logic should be implemented
        }

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }
    }
}
