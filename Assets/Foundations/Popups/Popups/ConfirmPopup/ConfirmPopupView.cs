using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Foundations.Popups.Data;
using Foundations.Popups.Views;

namespace Foundations.Popups.Popups.ConfirmPopup
{
    /// <summary>
    /// View for Confirm popup with flexible button configuration
    /// </summary>
    public class ConfirmPopupView : BaseDataPopupView<ConfirmPopupData>
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button okButton;
        [SerializeField] private TextMeshProUGUI yesButtonText;
        [SerializeField] private TextMeshProUGUI noButtonText;
        [SerializeField] private TextMeshProUGUI closeButtonText;
        [SerializeField] private TextMeshProUGUI okButtonText;
        [SerializeField] private Button backgroundButton;
        
        public event Action OnYesClicked;
        public event Action OnNoClicked;
        public event Action OnCloseClicked;
        public event Action OnOkClicked;
        public event Action OnBackgroundClickedEvent;
        
        protected override void Awake()
        {
            base.Awake();
            SetupButtonEvents();
        }
        
        private void SetupButtonEvents()
        {
            if (yesButton)
                yesButton.onClick.AddListener(() => OnYesClicked?.Invoke());
                
            if (noButton)
                noButton.onClick.AddListener(() => OnNoClicked?.Invoke());
                
            if (closeButton)
                closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
                
            if (okButton)
                okButton.onClick.AddListener(() => OnOkClicked?.Invoke());
                
            if (backgroundButton)
                backgroundButton.onClick.AddListener(() => OnBackgroundClickedEvent?.Invoke());
        }
        
        protected override void RefreshUI()
        {
            if (ViewData == null) 
                return;
            
            UpdateTexts();
            UpdateButtonVisibility();
            UpdateButtonTexts();
        }
        
        private void UpdateTexts()
        {
            if (titleText)
                titleText.text = ViewData.title;
                
            if (messageText)
                messageText.text = ViewData.message;
        }
        
        private void UpdateButtonVisibility()
        {
            if (yesButton)
                yesButton.gameObject.SetActive(ViewData.showYesButton);
                
            if (noButton)
                noButton.gameObject.SetActive(ViewData.showNoButton);
                
            if (closeButton)
                closeButton.gameObject.SetActive(ViewData.showCloseButton);
                
            if (okButton)
                okButton.gameObject.SetActive(ViewData.showOkButton);
        }
        
        private void UpdateButtonTexts()
        {
            if (yesButtonText)
                yesButtonText.text = ViewData.yesButtonText;
                
            if (noButtonText)
                noButtonText.text = ViewData.noButtonText;
                
            if (closeButtonText)
                closeButtonText.text = ViewData.closeButtonText;
                
            if (okButtonText)
                okButtonText.text = ViewData.okButtonText;
        }
        
        protected override void OnBackgroundClicked()
        {
            if (ViewData.canCloseOnOutsideClick)
            {
                OnBackgroundClickedEvent?.Invoke();
                Hide();
            }
        }

        protected override void OnDestroy()
        {
            // Clean up button events
            if (yesButton)
                yesButton.onClick.RemoveAllListeners();
                
            if (noButton)
                noButton.onClick.RemoveAllListeners();
                
            if (closeButton)
                closeButton.onClick.RemoveAllListeners();
                
            if (okButton)
                okButton.onClick.RemoveAllListeners();
                
            if (backgroundButton)
                backgroundButton.onClick.RemoveAllListeners();
        }
    }
}
