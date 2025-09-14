using Foundations.UIModules.UIPresenter;

namespace Foundations.UIModules.Popups
{
    public abstract class BasePopupPresenter<TPopupViewData, TPresenterData> : BaseUIPresenter<TPopupViewData, TPresenterData>
    {
        public int Priority { get; set; }
    }
}
