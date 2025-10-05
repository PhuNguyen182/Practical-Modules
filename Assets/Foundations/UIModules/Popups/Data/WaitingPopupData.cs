using System;

namespace Foundations.UIModules.Popups.Data
{
    /// <summary>
  
    /// Data structure for Waiting popup
    /// </summary>
    [Serializable]
    public class WaitingPopupData : PopupData
    {
        public string message;
        public float timeoutDuration; // -1 means no timeout
        public bool showProgressBar;
        public bool showCancelButton;
        public string cancelButtonText;
        
        public WaitingPopupData() : base()
        {
            message = "Please wait...";
            timeoutDuration = -1f;
            showProgressBar = false;
            showCancelButton = false;
            cancelButtonText = "Cancel";
            canCloseOnOutsideClick = false; // Waiting popups shouldn't close on outside click
        }
        
        public WaitingPopupData(string message, float timeoutDuration = -1f) : base()
        {
            this.message = message;
            this.timeoutDuration = timeoutDuration;
            showProgressBar = false;
            showCancelButton = false;
            cancelButtonText = "Cancel";
            canCloseOnOutsideClick = false;
        }
    }
}
