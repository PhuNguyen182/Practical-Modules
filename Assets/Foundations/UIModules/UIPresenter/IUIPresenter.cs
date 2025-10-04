using System;
using Foundations.UIModules.UIView;

namespace Foundations.UIModules.UIPresenter
{
    /// <summary>
    /// Base interface for all UI Presenters following MVP pattern
    /// Presenter acts as intermediary between View and Model (if any)
    /// </summary>
    public interface IUIPresenter : IPresenterShowable, IPresenterHideable
    {
        public void SetActive(bool active);
        public void Initialize();
        public void Dispose();
    }
    
    /// <summary>
    /// Generic interface for UI Presenters with specific data types
    /// TPresenterData: Data that Presenter manages (business logic)
    /// TViewData: Data format that View understands (UI-specific)
    /// </summary>
    public interface IUIPresenter<TViewData, TPresenterData> : IUIPresenter, IUpdatePresenter<TPresenterData>,
        IPresenterViewConverter<TPresenterData, TViewData>
    {
        public IUIView<TViewData> View { get; }
        public Action<TPresenterData> OnPresenterDataUpdated { get; set; }
        
        /// <summary>
        /// Subscribe to View events for user interactions
        /// This is where Presenter listens to View events
        /// </summary>
        public void SubscribeToViewEvents();
        
        /// <summary>
        /// Unsubscribe from View events to prevent memory leaks
        /// </summary>
        public void UnsubscribeFromViewEvents();
    }
}
