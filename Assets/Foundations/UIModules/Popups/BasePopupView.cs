using Foundations.UIModules.UIPresenter;
using Foundations.UIModules.UIView;

namespace Foundations.UIModules.Popups
{
    public abstract class BasePopupView<TPopupViewData> : BaseUIView<TPopupViewData>, IPopupView
    {
        public abstract IUIPresenter Presenter { get; }
    }
}
