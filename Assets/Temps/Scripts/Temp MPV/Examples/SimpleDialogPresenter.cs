using UnityEngine;

namespace UISystem.MVP.Examples
{
    /// <summary>
    /// Presenter for simple dialog popup
    /// </summary>
    public class SimpleDialogPresenter : BasePresenter<SimpleDialogModel, SimpleDialogView, SimpleDialogData>
    {
        protected override void OnTypedInitialize()
        {
            // Additional initialization if needed
        }
        
        /// <summary>
        /// Handle button click from view
        /// </summary>
        public void OnButtonClicked()
        {
            TypedModel?.OnButtonClicked();
        }
    }

    /// <summary>
    /// Presenter for confirm dialog popup
    /// </summary>
    public class ConfirmDialogPresenter : BasePresenter<ConfirmDialogModel, ConfirmDialogView, ConfirmDialogData>
    {
        protected override void OnTypedInitialize()
        {
            // Additional initialization if needed
        }
        
        /// <summary>
        /// Handle confirm button click from view
        /// </summary>
        public void OnConfirmClicked()
        {
            TypedModel?.OnConfirmClicked();
        }
        
        /// <summary>
        /// Handle cancel button click from view
        /// </summary>
        public void OnCancelClicked()
        {
            TypedModel?.OnCancelClicked();
        }
    }

    /// <summary>
    /// Presenter for input dialog popup
    /// </summary>
    public class InputDialogPresenter : BasePresenter<InputDialogModel, InputDialogView, InputDialogData>
    {
        protected override void OnTypedInitialize()
        {
            // Additional initialization if needed
        }
        
        /// <summary>
        /// Handle input change from view
        /// </summary>
        /// <param name="input">New input value</param>
        public void OnInputChanged(string input)
        {
            if (TypedModel != null)
            {
                TypedModel.CurrentInput = input;
            }
        }
        
        /// <summary>
        /// Handle confirm button click from view
        /// </summary>
        public void OnConfirmClicked()
        {
            TypedModel?.OnConfirmClicked();
        }
        
        /// <summary>
        /// Handle cancel button click from view
        /// </summary>
        public void OnCancelClicked()
        {
            TypedModel?.OnCancelClicked();
        }
    }

    /// <summary>
    /// Presenter for loading popup
    /// </summary>
    public class LoadingPopupPresenter : BasePresenter<LoadingPopupModel, LoadingPopupView, LoadingPopupData>
    {
        protected override void OnTypedInitialize()
        {
            // Additional initialization if needed
        }
        
        /// <summary>
        /// Update progress
        /// </summary>
        /// <param name="progress">Progress value (0-1)</param>
        public void UpdateProgress(float progress)
        {
            TypedModel?.UpdateProgress(progress);
        }
        
        /// <summary>
        /// Update message
        /// </summary>
        /// <param name="message">New message</param>
        public void UpdateMessage(string message)
        {
            TypedModel?.UpdateMessage(message);
        }
    }
}
