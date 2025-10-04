using Foundations.UIModules.UIPresenter;
using Foundations.Popups.Interfaces;
using Foundations.Popups.Data;
using Foundations.UIModules.UIView;

namespace Foundations.Popups.Presenters
{
    /// <summary>
    /// Base presenter for popups with data
    /// Manages popup lifecycle, data handling, and events
    /// </summary>
    /// <typeparam name="TPresenterData">Data type managed by presenter</typeparam>
    /// <typeparam name="TViewData">Data type understood by view</typeparam>
    public abstract class BaseDataPopupPresenter<TPresenterData, TViewData> : BaseUIPresenter<TPresenterData, TViewData>, IUIPresenter<TViewData, TPresenterData>
        where TPresenterData : PopupData
        where TViewData : PopupData
    {
        protected IPopup<TViewData> Popup;
        
        public override IUIView<TViewData> View => Popup as IUIView<TViewData>;
        
        protected override void Awake()
        {
            base.Awake();
            Popup = GetComponent<IPopup<TViewData>>();
        }
        
        public override TViewData ConvertToView(TPresenterData presenterData)
        {
            // Default conversion - can be overridden in derived classes
            return presenterData as TViewData;
        }
        
        /// <summary>
        /// Show the popup with data
        /// </summary>
        /// <param name="data">Data to show in the popup</param>
        protected virtual void ShowPopup(TPresenterData data)
        {
            UpdatePresenter(data);
            Popup?.Show();
        }
        
        /// <summary>
        /// Show the popup with current data
        /// </summary>
        public virtual void ShowPopup()
        {
            Popup?.Show();
        }
        
        /// <summary>
        /// Hide the popup
        /// </summary>
        protected virtual void HidePopup()
        {
            Popup?.Hide();
        }
        
        /// <summary>
        /// Destroy the popup
        /// </summary>
        public virtual void DestroyPopup()
        {
            Popup?.Destroy();
        }
        
        /// <summary>
        /// Update popup data
        /// </summary>
        /// <param name="data">New data</param>
        public virtual void UpdatePopupData(TPresenterData data)
        {
            UpdatePresenter(data);
        }
    }
}
