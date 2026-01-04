using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Foundations.UIModules.Popups.Interfaces;
using Foundations.UIModules.UIManager.UICanvases;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZLinq;

namespace Foundations.UIModules.Popups.Core
{
    /// <summary>
    /// Core popup manager that handles creation, destruction, and lifecycle of all popups
    /// Implements IPopupManager interface following the MVP pattern
    /// </summary>
    public class PopupManager : IPopupManager
    {
        private readonly IUICanvasManager _uiCanvasManager;
        private readonly Dictionary<Type, List<IPopup>> _activePopups;
        private readonly Dictionary<Type, GameObject> _popupPrefabs;
        
        public event Action<IPopup> OnPopupShown;
        public event Action<IPopup> OnPopupHidden;
        public event Action<IPopup> OnPopupDestroyed;

        public PopupManager(IUICanvasManager uiCanvasManager)
        {
            _uiCanvasManager = uiCanvasManager;
            _activePopups = new();
            _popupPrefabs = new();
            InitializePopupManager();
        }
        
        private void InitializePopupManager()
        {
            
        }
        
        public async UniTask<T> ShowPopup<T>(string popupName) where T : class, IPopup
        {
            var popup = await CreatePopup<T>(popupName);
            if (popup != null)
            {
                popup.Show();
                OnPopupShown?.Invoke(popup);
            }
            return popup;
        }

        public async UniTask<T> ShowPopup<T, TData>(string popupName, TData data) where T : class, IPopup<TData>
        {
            var popup = await CreatePopup<T>(popupName);
            if (popup != null)
            {
                popup.UpdateData(data);
                popup.Show();
                OnPopupShown?.Invoke(popup);
            }

            return popup;
        }

        public void HidePopup(IPopup popup)
        {
            if (popup == null) 
                return;
            
            popup.Hide();
            OnPopupHidden?.Invoke(popup);
        }
        
        public void HidePopup<T>() where T : class, IPopup
        {
            var popups = GetPopupsOfType<T>();
            foreach (var popup in popups)
            {
                HidePopup(popup);
            }
        }
        
        public void HideAllPopups()
        {
            var allPopups = _activePopups.Values.AsValueEnumerable().SelectMany(popupList => popupList).ToList();
            foreach (var popup in allPopups)
            {
                HidePopup(popup);
            }
        }
        
        public bool IsPopupActive<T>() where T : class, IPopup
        {
            return GetPopupsOfType<T>().AsValueEnumerable().Any(popup => popup.IsActive);
        }
        
        public IReadOnlyList<IPopup> GetActivePopups()
        {
            return _activePopups.Values.AsValueEnumerable()
                .SelectMany(popupList => popupList)
                .Where(popup => popup.IsActive)
                .ToList();
        }
        
        private async UniTask<T> CreatePopup<T>(string popupName) where T : class, IPopup
        {
            var popupType = typeof(T);
            if (_popupPrefabs.TryGetValue(popupType, out GameObject prefab))
                return InstantiatePopup<T>(prefab);
            
            prefab = await Addressables.LoadAssetAsync<GameObject>(popupName);
            if (prefab)
            {
                _popupPrefabs[popupType] = prefab;
                return InstantiatePopup<T>(prefab);
            }
            
            Debug.LogError($"Popup prefab for type {popupType.Name} not found");
            return null;
        }
        
        private T InstantiatePopup<T>(GameObject prefab) where T : class, IPopup
        {
            GameObject instance = GameObjectPoolManager.Spawn(prefab);
            if (!instance.TryGetComponent<T>(out var popup))
            {
                Debug.LogError($"Popup component of type {typeof(T).Name} not found on prefab {prefab.name}");
                GameObjectPoolManager.Despawn(instance);
                return null;
            }
            
            // Setup popup events
            popup.OnShown += OnPopupShownInternal;
            popup.OnHidden += OnPopupHiddenInternal;
            popup.OnDestroyed += OnPopupDestroyedInternal;
            
            // Add to active popups
            var popupType = typeof(T);
            if (!_activePopups.ContainsKey(popupType))
                _activePopups[popupType] = new List<IPopup>();
            
            _activePopups[popupType].Add(popup);
            
            // Set sorting order
            var canvas = _uiCanvasManager.GetCanvas(UICanvasType.Popup);
            if (canvas)
                popup.Transform.SetParent(canvas.transform);
            
            return popup;
        }
        
        private void OnPopupShownInternal(IPopup popup)
        {
            OnPopupShown?.Invoke(popup);
        }
        
        private void OnPopupHiddenInternal(IPopup popup)
        {
            OnPopupHidden?.Invoke(popup);
        }
        
        private void OnPopupDestroyedInternal(IPopup popup)
        {
            // Remove from active popups
            var popupType = popup.PopupType;
            if (_activePopups.ContainsKey(popupType))
            {
                _activePopups[popupType].Remove(popup);
                if (_activePopups[popupType].Count == 0)
                {
                    _activePopups.Remove(popupType);
                }
            }
            
            OnPopupDestroyed?.Invoke(popup);
        }
        
        private IEnumerable<IPopup> GetPopupsOfType<T>() where T : class, IPopup
        {
            var popupType = typeof(T);
            if (_activePopups.TryGetValue(popupType, out var popupList))
            {
                return popupList;
            }
            
            return Enumerable.Empty<IPopup>();
        }
        
        /// <summary>
        /// Register a popup prefab for a specific type
        /// </summary>
        /// <typeparam name="T">Popup type</typeparam>
        /// <param name="prefab">Popup prefab</param>
        public void RegisterPopupPrefab<T>(GameObject prefab) where T : class, IPopup
        {
            _popupPrefabs[typeof(T)] = prefab;
        }
        
        /// <summary>
        /// Unregister a popup prefab
        /// </summary>
        /// <typeparam name="T">Popup type</typeparam>
        public void UnregisterPopupPrefab<T>() where T : class, IPopup
        {
            _popupPrefabs.Remove(typeof(T));
        }
    }
}
