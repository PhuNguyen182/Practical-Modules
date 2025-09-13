using System;

namespace UISystem.MVP.Examples
{
    /// <summary>
    /// Data model for simple dialog popup
    /// </summary>
    [Serializable]
    public class SimpleDialogData
    {
        public string title;
        public string message;
        public string buttonText;
        public Action onButtonClick;
        
        public SimpleDialogData(string title, string message, string buttonText = "OK", Action onButtonClick = null)
        {
            this.title = title;
            this.message = message;
            this.buttonText = buttonText;
            this.onButtonClick = onButtonClick;
        }
    }

    /// <summary>
    /// Data model for confirm dialog popup
    /// </summary>
    [Serializable]
    public class ConfirmDialogData
    {
        public string title;
        public string message;
        public string confirmText;
        public string cancelText;
        public Action onConfirm;
        public Action onCancel;
        
        public ConfirmDialogData(string title, string message, string confirmText = "Yes", string cancelText = "No", Action onConfirm = null, Action onCancel = null)
        {
            this.title = title;
            this.message = message;
            this.confirmText = confirmText;
            this.cancelText = cancelText;
            this.onConfirm = onConfirm;
            this.onCancel = onCancel;
        }
    }

    /// <summary>
    /// Data model for input dialog popup
    /// </summary>
    [Serializable]
    public class InputDialogData
    {
        public string title;
        public string message;
        public string placeholder;
        public string initialValue;
        public string confirmText;
        public string cancelText;
        public Action<string> onConfirm;
        public Action onCancel;
        
        public InputDialogData(string title, string message, string placeholder = "", string initialValue = "", string confirmText = "OK", string cancelText = "Cancel", Action<string> onConfirm = null, Action onCancel = null)
        {
            this.title = title;
            this.message = message;
            this.placeholder = placeholder;
            this.initialValue = initialValue;
            this.confirmText = confirmText;
            this.cancelText = cancelText;
            this.onConfirm = onConfirm;
            this.onCancel = onCancel;
        }
    }

    /// <summary>
    /// Data model for loading popup
    /// </summary>
    [Serializable]
    public class LoadingPopupData
    {
        public string message;
        public bool showProgress;
        public float progress;
        
        public LoadingPopupData(string message = "Loading...", bool showProgress = false, float progress = 0f)
        {
            this.message = message;
            this.showProgress = showProgress;
            this.progress = progress;
        }
    }
}
