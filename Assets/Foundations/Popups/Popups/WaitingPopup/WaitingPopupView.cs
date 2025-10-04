using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Foundations.Popups.Data;
using Foundations.Popups.Views;

namespace Foundations.Popups.Popups.WaitingPopup
{
    /// <summary>
    /// View for Waiting popup with timeout functionality
    /// </summary>
    public class WaitingPopupView : BaseDataPopupView<WaitingPopupData>
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TextMeshProUGUI cancelButtonText;
        [SerializeField] private Image backgroundImage;
        
        [Header("Animation")]
        [SerializeField] private bool showLoadingAnimation = true;
        [SerializeField] private float animationSpeed = 1f;
        
        public event Action OnCancelClicked;
        public event Action OnTimeoutReached;
        
        private float currentTimeoutTimer;
        private bool isTimeoutActive;
        
        protected override void Awake()
        {
            base.Awake();
            SetupButtonEvents();
            InitializeProgressBar();
        }
        
        private void SetupButtonEvents()
        {
            if (cancelButton != null)
                cancelButton.onClick.AddListener(() => OnCancelClicked?.Invoke());
        }
        
        private void InitializeProgressBar()
        {
            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(false);
                progressBar.value = 0f;
            }
        }
        
        protected override void RefreshUI()
        {
            if (ViewData == null) return;
            
            UpdateTexts();
            UpdateButtonVisibility();
            UpdateProgressBar();
            SetupTimeout();
        }
        
        private void UpdateTexts()
        {
            if (messageText != null)
                messageText.text = ViewData.message;
                
            if (cancelButtonText != null)
                cancelButtonText.text = ViewData.cancelButtonText;
        }
        
        private void UpdateButtonVisibility()
        {
            if (cancelButton != null)
                cancelButton.gameObject.SetActive(ViewData.showCancelButton);
        }
        
        private void UpdateProgressBar()
        {
            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(ViewData.showProgressBar);
            }
        }
        
        private void SetupTimeout()
        {
            if (ViewData.timeoutDuration > 0)
            {
                currentTimeoutTimer = ViewData.timeoutDuration;
                isTimeoutActive = true;
            }
            else
            {
                isTimeoutActive = false;
            }
        }
        
        public override void Show()
        {
            base.Show();
            
            if (isTimeoutActive)
            {
                currentTimeoutTimer = ViewData.timeoutDuration;
            }
            
            // Make background cover full screen
            if (backgroundImage != null)
            {
                var rectTransform = backgroundImage.rectTransform;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
        }
        
        private void Update()
        {
            if (!IsActive || !isTimeoutActive) return;
            
            currentTimeoutTimer -= Time.deltaTime;
            
            // Update progress bar if shown
            if (ViewData.showProgressBar && progressBar != null)
            {
                float progress = 1f - (currentTimeoutTimer / ViewData.timeoutDuration);
                progressBar.value = Mathf.Clamp01(progress);
            }
            
            // Check for timeout
            if (currentTimeoutTimer <= 0)
            {
                OnTimeoutReached?.Invoke();
                Hide();
            }
        }
        
        /// <summary>
        /// Update progress bar manually (for non-timeout based progress)
        /// </summary>
        /// <param name="progress">Progress value (0-1)</param>
        public void UpdateProgress(float progress)
        {
            if (progressBar != null)
            {
                progressBar.value = Mathf.Clamp01(progress);
            }
        }
        
        /// <summary>
        /// Set custom message
        /// </summary>
        /// <param name="message">New message</param>
        public void SetMessage(string message)
        {
            if (messageText != null)
                messageText.text = message;
        }
        
        protected override void OnBackgroundClicked()
        {
            // Waiting popups typically don't close on background click
            // This behavior is controlled by ViewData.canCloseOnOutsideClick
            if (ViewData.canCloseOnOutsideClick)
            {
                base.OnBackgroundClicked();
            }
        }
        
        private void OnDestroy()
        {
            // Clean up button events
            if (cancelButton != null)
                cancelButton.onClick.RemoveAllListeners();
        }
    }
}
