using System;
using Foundations.Popups.Interfaces;
using Foundations.Popups.Core;
using Foundations.Popups.Data;
using Foundations.Popups.Popups.ConfirmPopup;
using Foundations.Popups.Popups.WaitingPopup;

namespace Foundations.Popups.Helpers
{
    /// <summary>
    /// Extension methods for easier popup management
    /// </summary>
    public static class PopupExtensions
    {
        /// <summary>
        /// Quick access to PopupManager instance
        /// </summary>
        public static IPopupManager PopupManager => UnityEngine.Object.FindObjectOfType<PopupManager>();
        
        /*
        /// <summary>
        /// Show a simple Yes/No confirmation popup
        /// </summary>
        /// <param name="message">Confirmation message</param>
        /// <param name="title">Popup title</param>
        /// <param name="onResult">Callback when user makes a choice</param>
        public static void ShowConfirmPopup(this IPopupManager popupManager, string message, string title = "Confirm", Action<bool> onResult = null)
        {
            var popup = popupManager.ShowPopup<ConfirmPopupPresenter, ConfirmPopupData>(new ConfirmPopupData(message, title));
            
            if (onResult != null)
            {
                popup.OnYesResult += () => onResult(true);
                popup.OnNoResult += () => onResult(false);
                popup.OnCloseResult += () => onResult(false);
            }
        }
        
        /// <summary>
        /// Show a simple OK dialog popup
        /// </summary>
        /// <param name="message">Dialog message</param>
        /// <param name="title">Popup title</param>
        /// <param name="onOk">Callback when user clicks OK</param>
        public static void ShowOkPopup(this IPopupManager popupManager, string message, string title = "Information", Action onOk = null)
        {
            var popup = popupManager.ShowPopup<ConfirmPopupPresenter, ConfirmPopupData>(
                new ConfirmPopupData(message, title) { showYesButton = false, showNoButton = false, showOkButton = true });
            
            if (onOk != null)
            {
                popup.OnOkResult += onOk;
            }
        }
        
        /// <summary>
        /// Show a waiting popup
        /// </summary>
        /// <param name="message">Waiting message</param>
        /// <param name="timeoutDuration">Timeout duration (-1 for no timeout)</param>
        /// <param name="onCompleted">Callback when waiting is completed</param>
        /// <param name="onCancel">Callback when user cancels</param>
        public static void ShowWaitingPopup(this IPopupManager popupManager, string message, float timeoutDuration = -1f, Action onCompleted = null, Action onCancel = null)
        {
            var popup = popupManager.ShowPopup<WaitingPopupPresenter, WaitingPopupData>(
                new WaitingPopupData(message, timeoutDuration));
            
            if (onCompleted != null)
            {
                popup.OnWaitingCompleted += onCompleted;
            }
            
            if (onCancel != null)
            {
                popup.OnCancelRequested += onCancel;
            }
        }
        */
        
        /// <summary>
        /// Show an infinite waiting popup (no timeout)
        /// </summary>
        /// <param name="message">Waiting message</param>
        /// <param name="onCancel">Callback when user cancels</param>
        public static void ShowInfiniteWaitingPopup(this IPopupManager popupManager, string message, Action onCancel = null)
        {
            //popupManager.ShowWaitingPopup(message, -1f, null, onCancel);
        }
        
        /// <summary>
        /// Show a timed waiting popup
        /// </summary>
        /// <param name="message">Waiting message</param>
        /// <param name="timeoutDuration">Timeout duration</param>
        /// <param name="onCompleted">Callback when waiting is completed</param>
        public static void ShowTimedWaitingPopup(this IPopupManager popupManager, string message, float timeoutDuration, Action onCompleted = null)
        {
            //popupManager.ShowWaitingPopup(message, timeoutDuration, onCompleted, null);
        }
    }
}
