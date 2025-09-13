using System;
using System.Collections.Generic;
using UnityEngine;
using UISystem.MVP.Examples;

namespace UISystem.MVP
{
    /// <summary>
    /// Implementation of IPopupCreator for creating popups with data binding
    /// </summary>
    public class PopupCreator : MonoBehaviour, IPopupCreator
    {
        [Header("Popup Creator Settings")]
        [SerializeField] private bool autoRegisterDefaultTypes = true;
        [SerializeField] private string defaultPopupPrefabPath = "Prefabs/Popups/";
        
        public event Action<IPresenter>? OnPopupCreated;
        
        private Dictionary<string, Func<object?, Transform?, IPresenter>> _popupFactories = new Dictionary<string, Func<object?, Transform?, IPresenter>>();
        private Dictionary<string, object> _typedFactories = new Dictionary<string, object>();
        private IUICanvasManager? _canvasManager;
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
            
            if (autoRegisterDefaultTypes)
            {
                RegisterDefaultPopupTypes();
            }
            
            _initialized = true;
            Debug.Log("PopupCreator initialized");
        }
        
        public IPresenter? CreatePopup(string popupType, object? data = null, Transform? parent = null)
        {
            if (!_initialized)
            {
                Debug.LogError("PopupCreator not initialized!");
                return null;
            }
            
            if (string.IsNullOrEmpty(popupType))
            {
                Debug.LogError("Popup type cannot be null or empty!");
                return null;
            }
            
            if (!_popupFactories.TryGetValue(popupType, out var factory))
            {
                Debug.LogError($"Popup type '{popupType}' is not registered!");
                return null;
            }
            
            // Use popup canvas as default parent if none provided
            if (parent == null)
            {
                var popupCanvas = _canvasManager.GetCanvas(UICanvasType.Popup);
                parent = popupCanvas?.transform;
            }
            
            try
            {
                var presenter = factory.Invoke(data, parent);
                OnPopupCreated?.Invoke(presenter);
                
                Debug.Log($"Created popup of type: {popupType}");
                return presenter;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create popup of type '{popupType}': {ex.Message}");
                return null;
            }
        }
        
        public IPresenter? CreatePopup<T>(string popupType, T data, Transform? parent = null)
        {
            if (!_initialized)
            {
                Debug.LogError("PopupCreator not initialized!");
                return null;
            }
            
            if (string.IsNullOrEmpty(popupType))
            {
                Debug.LogError("Popup type cannot be null or empty!");
                return null;
            }
            
            // Try to find typed factory first
            if (_typedFactories.TryGetValue(popupType, out var typedFactoryObj))
            {
                if (typedFactoryObj is Func<T, Transform, IPresenter> typedFactory)
                {
                    if (parent == null)
                    {
                        var popupCanvas = _canvasManager.GetCanvas(UICanvasType.Popup);
                        parent = popupCanvas?.transform;
                    }
                    
                    try
                    {
                        var presenter = typedFactory.Invoke(data, parent);
                        OnPopupCreated?.Invoke(presenter);
                        
                        Debug.Log($"Created typed popup of type: {popupType}");
                        return presenter;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to create typed popup of type '{popupType}': {ex.Message}");
                        return null;
                    }
                }
            }
            
            // Fallback to generic factory
            return CreatePopup(popupType, data, parent);
        }
        
        public void RegisterPopupType(string popupType, Func<object?, Transform?, IPresenter> factory)
        {
            if (string.IsNullOrEmpty(popupType))
            {
                Debug.LogError("Popup type cannot be null or empty!");
                return;
            }
            
            if (factory == null)
            {
                Debug.LogError("Factory function cannot be null!");
                return;
            }
            
            _popupFactories[popupType] = factory;
            Debug.Log($"Registered popup type: {popupType}");
        }
        
        public void RegisterPopupType<T>(string popupType, Func<T, Transform?, IPresenter> factory)
        {
            if (string.IsNullOrEmpty(popupType))
            {
                Debug.LogError("Popup type cannot be null or empty!");
                return;
            }
            
            if (factory == null)
            {
                Debug.LogError("Typed factory function cannot be null!");
                return;
            }
            
            _typedFactories[popupType] = factory;
            
            // Also register as generic factory
            RegisterPopupType(popupType, (data, parent) => factory.Invoke((T)data, parent));
            
            Debug.Log($"Registered typed popup type: {popupType}<{typeof(T).Name}>");
        }
        
