using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.MVP
{
    /// <summary>
    /// Implementation of IUICanvasManager for handling UI canvases
    /// </summary>
    public class UICanvasManager : MonoBehaviour, IUICanvasManager
    {
        [Header("Canvas Settings")]
        [SerializeField] private CanvasConfig[] defaultCanvasConfigs = new CanvasConfig[]
        {
            new CanvasConfig(UICanvasType.Main, 0, true, "MainCanvas"),
            new CanvasConfig(UICanvasType.Popup, 100, false, "PopupCanvas"),
            new CanvasConfig(UICanvasType.Overlay, 200, false, "OverlayCanvas"),
            new CanvasConfig(UICanvasType.Loading, 300, false, "LoadingCanvas"),
            new CanvasConfig(UICanvasType.Notification, 400, false, "NotificationCanvas"),
            new CanvasConfig(UICanvasType.Debug, 1000, false, "DebugCanvas")
        };
        
        public event Action<UICanvasType, Canvas> OnCanvasCreated;
        public event Action<UICanvasType> OnCanvasDestroyed;
        
        private Dictionary<UICanvasType, Canvas> _canvases = new Dictionary<UICanvasType, Canvas>();
        private Dictionary<UICanvasType, CanvasConfig> _canvasConfigs = new Dictionary<UICanvasType, CanvasConfig>();
        private bool _initialized = false;
        
        public void Initialize()
        {
            if (_initialized) return;
            
            // Initialize default canvas configurations
            foreach (var config in defaultCanvasConfigs)
            {
                _canvasConfigs[config.canvasType] = config;
            }
            
            _initialized = true;
            Debug.Log("UICanvasManager initialized");
        }
        
        public Canvas GetCanvas(UICanvasType canvasType)
        {
            return _canvases.TryGetValue(canvasType, out var canvas) ? canvas : null;
        }
        
        public Canvas CreateCanvas(UICanvasType canvasType, CanvasConfig config = null)
        {
            if (_canvases.ContainsKey(canvasType))
            {
                Debug.LogWarning($"Canvas of type {canvasType} already exists");
                return _canvases[canvasType];
            }
            
            // Use provided config or default config
            var canvasConfig = config ?? GetDefaultConfig(canvasType);
            
            // Create canvas GameObject
            GameObject canvasGO = new GameObject(canvasConfig.canvasName);
            canvasGO.transform.SetParent(transform);
            
            // Add Canvas component
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = canvasConfig.sortOrder;
            
            // Add CanvasScaler
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            // Add GraphicRaycaster
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Store canvas
            _canvases[canvasType] = canvas;
            _canvasConfigs[canvasType] = canvasConfig;
            
            OnCanvasCreated?.Invoke(canvasType, canvas);
            
            Debug.Log($"Created canvas: {canvasType}");
            return canvas;
        }
        
        public void DestroyCanvas(UICanvasType canvasType)
        {
            if (!_canvases.TryGetValue(canvasType, out var canvas)) return;
            
            // Don't destroy persistent canvases
            if (_canvasConfigs.TryGetValue(canvasType, out var config) && config.isPersistent)
            {
                Debug.LogWarning($"Cannot destroy persistent canvas: {canvasType}");
                return;
            }
            
            _canvases.Remove(canvasType);
            _canvasConfigs.Remove(canvasType);
            
            if (canvas != null && canvas.gameObject != null)
            {
                DestroyImmediate(canvas.gameObject);
            }
            
            OnCanvasDestroyed?.Invoke(canvasType);
            Debug.Log($"Destroyed canvas: {canvasType}");
        }
        
        public bool HasCanvas(UICanvasType canvasType)
        {
            return _canvases.ContainsKey(canvasType) && _canvases[canvasType] != null;
        }
        
        public Dictionary<UICanvasType, Canvas> GetAllCanvases()
        {
            return new Dictionary<UICanvasType, Canvas>(_canvases);
        }
        
        public void SetCanvasSortOrder(UICanvasType canvasType, int sortOrder)
        {
            if (_canvases.TryGetValue(canvasType, out var canvas))
            {
                canvas.sortingOrder = sortOrder;
                Debug.Log($"Set canvas {canvasType} sort order to {sortOrder}");
            }
        }
        
        public void SetCanvasActive(UICanvasType canvasType, bool show)
        {
            if (_canvases.TryGetValue(canvasType, out var canvas))
            {
                canvas.gameObject.SetActive(show);
                Debug.Log($"Set canvas {canvasType} active: {show}");
            }
        }
        
        public void Cleanup()
        {
            var canvasTypesToDestroy = new List<UICanvasType>(_canvases.Keys);
            
            foreach (var canvasType in canvasTypesToDestroy)
            {
                DestroyCanvas(canvasType);
            }
            
            _canvases.Clear();
            _canvasConfigs.Clear();
            _initialized = false;
            
            Debug.Log("UICanvasManager cleaned up");
        }
        
        private CanvasConfig GetDefaultConfig(UICanvasType canvasType)
        {
            if (_canvasConfigs.TryGetValue(canvasType, out var config))
            {
                return config;
            }
            
            // Create default config if none exists
            return new CanvasConfig(canvasType, 0, false);
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
