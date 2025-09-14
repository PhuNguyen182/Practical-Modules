using System;
using Foundations.UIModules.UIView;

namespace Foundations.UIModules.UIPresenter
{
    public interface IUIPresenter :  IPresenterShowable, IPresenterHideable
    {
        public void SetActive(bool active);
    }
    
    public interface IUIPresenter<TViewData, TPresenterData> : IUIPresenter, IUpdatePresenter<TPresenterData>,
        IPresenterViewConverter<TPresenterData, TViewData>
    {
        public IUIView<TViewData> View { get; }
        public Action<TPresenterData> OnPresenterDataUpdated { get; set; }
    }
}
