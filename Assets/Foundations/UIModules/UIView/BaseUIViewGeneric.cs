using System;

namespace Foundations.UIModules.UIView
{
    public abstract class BaseUIView<TViewData>: BaseUIView, IUIView<TViewData>
    {
        public TViewData ViewData { get; private set; }
        public Action<TViewData> OnViewDataUpdated { get; set; }

        public virtual void UpdateData(TViewData viewData)
        {
            ViewData = viewData;
            OnViewDataUpdated?.Invoke(ViewData);
        }
    }
}
