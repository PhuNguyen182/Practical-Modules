using System;
using UnityEngine;
using Foundations.Popups.Data;
using Foundations.Popups.Presenters;
using Foundations.UIModules.UIView;

namespace Foundations.Popups.Popups.WaitingPopup
{
    /// <summary>
    /// Presenter for Waiting popup following MVP pattern
    /// Handles timeout logic and coordinates between View and external systems
    /// </summary>
    public class WaitingPopupPresenter : BaseDataPopupPresenter<WaitingPopupData, WaitingPopupData>
    {
        [Header("Waiting Popup Settings")]
        [SerializeField] private WaitingPopupView waitingPopupView;
        
        public override IUIView<WaitingPopupData> View => waitingPopupView;
        
        // Events for external systems to listen to
        public event Action OnWaitingCompleted;
        public event Action OnCancelRequested;
        public event Action OnTimeoutReached;
        public event Action OnWaitingStarted;
        public event Action OnWaitingEnded;
        
        private Coroutine _timeoutCoroutine;
        
        protected override void Awake()
        {
            base.Awake();
            if (waitingPopupView == null)
                waitingPopupView = GetComponent<WaitingPopupView>();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            SubscribeToViewEvents();
        }
        
        public override void Dispose()
        {
            UnsubscribeFromViewEvents();
            StopTimeoutCoroutine();
            base.Dispose();
        }
        
        public override void SubscribeToViewEvents()
        {
            if (waitingPopupView != null)
            {
                waitingPopupView.OnCancelClicked += OnCancelButtonClicked;
                waitingPopupView.OnTimeoutReached += OnTimeoutReachedInternal;
            }
        }
        
        public override void UnsubscribeFromViewEvents()
        {
            if (waitingPopupView != null)
            {
                waitingPopupView.OnCancelClicked -= OnCancelButtonClicked;
                waitingPopupView.OnTimeoutReached -= OnTimeoutReachedInternal;
            }
        }
        
        private void OnCancelButtonClicked()
        {
            Debug.Log("Waiting Popup: Cancel button clicked");
            OnCancelRequested?.Invoke();
            OnWaitingCompleted?.Invoke();
            
            // End waiting and hide popup
            EndWaiting();
            HidePopup();
        }
        
        private void OnTimeoutReachedInternal()
        {
            Debug.Log("Waiting Popup: Timeout reached");
            OnTimeoutReached?.Invoke();
            OnWaitingCompleted?.Invoke();
            
            // End waiting and hide popup
            EndWaiting();
            HidePopup();
        }
        
        protected override void ShowPopup(WaitingPopupData data)
        {
            base.ShowPopup(data);
            StartWaiting();
        }
        
        protected override void HidePopup()
        {
            EndWaiting();
            base.HidePopup();
        }

        private void StartWaiting()
        {
            OnWaitingStarted?.Invoke();
            
            StopTimeoutCoroutine();
            _timeoutCoroutine = StartCoroutine(TimeoutCoroutine());
        }

        private void EndWaiting()
        {
            OnWaitingEnded?.Invoke();
            StopTimeoutCoroutine();
        }
        
        private void StopTimeoutCoroutine()
        {
            if (_timeoutCoroutine != null)
            {
                StopCoroutine(_timeoutCoroutine);
                _timeoutCoroutine = null;
            }
        }

        private System.Collections.IEnumerator TimeoutCoroutine()
        {
            yield return new WaitForSeconds(60);

            // Timeout reached
            OnTimeoutReachedInternal();
        }

        /// <summary>
        /// Show waiting popup with custom message
        /// </summary>
        /// <param name="message">Waiting message</param>
        /// <param name="timeoutDuration">Timeout duration (-1 for no timeout)</param>
        /// <param name="showProgressBar">Show progress bar</param>
        /// <param name="showCancelButton">Show cancel button</param>
        private void ShowWaitingPopup(string message, float timeoutDuration = -1f, bool showProgressBar = false, bool showCancelButton = false)
        {
            var data = new WaitingPopupData
            {
                message = message,
                timeoutDuration = timeoutDuration,
                showProgressBar = showProgressBar,
                showCancelButton = showCancelButton
            };
            
            ShowPopup(data);
        }
        
        /// <summary>
        /// Show infinite waiting popup (no timeout)
        /// </summary>
        /// <param name="message">Waiting message</param>
        /// <param name="showCancelButton">Show cancel button</param>
        public void ShowInfiniteWaitingPopup(string message, bool showCancelButton = true)
        {
            ShowWaitingPopup(message, -1f, false, showCancelButton);
        }
        
        /// <summary>
        /// Show timed waiting popup
        /// </summary>
        /// <param name="message">Waiting message</param>
        /// <param name="timeoutDuration">Timeout duration</param>
        /// <param name="showProgressBar">Show progress bar</param>
        public void ShowTimedWaitingPopup(string message, float timeoutDuration, bool showProgressBar = true)
        {
            ShowWaitingPopup(message, timeoutDuration, showProgressBar);
        }
        
        /// <summary>
        /// Update progress manually
        /// </summary>
        /// <param name="progress">Progress value (0-1)</param>
        public void UpdateProgress(float progress)
        {
            waitingPopupView?.UpdateProgress(progress);
        }
        
        /// <summary>
        /// Update waiting message
        /// </summary>
        /// <param name="message">New message</param>
        public void UpdateMessage(string message)
        {
            waitingPopupView?.SetMessage(message);
        }
        
        /// <summary>
        /// Manually complete waiting
        /// </summary>
        public void CompleteWaiting()
        {
            Debug.Log("Waiting Popup: Manually completed");
            OnWaitingCompleted?.Invoke();
            EndWaiting();
            HidePopup();
        }
    }
}
