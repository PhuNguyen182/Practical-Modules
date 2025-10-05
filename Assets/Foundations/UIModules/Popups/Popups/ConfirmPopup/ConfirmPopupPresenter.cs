using System;
using Foundations.UIModules.Popups.Data;
using Foundations.UIModules.Popups.Presenters;
using Foundations.UIModules.UIView;
using UnityEngine;

namespace Foundations.UIModules.Popups.Popups.ConfirmPopup
{
    /// <summary>
    /// Presenter for Confirm popup following MVP pattern
    /// Handles business logic and coordinates between View and external systems
    /// </summary>
    public class ConfirmPopupPresenter : BaseDataPopupPresenter<ConfirmPopupData, ConfirmPopupData>
    {
        [Header("Confirm Popup Settings")]
        [SerializeField] private ConfirmPopupView confirmPopupView;
        
        public override IUIView<ConfirmPopupData> View => confirmPopupView;
        
        // Events for external systems to listen to
        public event Action OnConfirmResult;
        public event Action OnYesResult;
        public event Action OnNoResult;
        public event Action OnCloseResult;
        public event Action OnOkResult;
        
        protected override void Awake()
        {
            base.Awake();
            if (confirmPopupView == null)
                confirmPopupView = GetComponent<ConfirmPopupView>();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            SubscribeToViewEvents();
        }
        
        public override void Dispose()
        {
            UnsubscribeFromViewEvents();
            base.Dispose();
        }
        
        private void SubscribeToViewEvents()
        {
            if (confirmPopupView != null)
            {
                confirmPopupView.OnYesClicked += OnYesButtonClicked;
                confirmPopupView.OnNoClicked += OnNoButtonClicked;
                confirmPopupView.OnCloseClicked += OnCloseButtonClicked;
                confirmPopupView.OnOkClicked += OnOkButtonClicked;
                confirmPopupView.OnBackgroundClickedEvent += OnBackgroundClicked;
            }
        }
        
        private void UnsubscribeFromViewEvents()
        {
            if (confirmPopupView != null)
            {
                confirmPopupView.OnYesClicked -= OnYesButtonClicked;
                confirmPopupView.OnNoClicked -= OnNoButtonClicked;
                confirmPopupView.OnCloseClicked -= OnCloseButtonClicked;
                confirmPopupView.OnOkClicked -= OnOkButtonClicked;
                confirmPopupView.OnBackgroundClickedEvent -= OnBackgroundClicked;
            }
        }
        
        private void OnYesButtonClicked()
        {
            Debug.Log("Confirm Popup: Yes button clicked");
            OnYesResult?.Invoke();
            OnConfirmResult?.Invoke();
            
            // Hide popup after user interaction
            HidePopup();
        }
        
        private void OnNoButtonClicked()
        {
            Debug.Log("Confirm Popup: No button clicked");
            OnNoResult?.Invoke();
            OnConfirmResult?.Invoke();
            
            // Hide popup after user interaction
            HidePopup();
        }
        
        private void OnCloseButtonClicked()
        {
            Debug.Log("Confirm Popup: Close button clicked");
            OnCloseResult?.Invoke();
            OnConfirmResult?.Invoke();
            
            // Hide popup after user interaction
            HidePopup();
        }
        
        private void OnOkButtonClicked()
        {
            Debug.Log("Confirm Popup: OK button clicked");
            OnOkResult?.Invoke();
            OnConfirmResult?.Invoke();
            
            // Hide popup after user interaction
            HidePopup();
        }
        
        private void OnBackgroundClicked()
        {
            Debug.Log("Confirm Popup: Background clicked");
            // Background click handling is already done in View
            // Just hide the popup
            HidePopup();
        }
        
        /// <summary>
        /// Show confirm popup with custom data
        /// </summary>
        /// <param name="title">Popup title</param>
        /// <param name="message">Popup message</param>
        /// <param name="showYesNo">Show Yes/No buttons</param>
        /// <param name="showOk">Show OK button</param>
        /// <param name="showClose">Show Close button</param>
        public void ShowConfirmPopup(string title, string message, bool showYesNo = true, bool showOk = false, bool showClose = true)
        {
            var data = new ConfirmPopupData
            {
                title = title,
                message = message,
                showYesButton = showYesNo,
                showNoButton = showYesNo,
                showOkButton = showOk,
                showCloseButton = showClose
            };
            
            ShowPopup(data);
        }
        
        /// <summary>
        /// Show simple Yes/No confirmation
        /// </summary>
        /// <param name="message">Confirmation message</param>
        /// <param name="title">Popup title</param>
        public void ShowYesNoPopup(string message, string title = "Confirm")
        {
            ShowConfirmPopup(title, message, true, false, true);
        }
        
        /// <summary>
        /// Show simple OK dialog
        /// </summary>
        /// <param name="message">Dialog message</param>
        /// <param name="title">Popup title</param>
        public void ShowOkPopup(string message, string title = "Information")
        {
            ShowConfirmPopup(title, message, false, true, true);
        }
    }
}