        public void UnregisterPopupType(string popupType)
        {
            if (_popupFactories.Remove(popupType))
            {
                Debug.Log($"Unregistered popup type: {popupType}");
            }
            
            if (_typedFactories.Remove(popupType))
            {
                Debug.Log($"Unregistered typed popup type: {popupType}");
            }
        }
        
        public bool IsPopupTypeRegistered(string popupType)
        {
            return _popupFactories.ContainsKey(popupType);
        }
        
        public List<string> GetRegisteredPopupTypes()
        {
            return new List<string>(_popupFactories.Keys);
        }
        
        public void Cleanup()
        {
            _popupFactories.Clear();
            _typedFactories.Clear();
            _initialized = false;
            
            Debug.Log("PopupCreator cleaned up");
        }
        
        private void RegisterDefaultPopupTypes()
        {
            // Register some common popup types
            RegisterPopupType("SimpleDialog", (data, parent) => CreateSimpleDialogPopup(data, parent));
            RegisterPopupType("ConfirmDialog", (data, parent) => CreateConfirmDialogPopup(data, parent));
            RegisterPopupType("InputDialog", (data, parent) => CreateInputDialogPopup(data, parent));
            RegisterPopupType("LoadingPopup", (data, parent) => CreateLoadingPopup(data, parent));
            
            Debug.Log("Registered default popup types");
        }
        
        private IPresenter? CreateSimpleDialogPopup(object? data, Transform? parent)
        {
            // This would typically load from a prefab
            // For now, we'll create a simple implementation
            var dialogModel = new SimpleDialogModel();
            var dialogView = CreateSimpleDialogView(parent);
            var dialogPresenter = new SimpleDialogPresenter();
            
            dialogPresenter.Initialize(dialogModel, dialogView);
            
            if (data is SimpleDialogData dialogData)
            {
                dialogModel.Initialize(dialogData);
            }
            
            return dialogPresenter;
        }
        
        private IPresenter? CreateConfirmDialogPopup(object? data, Transform? parent)
        {
            var confirmModel = new ConfirmDialogModel();
            var confirmView = CreateConfirmDialogView(parent);
            var confirmPresenter = new ConfirmDialogPresenter();
            
            confirmPresenter.Initialize(confirmModel, confirmView);
            
            if (data is ConfirmDialogData confirmData)
            {
                confirmModel.Initialize(confirmData);
            }
            
            return confirmPresenter;
        }
        
        private IPresenter? CreateInputDialogPopup(object? data, Transform? parent)
        {
            var inputModel = new InputDialogModel();
            var inputView = CreateInputDialogView(parent);
            var inputPresenter = new InputDialogPresenter();
            
            inputPresenter.Initialize(inputModel, inputView);
            
            if (data is InputDialogData inputData)
            {
                inputModel.Initialize(inputData);
            }
            
            return inputPresenter;
        }
        
        private IPresenter? CreateLoadingPopup(object? data, Transform? parent)
        {
            var loadingModel = new LoadingPopupModel();
            var loadingView = CreateLoadingPopupView(parent);
            var loadingPresenter = new LoadingPopupPresenter();
            
            loadingPresenter.Initialize(loadingModel, loadingView);
            
            if (data is LoadingPopupData loadingData)
            {
                loadingModel.Initialize(loadingData);
            }
            
            return loadingPresenter;
        }
        
        // These methods would typically load from prefabs
        // For now, we'll create placeholder implementations
        private SimpleDialogView? CreateSimpleDialogView(Transform? parent)
        {
            var go = new GameObject("SimpleDialog");
            go.transform.SetParent(parent);
            return go.AddComponent<SimpleDialogView>();
        }
        
        private ConfirmDialogView? CreateConfirmDialogView(Transform? parent)
        {
            var go = new GameObject("ConfirmDialog");
            go.transform.SetParent(parent);
            return go.AddComponent<ConfirmDialogView>();
        }
        
        private InputDialogView? CreateInputDialogView(Transform? parent)
        {
            var go = new GameObject("InputDialog");
            go.transform.SetParent(parent);
            return go.AddComponent<InputDialogView>();
        }
        
        private LoadingPopupView? CreateLoadingPopupView(Transform? parent)
        {
            var go = new GameObject("LoadingPopup");
            go.transform.SetParent(parent);
            return go.AddComponent<LoadingPopupView>();
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
