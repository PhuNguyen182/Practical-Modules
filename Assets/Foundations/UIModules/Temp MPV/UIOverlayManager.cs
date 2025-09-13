using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.MVP
{
    /// <summary>
    /// Implementation of IUIOverlayManager for handling input-blocking overlays
    /// </summary>
    public class UIOverlayManager : MonoBehaviour, IUIOverlayManager
    {
        [Header("Overlay Settings")]
        [SerializeField] private bool enableStacking = true;
        [SerializeField] private int maxOverlayCount = 5;
        
        public event Action<string>? OnOverlayShown;
        public event Action<string>? OnOverlayHidden;
        
        private Dictionary<string, OverlayData> _activeOverlays = new Dictionary<string, OverlayData>();
        private List<OverlayData> _overlayStack = new List<OverlayData>();
        private IUICanvasManager? _canvasManager;
        private bool _initialized = false;
        
        private class OverlayData
        {
            public string overlayId = string.Empty;
            public GameObject? gameObject;
            public Image? image;
            public CanvasGroup? canvasGroup;
            public int priority;
            public bool blockInput;
            public Color color;
        }
        
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
            
            // Ensure overlay canvas exists
            if (!_canvasManager.HasCanvas(UICanvasType.Overlay))
            {
                _canvasManager.CreateCanvas(UICanvasType.Overlay);
            }
            
            _initialized = true;
            Debug.Log("UIOverlayManager initialized");
        }
        
        public void ShowOverlay(string overlayId, Color? color = null, bool blockInput = true, int priority = 0)
        {
            if (!_initialized)
            {
                Debug.LogError("UIOverlayManager not initialized!");
                return;
            }
            
            if (string.IsNullOrEmpty(overlayId))
            {
                Debug.LogError("Overlay ID cannot be null or empty!");
                return;
            }
            
            // Check if overlay already exists
            if (_activeOverlays.ContainsKey(overlayId))
            {
                Debug.LogWarning($"Overlay with ID {overlayId} already exists!");
                return;
            }
            
            // Check max overlay count
            if (_activeOverlays.Count >= maxOverlayCount)
            {
                Debug.LogWarning($"Maximum overlay count ({maxOverlayCount}) reached!");
                return;
            }
            
            // Create overlay
            var overlayData = CreateOverlay(overlayId, color ?? new Color(0, 0, 0, 0.5f), blockInput, priority);
            
            // Add to active overlays
            _activeOverlays[overlayId] = overlayData;
            
            // Add to stack and sort by priority
            _overlayStack.Add(overlayData);
            _overlayStack.Sort((a, b) => b.priority.CompareTo(a.priority));
            
            // Update visual order
            UpdateOverlayVisualOrder();
            
            // Notify event
            OnOverlayShown?.Invoke(overlayId);
            
            Debug.Log($"Shown overlay: {overlayId}");
        }
        
        public void HideOverlay(string overlayId, bool animate = true)
        {
            if (!_activeOverlays.TryGetValue(overlayId, out var overlayData)) return;
            
            // Animate if requested
            if (animate && overlayData.canvasGroup != null)
            {
                StartCoroutine(AnimateOverlayOut(overlayData, () => DestroyOverlay(overlayId)));
            }
            else
            {
                DestroyOverlay(overlayId);
            }
        }
        
        public void HideAllOverlays(bool animate = true)
        {
            var overlayIds = new List<string>(_activeOverlays.Keys);
            
            foreach (var overlayId in overlayIds)
            {
                HideOverlay(overlayId, animate);
            }
        }
        
        public bool IsOverlayShown(string overlayId)
        {
            return _activeOverlays.ContainsKey(overlayId);
        }
        
        public List<string> GetActiveOverlayIds()
        {
            return new List<string>(_activeOverlays.Keys);
        }
        
        public bool IsInputBlocked()
        {
            return _overlayStack.Any(overlay => overlay.blockInput);
        }
        
        public string GetTopOverlay()
        {
            return _overlayStack.FirstOrDefault()?.overlayId;
        }
        
        public void SetOverlayPriority(string overlayId, int priority)
        {
            if (!_activeOverlays.TryGetValue(overlayId, out var overlayData)) return;
            
            overlayData.priority = priority;
            
            // Re-sort stack
            _overlayStack.Sort((a, b) => b.priority.CompareTo(a.priority));
            
            // Update visual order
            UpdateOverlayVisualOrder();
        }
        
        public void Cleanup()
        {
            HideAllOverlays(false);
            _activeOverlays.Clear();
            _overlayStack.Clear();
            _initialized = false;
            
            Debug.Log("UIOverlayManager cleaned up");
        }
        
        private OverlayData? CreateOverlay(string overlayId, Color color, bool blockInput, int priority)
        {
            var overlayCanvas = _canvasManager.GetCanvas(UICanvasType.Overlay);
            if (overlayCanvas == null) return null;
            
            // Create overlay GameObject
            GameObject overlayGO = new GameObject($"Overlay_{overlayId}");
            overlayGO.transform.SetParent(overlayCanvas.transform, false);
            
            // Add RectTransform
            RectTransform rectTransform = overlayGO.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            
            // Add Image component
            Image image = overlayGO.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = blockInput;
            
            // Add CanvasGroup for animations
            CanvasGroup canvasGroup = overlayGO.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = blockInput;
            
            // Animate in
            StartCoroutine(AnimateOverlayIn(canvasGroup));
            
            var overlayData = new OverlayData
            {
                overlayId = overlayId,
                gameObject = overlayGO,
                image = image,
                canvasGroup = canvasGroup,
                priority = priority,
                blockInput = blockInput,
                color = color
            };
            
            return overlayData;
        }
        
        private void DestroyOverlay(string overlayId)
        {
            if (!_activeOverlays.TryGetValue(overlayId, out var overlayData)) return;
            
            // Remove from collections
            _activeOverlays.Remove(overlayId);
            _overlayStack.Remove(overlayData);
            
            // Destroy GameObject
            if (overlayData.gameObject != null)
            {
                Destroy(overlayData.gameObject);
            }
            
            // Notify event
            OnOverlayHidden?.Invoke(overlayId);
            
            Debug.Log($"Hidden overlay: {overlayId}");
        }
        
        private void UpdateOverlayVisualOrder()
        {
            var overlayCanvas = _canvasManager.GetCanvas(UICanvasType.Overlay);
            if (overlayCanvas == null) return;
            
            // Update sort order based on stack position
            for (int i = 0; i < _overlayStack.Count; i++)
            {
                var overlay = _overlayStack[i];
                if (overlay.gameObject != null)
                {
                    overlay.gameObject.transform.SetSiblingIndex(i);
                }
            }
        }
        
        private System.Collections.IEnumerator AnimateOverlayIn(CanvasGroup canvasGroup)
        {
            float duration = 0.2f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
        
        private System.Collections.IEnumerator AnimateOverlayOut(OverlayData overlayData, System.Action onComplete)
        {
            float duration = 0.2f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                overlayData.canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                yield return null;
            }
            
            overlayData.canvasGroup.alpha = 0f;
            onComplete?.Invoke();
        }
        
        private void Awake()
        {
            Initialize();
        }
        
        private void OnDestroy()
        {
            Cleanup();
        }
    }
}
