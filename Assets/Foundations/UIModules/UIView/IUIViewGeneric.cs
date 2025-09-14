using System;
using Foundations.UIModules.UIPresenter;

namespace Foundations.UIModules.UIView
{
    public interface IUIView<TViewData> : IUIView, IViewUpdater<TViewData>
    {
        public Action<TViewData> OnViewDataUpdated { get; set; }
    }
}
