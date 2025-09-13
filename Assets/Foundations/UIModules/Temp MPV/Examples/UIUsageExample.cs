using UnityEngine;
using UnityEngine.UI;
using UISystem.MVP;

namespace UISystem.MVP.Examples
{
    /// <summary>
    /// Example script demonstrating how to use the MVP UI system
    /// </summary>
    public class UIUsageExample : MonoBehaviour
    {
        [Header("Example Buttons")]
        [SerializeField] private Button showSimpleDialogButton;
        [SerializeField] private Button showConfirmDialogButton;
        [SerializeField] private Button showInputDialogButton;
        [SerializeField] private Button showLoadingPopupButton;
        [SerializeField] private Button showOverlayButton;
        [SerializeField] private Button hideOverlayButton;
        [SerializeField] private Button closeAllPopupsButton;
        
        private string _currentOverlayId;
        private IUIPopupManager _popupManager;
        
        private void Start()
        {
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            if (showSimpleDialogButton != null)
                showSimpleDialogButton.onClick.AddListener(ShowSimpleDialogExample);
                
            if (showConfirmDialogButton != null)
                showConfirmDialogButton.onClick.AddListener(ShowConfirmDialogExample);
                
            if (showInputDialogButton != null)
                showInputDialogButton.onClick.AddListener(ShowInputDialogExample);
                
            if (showLoadingPopupButton != null)
                showLoadingPopupButton.onClick.AddListener(ShowLoadingPopupExample);
                
            if (showOverlayButton != null)
                showOverlayButton.onClick.AddListener(ShowOverlayExample);
                
            if (hideOverlayButton != null)
                hideOverlayButton.onClick.AddListener(HideOverlayExample);
                
            if (closeAllPopupsButton != null)
                closeAllPopupsButton.onClick.AddListener(CloseAllPopupsExample);
        }
        
        private void ShowSimpleDialogExample()
        {
            Debug.Log("Showing Simple Dialog Example");
            
            UISystemManager.Instance.ShowSimpleDialog(
                "Simple Dialog",
                "This is a simple dialog popup example.",
                () => Debug.Log("Simple dialog button clicked!")
            );
        }
        
        private void ShowConfirmDialogExample()
        {
            Debug.Log("Showing Confirm Dialog Example");
            
            UISystemManager.Instance.ShowConfirmDialog(
                "Confirm Action",
                "Are you sure you want to perform this action?",
                () => Debug.Log("User confirmed the action!"),
                () => Debug.Log("User cancelled the action!")
            );
        }
        
        private void ShowInputDialogExample()
        {
            Debug.Log("Showing Input Dialog Example");
            
            var data = new InputDialogData(
                "Enter Your Name",
                "Please enter your name below:",
                "Your name here...",
                "",
                "Submit",
                "Cancel",
                (input) => Debug.Log($"User entered: {input}"),
                () => Debug.Log("User cancelled input dialog")
            );
            
            var presenter = UISystemManager.Instance.PopupCreator.CreatePopup("InputDialog", data);
            
            if (presenter != null)
            {
                var popupId = System.Guid.NewGuid().ToString();
                UISystemManager.Instance.PopupManager.ShowPopup(popupId, "InputDialog", presenter, 0, true);
            }
        }
        
        private void ShowLoadingPopupExample()
        {
            Debug.Log("Showing Loading Popup Example");
            
            UISystemManager.Instance.ShowLoadingPopup("Loading game data...", true);
            
            // Simulate progress update
            StartCoroutine(SimulateLoadingProgress());
        }
        
        private System.Collections.IEnumerator SimulateLoadingProgress()
        {
            yield return new WaitForSeconds(1f);
            
            // Update progress over time
            for (float progress = 0f; progress <= 1f; progress += 0.1f)
            {
                yield return new WaitForSeconds(0.2f);
                
                // Find the loading popup presenter and update progress
                var popupManager = UISystemManager.Instance.PopupManager;
                var allPopups = popupManager.GetAllShownPopups();
                
                foreach (var popup in allPopups)
                {
                    if (popup.popupType == "LoadingPopup" && popup.presenter is LoadingPopupPresenter loadingPresenter)
                    {
                        loadingPresenter.UpdateProgress(progress);
                        break;
                    }
                }
            }
            
            yield return new WaitForSeconds(1f);
            
            // Close the loading popup
            var topPopup = _popupManager.GetTopPopup();
            if (topPopup != null && topPopup.popupType == "LoadingPopup")
            {
                _popupManager.ClosePopup(topPopup.popupId);
            }
        }
        
        private void ShowOverlayExample()
        {
            Debug.Log("Showing Overlay Example");
            
            _currentOverlayId = System.Guid.NewGuid().ToString();
            
            UISystemManager.Instance.OverlayManager.ShowOverlay(
                _currentOverlayId,
                new Color(0, 0, 0, 0.7f),
                true,
                0
            );
            
            // Auto-hide after 3 seconds
            Invoke(nameof(HideOverlayExample), 3f);
        }
        
        private void HideOverlayExample()
        {
            if (!string.IsNullOrEmpty(_currentOverlayId))
            {
                Debug.Log("Hiding Overlay Example");
                UISystemManager.Instance.OverlayManager.HideOverlay(_currentOverlayId, true);
                _currentOverlayId = null;
            }
        }
        
        private void CloseAllPopupsExample()
        {
            Debug.Log("Closing All Popups Example");
            UISystemManager.Instance.PopupManager.CloseAllPopups(true);
        }
        
        private void OnDestroy()
        {
            // Clean up button listeners
            if (showSimpleDialogButton != null)
                showSimpleDialogButton.onClick.RemoveListener(ShowSimpleDialogExample);
                
            if (showConfirmDialogButton != null)
                showConfirmDialogButton.onClick.RemoveListener(ShowConfirmDialogExample);
                
            if (showInputDialogButton != null)
                showInputDialogButton.onClick.RemoveListener(ShowInputDialogExample);
                
            if (showLoadingPopupButton != null)
                showLoadingPopupButton.onClick.RemoveListener(ShowLoadingPopupExample);
                
            if (showOverlayButton != null)
                showOverlayButton.onClick.RemoveListener(ShowOverlayExample);
                
            if (hideOverlayButton != null)
                hideOverlayButton.onClick.RemoveListener(HideOverlayExample);
                
            if (closeAllPopupsButton != null)
                closeAllPopupsButton.onClick.RemoveListener(CloseAllPopupsExample);
        }
    }
}
