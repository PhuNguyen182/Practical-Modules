using System;
using Foundations.UIModules.UIView;

namespace Foundations.UIModules.UIPresenter
{
    public interface IPresenterViewConverter<in TPresenterData, out TViewData>
    {
        public TViewData ConvertToView(TPresenterData presenterData);
    }
    
    public interface IUIPresenter<TViewData, TPresenterData> : IUpdatePresenter<TPresenterData>, IPresenterShowable,
        IPresenterViewConverter<TPresenterData, TViewData>, IPresenterHideable
    {
        public void SetActive(bool active);
        public Action<TPresenterData> OnPresenterDataUpdated { get; set; }
        public IUIView<TViewData> View { get; }
    }
}
