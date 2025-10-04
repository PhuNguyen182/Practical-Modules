using System;
using UnityEngine;
using Foundations.Popups.Interfaces;
using Foundations.Popups.Core;
using Foundations.Popups.Data;
using Foundations.Popups.Popups.ConfirmPopup;
using Foundations.Popups.Popups.WaitingPopup;

namespace Foundations.Popups.Helpers
{
    /// <summary>
    /// Utility class for common popup operations
    /// Provides static methods for easy popup management
    /// </summary>
    public static class PopupUtility
    {
        private static IPopupManager popupManager;
        
        /// <summary>
        /// Get or find the PopupManager instance
        /// </summary>
        public static IPopupManager PopupManager
        {
            get
            {
                if (popupManager == null)
                {
                    popupManager = UnityEngine.Object.FindObjectOfType<PopupManager>();
                    if (popupManager == null)
                    {
                        Debug.LogError("PopupManager not found in scene! Please add PopupManager to your scene.");
                    }
                }
                return popupManager;
            }
            set => popupManager = value;
        }
        
        /*
        /// <summary>
        /// Show a Yes/No confirmation popup with callback
        /// </summary>
        /// <param name="message">Confirmation message</param>
        /// <param name="onYes">Callback when user clicks Yes</param>
        /// <param name="onNo">Callback when user clicks No</param>
        /// <param name="title">Popup title</param>
        public static void ShowConfirmDialog(string message, Action onYes = null, Action onNo = null, string title = "Confirm")
        {
            var popup = PopupManager?.ShowPopup<ConfirmPopupPresenter, ConfirmPopupData>(new ConfirmPopupData(message, title));
            
            if (popup != null)
            {
                if (onYes != null)
                    popup.OnYesResult += onYes;
                    
                if (onNo != null)
                    popup.OnNoResult += onNo;
            }
        }
        
        /// <summary>
        /// Show an OK dialog popup with callback
        /// </summary>
        /// <param name="message">Dialog message</param>
        /// <param name="onOk">Callback when user clicks OK</param>
        /// <param name="title">Popup title</param>
        public static void ShowInfoDialog(string message, Action onOk = null, string title = "Information")
        {
            var data = new ConfirmPopupData(message, title)
            {
                showYesButton = false,
                showNoButton = false,
                showOkButton = true,
                showCloseButton = true
            };
            
            var popup = PopupManager?.ShowPopup<ConfirmPopupPresenter, ConfirmPopupData>(data);
            
            if (popup != null && onOk != null)
            {
                popup.OnOkResult += onOk;
            }
        }
        
        /// <summary>
        /// Show a waiting popup with timeout
        /// </summary>
        /// <param name="message">Waiting message</param>
        /// <param name="timeoutDuration">Timeout duration (-1 for no timeout)</param>
        /// <param name="onCompleted">Callback when waiting is completed</param>
        /// <param name="onCancel">Callback when user cancels</param>
        public static void ShowWaitingDialog(string message, float timeoutDuration = -1f, Action onCompleted = null, Action onCancel = null)
        {
            var popup = PopupManager?.ShowPopup<WaitingPopupPresenter, WaitingPopupData>(
                new WaitingPopupData(message, timeoutDuration));
            
            if (popup != null)
            {
                if (onCompleted != null)
                    popup.OnWaitingCompleted += onCompleted;
                    
                if (onCancel != null)
                    popup.OnCancelRequested += onCancel;
            }
        }
        
        /// <summary>
        /// Show a blocking waiting popup (covers full screen, no timeout)
        /// </summary>
        /// <param name="message">Waiting message</param>
        /// <param name="onCancel">Callback when user cancels</param>
        public static void ShowBlockingWaitingDialog(string message, Action onCancel = null)
        {
            var data = new WaitingPopupData(message, -1f)
            {
                showCancelButton = true,
                canCloseOnOutsideClick = false
            };
            
            var popup = PopupManager?.ShowPopup<WaitingPopupPresenter, WaitingPopupData>(data);
            
            if (popup != null && onCancel != null)
            {
                popup.OnCancelRequested += onCancel;
            }
        }
        */
        
        /// <summary>
        /// Hide all active popups
        /// </summary>
        public static void HideAllPopups()
        {
            PopupManager?.HideAllPopups();
        }
        
        /// <summary>
        /// Check if any popup is currently active
        /// </summary>
        /// <returns>True if any popup is active</returns>
        public static bool HasActivePopups()
        {
            return PopupManager?.GetActivePopups().Count > 0;
        }
        
        /// <summary>
        /// Check if a specific popup type is active
        /// </summary>
        /// <typeparam name="T">Popup type</typeparam>
        /// <returns>True if popup is active</returns>
        public static bool IsPopupActive<T>() where T : class, IPopup
        {
            return PopupManager?.IsPopupActive<T>() ?? false;
        }
        
        /*
        /// <summary>
        /// Create a simple popup chain (sequence of popups)
        /// </summary>
        /// <param name="messages">Array of messages to show</param>
        /// <param name="onComplete">Callback when all popups are completed</param>
        public static void ShowPopupChain(string[] messages, Action onComplete = null)
        {
            if (messages == null || messages.Length == 0)
            {
                onComplete?.Invoke();
                return;
            }
            
            ShowInfoDialog(messages[0], () =>
            {
                if (messages.Length > 1)
                {
                    var remainingMessages = new string[messages.Length - 1];
                    Array.Copy(messages, 1, remainingMessages, 0, remainingMessages.Length);
                    ShowPopupChain(remainingMessages, onComplete);
                }
                else
                {
                    onComplete?.Invoke();
                }
            });
        }*/
    }
}
