using Foundations.UIModules.Popups.Data;
using Foundations.UIModules.Popups.Interfaces;
using Foundations.UIModules.UIPresenter;
using Foundations.UIModules.UIView;

namespace Foundations.UIModules.Popups.Presenters
{
    /// <summary>
    /// Base presenter for popups without data
    /// Manages popup lifecycle and handles events
    /// </summary>
    public abstract class BasePopupPresenter : BaseUIPresenter<PopupData, PopupData>
    {
        protected IPopup Popup;
        
        public override IUIView<PopupData> View => Popup as IUIView<PopupData>;
        
        protected override void Awake()
        {
            base.Awake();
            Popup = GetComponent<IPopup>();
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
