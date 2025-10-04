using System;

namespace Foundations.UIModules.UIView
{
    public abstract class BaseUIView<TViewData>: BaseUIView, IUIView<TViewData>
    {
        public TViewData ViewData { get; private set; }
        public event Action<TViewData> OnUserInteraction;
        public event Action<TViewData> OnViewDataUpdated;

        public virtual void UpdateData(TViewData viewData)
        {
            ViewData = viewData;
            OnViewDataUpdated?.Invoke(ViewData);
        }
        
        /// <summary>
        /// Call this method when user interacts with UI elements
        /// This will notify the Presenter about user actions
        /// </summary>
        protected virtual void NotifyUserInteraction()
        {
            OnUserInteraction?.Invoke(ViewData);
        }
    }
}
