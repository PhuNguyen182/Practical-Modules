using Foundations.UIModules.UIPresenter;
using Foundations.Popups.Interfaces;
using Foundations.Popups.Data;
using Foundations.UIModules.UIView;

namespace Foundations.Popups.Presenters
{
    /// <summary>
    /// Base presenter for popups without data
    /// Manages popup lifecycle and handles events
    /// </summary>
    public abstract class BasePopupPresenter : BaseUIPresenter<PopupData, PopupData>, IUIPresenter
    {
        protected IPopup Popup;
        
        public override IUIView<PopupData> View => Popup as IUIView<PopupData>;
        
        protected override void Awake()
        {
            base.Awake();
            Popup = GetComponent<IPopup>();
        }
        
        public override PopupData ConvertToView(PopupData presenterData)
        {
            return presenterData;
        }
        
        /// <summary>
        /// Show the popup
        /// </summary>
        public virtual void ShowPopup()
        {
            Popup?.Show();
        }
        
        /// <summary>
        /// Hide the popup
        /// </summary>
        public virtual void HidePopup()
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
    }
}
