using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Foundations.UIModules.UIManager.UICanvases
{
    public class UICanvasManager : MonoBehaviour, IUICanvasManager
    {
        [SerializeField] private CanvasDefinitionSettings canvasDefinitionSettings;
        
        [Header("Canvas Settings")] [SerializeField]
        private CanvasConfig[] defaultCanvasConfigs =
        {
            new(UICanvasType.Main, 0, true, CanvasConstants.MainCanvasName),
            new(UICanvasType.Popup, 100, false, CanvasConstants.PopupCanvasName),
            new(UICanvasType.Overlay, 200, false, CanvasConstants.OverlayCanvasName),
            new(UICanvasType.Loading, 300, false, CanvasConstants.LoadingCanvasName),
            new(UICanvasType.Notification, 400, false, CanvasConstants.NotificationCanvasName),
            new(UICanvasType.Debug, 1000, false, CanvasConstants.DebugCanvasName)
        };
        
        private readonly Dictionary<UICanvasType, Canvas> _canvasConfigs = new();

        public Action<UICanvasType> OnCanvasDestroyed { get; set; }
        public Action<UICanvasType, Canvas> OnCanvasCreated { get; set; }
        
        private void Awake() => Initialize();

        public void Initialize()
        {
            if (!canvasDefinitionSettings)
            {
                Debug.LogError("No canvas definition settings found!");
                return;
            }
            
            canvasDefinitionSettings.Initialize();
            foreach (var canvasConfig in defaultCanvasConfigs)
            {
                var canvas = CreateCanvas(canvasConfig);
                _canvasConfigs.Add(canvasConfig.canvasType, canvas);
                OnCanvasCreated?.Invoke(canvasConfig.canvasType, canvas);
            }
        }
        
        public Canvas GetCanvas(UICanvasType canvasType)
            => _canvasConfigs.GetValueOrDefault(canvasType);

        public Canvas CreateCanvas(CanvasConfig config = null)
        {
            if (config == null)
            {
                Debug.LogError("Canvas config cannot be null");
                return null;
            }

            var canvas = Instantiate(canvasDefinitionSettings.canvasPrefab);
            var canvasSetting = canvasDefinitionSettings.CanvasSettings[config.canvasType];
            canvas.gameObject.name = config.canvasName;
            canvas.renderMode = canvasSetting.canvasRenderMode;
            canvas.sortingOrder = config.sortOrder;
            canvas.sortingLayerID = canvasSetting.SortingLayer.id;

            if (canvas.TryGetComponent(out CanvasScaler canvasScaler))
            {
                canvasScaler.screenMatchMode = canvasSetting.screenMatchMode;
                canvasScaler.referenceResolution = canvasSetting.referenceResolution;
                canvasScaler.matchWidthOrHeight = canvasSetting.matchWidthOrHeight;
            }
            
            return canvas;
        }

        public bool HasCanvas(UICanvasType canvasType)
        {
            bool hasCanvas = _canvasConfigs.ContainsKey(canvasType);
            bool isValidCanvas = _canvasConfigs[canvasType];
            return hasCanvas && isValidCanvas;
        }

        public Dictionary<UICanvasType, Canvas> GetAllCanvases() => _canvasConfigs;
        
        public void SetCanvasSortOrder(UICanvasType canvasType, int sortOrder)
        {
            var canvas = GetCanvas(canvasType);
            canvas.sortingOrder = sortOrder;
            _canvasConfigs[canvasType] = canvas;
        }

        public void SetCanvasActive(UICanvasType canvasType, bool active)
        {
            if (HasCanvas(canvasType))
            {
                var canvas = GetCanvas(canvasType);
                canvas.gameObject.SetActive(active);
            }
        }
        
        public void DestroyCanvas(UICanvasType canvasType)
        {
            if (HasCanvas(canvasType))
            {
                var canvas = GetCanvas(canvasType);
                Destroy(canvas.gameObject);
                _canvasConfigs.Remove(canvasType);
                OnCanvasDestroyed?.Invoke(canvasType);
            }
        }

        public void Cleanup()
        {
            var canvasKeys = new List<UICanvasType>(_canvasConfigs.Keys);
            foreach (var canvasKey in canvasKeys)
                DestroyCanvas(canvasKey);
            
            _canvasConfigs.Clear();
        }

        private void OnDestroy()
        {
            Cleanup();
            OnCanvasDestroyed = null;
            OnCanvasCreated = null;
        }
    }
}
