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
    }
}
