using Foundations.UIModules.UIPresenter;

namespace Foundations.UIModules.Popups
{
    public interface IPopupView
    {
        public IUIPresenter Presenter { get; }
    }
}