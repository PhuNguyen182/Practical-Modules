using System;
using UnityEngine;

namespace UISystem.MVP
{
    /// <summary>
    /// Main manager that coordinates all UI system components
    /// </summary>
    public class UISystemManager : MonoBehaviour
    {
        [Header("UI System Components")]
        [SerializeField] private UICanvasManager canvasManager;
        [SerializeField] private UIPopupManager popupManager;
        [SerializeField] private UIOverlayManager overlayManager;
        [SerializeField] private PopupCreator popupCreator;
        
        [Header("System Settings")]
        [SerializeField] private bool initializeOnAwake = true;
        [SerializeField] private bool enableDebugLogging = true;
        
        // Singleton instance
        public static UISystemManager Instance { get; private set; }
        
        // Public access to managers
        public IUICanvasManager CanvasManager => canvasManager;
        public IUIPopupManager PopupManager => popupManager;
        public IUIOverlayManager OverlayManager => overlayManager;
        public IPopupCreator PopupCreator => popupCreator;
        
        // Events
        public event Action OnSystemInitialized;
        public event Action OnSystemShutdown;
        
        private bool _initialized = false;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                if (initializeOnAwake)
                {
                    Initialize();
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initialize the entire UI system
        /// </summary>
        public void Initialize()
        {
            if (_initialized) return;
            
            if (enableDebugLogging)
                Debug.Log("Initializing UI System...");
            
            try
            {
                // Initialize canvas manager first
                if (canvasManager != null)
                {
                    canvasManager.Initialize();
                }
                else
                {
                    Debug.LogError("CanvasManager is not assigned!");
                    return;
                }
                
                // Initialize popup manager
                if (popupManager != null)
                {
                    popupManager.Initialize();
                }
                else
                {
                    Debug.LogError("PopupManager is not assigned!");
                    return;
                }
                
                // Initialize overlay manager
                if (overlayManager != null)
                {
                    overlayManager.Initialize();
                }
                else
                {
                    Debug.LogError("OverlayManager is not assigned!");
                    return;
                }
                
                // Initialize popup creator
                if (popupCreator != null)
                {
                    popupCreator.Initialize();
                }
                else
                {
                    Debug.LogError("PopupCreator is not assigned!");
                    return;
                }
                
                // Setup event connections
                SetupEventConnections();
                
                _initialized = true;
                
                if (enableDebugLogging)
                    Debug.Log("UI System initialized successfully!");
                
                OnSystemInitialized?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize UI System: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Shutdown the UI system
        /// </summary>
        public void Shutdown()
        {
            if (!_initialized) return;
            
            if (enableDebugLogging)
                Debug.Log("Shutting down UI System...");
            
            try
            {
                // Cleanup all managers
                popupCreator?.Cleanup();
                overlayManager?.Cleanup();
                popupManager?.Cleanup();
                canvasManager?.Cleanup();
                
                _initialized = false;
                
                if (enableDebugLogging)
                    Debug.Log("UI System shut down successfully!");
                
                OnSystemShutdown?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to shutdown UI System: {ex.Message}");
            }
        }
        
        private void SetupEventConnections()
        {
            // Connect popup manager events
            if (popupManager != null)
            {
                popupManager.OnPopupShown += OnPopupShown;
                popupManager.OnPopupHidden += OnPopupHidden;
                popupManager.OnPopupClosed += OnPopupClosed;
            }
            
            // Connect overlay manager events
            if (overlayManager != null)
            {
                overlayManager.OnOverlayShown += OnOverlayShown;
                overlayManager.OnOverlayHidden += OnOverlayHidden;
            }
        }
        
        private void OnPopupShown(PopupInfo popupInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"Popup shown: {popupInfo}");
        }
        
        private void OnPopupHidden(PopupInfo popupInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"Popup hidden: {popupInfo}");
        }
        
        private void OnPopupClosed(PopupInfo popupInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"Popup closed: {popupInfo}");
        }
        
        private void OnOverlayShown(string overlayId)
        {
            if (enableDebugLogging)
                Debug.Log($"Overlay shown: {overlayId}");
        }
        
        private void OnOverlayHidden(string overlayId)
        {
            if (enableDebugLogging)
                Debug.Log($"Overlay hidden: {overlayId}");
        }
        
        /// <summary>
        /// Quick access method to show a simple dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <param name="onButtonClick">Button click callback</param>
        public void ShowSimpleDialog(string title, string message, Action onButtonClick = null)
        {
            if (!_initialized)
            {
                Debug.LogError("UI System not initialized!");
                return;
            }
            
            var data = new Examples.SimpleDialogData(title, message, "OK", onButtonClick);
            var presenter = popupCreator.CreatePopup("SimpleDialog", data);
            
            if (presenter != null)
            {
                var popupId = Guid.NewGuid().ToString();
                popupManager.ShowPopup(popupId, "SimpleDialog", presenter, 0, false);
            }
        }
        
        /// <summary>
        /// Quick access method to show a confirm dialog
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <param name="onConfirm">Confirm callback</param>
        /// <param name="onCancel">Cancel callback</param>
        public void ShowConfirmDialog(string title, string message, Action onConfirm = null, Action onCancel = null)
        {
            if (!_initialized)
            {
                Debug.LogError("UI System not initialized!");
                return;
            }
            
            var data = new Examples.ConfirmDialogData(title, message, "Yes", "No", onConfirm, onCancel);
            var presenter = popupCreator.CreatePopup("ConfirmDialog", data);
            
            if (presenter != null)
            {
                var popupId = Guid.NewGuid().ToString();
                popupManager.ShowPopup(popupId, "ConfirmDialog", presenter, 0, true);
            }
        }
        
        /// <summary>
        /// Quick access method to show loading popup
        /// </summary>
        /// <param name="message">Loading message</param>
        /// <param name="showProgress">Whether to show progress</param>
        public void ShowLoadingPopup(string message = "Loading...", bool showProgress = false)
        {
            if (!_initialized)
            {
                Debug.LogError("UI System not initialized!");
                return;
            }
            
            var data = new Examples.LoadingPopupData(message, showProgress, 0f);
            var presenter = popupCreator.CreatePopup("LoadingPopup", data);
            
            if (presenter != null)
            {
                var popupId = Guid.NewGuid().ToString();
                popupManager.ShowPopup(popupId, "LoadingPopup", presenter, 100, true);
            }
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Shutdown();
                Instance = null;
            }
        }
        
        private void OnApplicationQuit()
        {
            Shutdown();
        }
    }
}
