using System;

namespace Foundations.Popups.Data
{
    /// <summary>
    /// Data structure for Confirm popup
    /// </summary>
    [Serializable]
    public class ConfirmPopupData : PopupData
    {
        public string title;
        public string message;
        public string yesButtonText;
        public string noButtonText;
        public string closeButtonText;
        public string okButtonText;
        public bool showYesButton;
        public bool showNoButton;
        public bool showCloseButton;
        public bool showOkButton;
        
        public ConfirmPopupData() : base()
        {
            title = "Confirm";
            message = "Are you sure?";
            yesButtonText = "Yes";
            noButtonText = "No";
            closeButtonText = "Close";
            okButtonText = "OK";
            showYesButton = true;
            showNoButton = true;
            showCloseButton = true;
            showOkButton = false;
        }
        
        public ConfirmPopupData(string message, string title = "Confirm") : base()
        {
            this.title = title;
            this.message = message;
            yesButtonText = "Yes";
            noButtonText = "No";
            closeButtonText = "Close";
            okButtonText = "OK";
            showYesButton = true;
            showNoButton = true;
            showCloseButton = true;
            showOkButton = false;
        }
    }
}
