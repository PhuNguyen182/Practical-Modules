using System;

namespace UISystem.MVP.Examples
{
    /// <summary>
    /// Model for simple dialog popup
    /// </summary>
    public class SimpleDialogModel : BaseModel<SimpleDialogData>
    {
        protected override void OnInitializeData(SimpleDialogData data)
        {
            // Initialize model with dialog data
            if (data != null)
            {
                Data = data;
            }
        }
        
        /// <summary>
        /// Handle button click
        /// </summary>
        public void OnButtonClicked()
        {
            Data?.onButtonClick?.Invoke();
        }
    }

    /// <summary>
    /// Model for confirm dialog popup
    /// </summary>
    public class ConfirmDialogModel : BaseModel<ConfirmDialogData>
    {
        protected override void OnInitializeData(ConfirmDialogData data)
        {
            if (data != null)
            {
                Data = data;
            }
        }
        
        /// <summary>
        /// Handle confirm button click
        /// </summary>
        public void OnConfirmClicked()
        {
            Data?.onConfirm?.Invoke();
        }
        
        /// <summary>
        /// Handle cancel button click
        /// </summary>
        public void OnCancelClicked()
        {
            Data?.onCancel?.Invoke();
        }
    }

    /// <summary>
    /// Model for input dialog popup
    /// </summary>
    public class InputDialogModel : BaseModel<InputDialogData>
    {
        private string _currentInput;
        
        public string CurrentInput
        {
            get => _currentInput;
            set
            {
                _currentInput = value;
                OnDataChanged?.Invoke(Data);
            }
        }
        
        protected override void OnInitializeData(InputDialogData data)
        {
            if (data != null)
            {
                Data = data;
                _currentInput = data.initialValue;
            }
        }
        
        /// <summary>
        /// Handle confirm button click with input
        /// </summary>
        public void OnConfirmClicked()
        {
            Data?.onConfirm?.Invoke(_currentInput);
        }
        
        /// <summary>
        /// Handle cancel button click
        /// </summary>
        public void OnCancelClicked()
        {
            Data?.onCancel?.Invoke();
        }
    }

    /// <summary>
    /// Model for loading popup
    /// </summary>
    public class LoadingPopupModel : BaseModel<LoadingPopupData>
    {
        protected override void OnInitializeData(LoadingPopupData data)
        {
            if (data != null)
            {
                Data = data;
            }
        }
        
        /// <summary>
        /// Update progress
        /// </summary>
        /// <param name="progress">New progress value (0-1)</param>
        public void UpdateProgress(float progress)
        {
            if (Data != null)
            {
                var newData = new LoadingPopupData(Data.message, Data.showProgress, progress);
                UpdateData(newData);
            }
        }
        
        /// <summary>
        /// Update message
        /// </summary>
        /// <param name="message">New message</param>
        public void UpdateMessage(string message)
        {
            if (Data != null)
            {
                var newData = new LoadingPopupData(message, Data.showProgress, Data.progress);
                UpdateData(newData);
            }
        }
    }
}
