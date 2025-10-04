using System;

namespace Foundations.UIModules.UIView
{
    public interface IUIView<TViewData> : IUIView, IViewUpdater<TViewData>
    {
        // Events for user interactions with specific data context
        public event Action<TViewData> OnUserInteraction;
        public event Action<TViewData> OnViewDataUpdated;
    }
}
