using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UISystem.MVP
{
    /// <summary>
    /// Implementation of IUIPopupManager for handling popup stacking and management
    /// </summary>
    public class UIPopupManager : MonoBehaviour, IUIPopupManager
    {
        [Header("Popup Settings")]
        [SerializeField] private bool enableStacking = true;
        [SerializeField] private int maxPopupCount = 10;
        [SerializeField] private bool autoCloseLowerPriority = false;
        
        public event Action<PopupInfo> OnPopupShown;
        public event Action<PopupInfo> OnPopupHidden;
        public event Action<PopupInfo> OnPopupClosed;
        public event Action<List<PopupInfo>> OnPopupStackChanged;
        
        private Dictionary<string, PopupInfo> _activePopups = new Dictionary<string, PopupInfo>();
        private List<PopupInfo> _popupStack = new List<PopupInfo>();
        private IUICanvasManager _canvasManager;
        private bool _initialized = false;
        
        public void Initialize()
        {
            if (_initialized) return;
            
            _canvasManager = FindObjectOfType<UICanvasManager>();
            if (_canvasManager == null)
            {
                Debug.LogError("UICanvasManager not found! Please ensure it exists in the scene.");
                return;
            }
            
            _canvasManager.Initialize();
            
            // Ensure popup canvas exists
            if (!_canvasManager.HasCanvas(UICanvasType.Popup))
            {
                _canvasManager.CreateCanvas(UICanvasType.Popup);
            }
            
            _initialized = true;
            Debug.Log("UIPopupManager initialized");
        }
        
        public PopupInfo? ShowPopup(string popupId, string popupType, object? data = null, int priority = 0, bool modal = false)
        {
            if (!_initialized)
            {
                Debug.LogError("UIPopupManager not initialized!");
                return null;
            }
            
            if (string.IsNullOrEmpty(popupId))
            {
                Debug.LogError("Popup ID cannot be null or empty!");
                return null;
            }
            
            // Check if popup already exists
            if (_activePopups.ContainsKey(popupId))
            {
                Debug.LogWarning($"Popup with ID {popupId} already exists!");
                return _activePopups[popupId];
            }
            
            // Check max popup count
            if (_activePopups.Count >= maxPopupCount)
            {
                if (autoCloseLowerPriority)
                {
                    CloseLowestPriorityPopup();
                }
                else
                {
                    Debug.LogWarning($"Maximum popup count ({maxPopupCount}) reached!");
                    return null;
                }
            }
            
            // Create popup info
            var popupInfo = new PopupInfo(popupId, popupType, priority, modal, !modal, true, data);
            
            // Add to active popups
            _activePopups[popupId] = popupInfo;
            
            // Add to stack and sort by priority
            _popupStack.Add(popupInfo);
            _popupStack.Sort((a, b) => b.priority.CompareTo(a.priority));
            
            // Show the popup
            ShowPopupInternal(popupInfo);
            
            // Notify events
            OnPopupShown?.Invoke(popupInfo);
            OnPopupStackChanged?.Invoke(new List<PopupInfo>(_popupStack));
            
            Debug.Log($"Shown popup: {popupInfo}");
            return popupInfo;
        }
        
        public void HidePopup(string popupId, bool animate = true)
        {
            if (!_activePopups.TryGetValue(popupId, out var popupInfo)) return;
            
            HidePopupInternal(popupInfo, animate);
            OnPopupHidden?.Invoke(popupInfo);
        }
        
        public void ClosePopup(string popupId, bool animate = true)
        {
            if (!_activePopups.TryGetValue(popupId, out var popupInfo)) return;
            
            HidePopupInternal(popupInfo, animate);
            
            // Remove from collections
            _activePopups.Remove(popupId);
            _popupStack.Remove(popupInfo);
            
            // Notify events
            OnPopupClosed?.Invoke(popupInfo);
            OnPopupStackChanged?.Invoke(new List<PopupInfo>(_popupStack));
            
            // Destroy presenter if needed
            if (popupInfo.destroyOnClose && popupInfo.presenter != null)
            {
                popupInfo.presenter.Dispose();
            }
            
            Debug.Log($"Closed popup: {popupId}");
        }
        
        public void CloseAllPopups(bool animate = true)
        {
            var popupIds = new List<string>(_activePopups.Keys);
            
            foreach (var popupId in popupIds)
            {
                ClosePopup(popupId, animate);
            }
        }
        
        public void CloseAllPopupsExcept(string keepPopupId, bool animate = true)
        {
            var popupIds = new List<string>(_activePopups.Keys);
            
            foreach (var popupId in popupIds)
            {
                if (popupId != keepPopupId)
                {
                    ClosePopup(popupId, animate);
                }
            }
        }
        
        public PopupInfo GetPopupInfo(string popupId)
        {
            return _activePopups.TryGetValue(popupId, out var popupInfo) ? popupInfo : null;
        }
        
        public bool IsPopupShown(string popupId)
        {
            return _activePopups.ContainsKey(popupId);
        }
        
        public List<PopupInfo> GetAllShownPopups()
        {
            return new List<PopupInfo>(_activePopups.Values);
        }
        
        public PopupInfo GetTopPopup()
        {
            return _popupStack.FirstOrDefault();
        }
        
        public List<PopupInfo> GetPopupStack()
        {
            return new List<PopupInfo>(_popupStack);
        }
        
        public void BringPopupToFront(string popupId)
        {
            if (!_activePopups.TryGetValue(popupId, out var popupInfo)) return;
            
            // Find the highest priority and add 1
            int highestPriority = _popupStack.Count > 0 ? _popupStack[0].priority : 0;
            SetPopupPriority(popupId, highestPriority + 1);
        }
        
        public void SetPopupPriority(string popupId, int priority)
        {
            if (!_activePopups.TryGetValue(popupId, out var popupInfo)) return;
            
            popupInfo.priority = priority;
            
            // Re-sort stack
            _popupStack.Sort((a, b) => b.priority.CompareTo(a.priority));
            
            // Update visual order
            UpdatePopupVisualOrder();
            
            OnPopupStackChanged?.Invoke(new List<PopupInfo>(_popupStack));
        }
        
        public void Cleanup()
        {
            CloseAllPopups(false);
            _activePopups.Clear();
            _popupStack.Clear();
            _initialized = false;
            
            Debug.Log("UIPopupManager cleaned up");
        }
        
        private void ShowPopupInternal(PopupInfo popupInfo)
        {
            if (popupInfo.presenter != null)
            {
                popupInfo.presenter.Show();
            }
            
            UpdatePopupVisualOrder();
        }
        
        private void HidePopupInternal(PopupInfo popupInfo, bool animate)
        {
            if (popupInfo.presenter != null)
            {
                popupInfo.presenter.Hide();
            }
        }
        
        private void UpdatePopupVisualOrder()
        {
            var popupCanvas = _canvasManager.GetCanvas(UICanvasType.Popup);
            if (popupCanvas == null) return;
            
            // Update sort order based on stack position
            for (int i = 0; i < _popupStack.Count; i++)
            {
                var popup = _popupStack[i];
                if (popup.presenter?.View?.RectTransform != null)
                {
                    popup.presenter.View.RectTransform.SetSiblingIndex(i);
                }
            }
        }
        
        private void CloseLowestPriorityPopup()
        {
            if (_popupStack.Count == 0) return;
            
            var lowestPriorityPopup = _popupStack.Last();
            ClosePopup(lowestPriorityPopup.popupId, false);
        }
        
        private void Awake()
        {
            Initialize();
        }
        
        private void OnDestroy()
        {
            Cleanup();
        }
        
        // Handle background click for non-modal popups
        public void OnBackgroundClick()
        {
            var topPopup = GetTopPopup();
            if (topPopup != null && topPopup.canCloseOnBackgroundClick && !topPopup.isModal)
            {
                ClosePopup(topPopup.popupId);
            }
        }
    }
}
