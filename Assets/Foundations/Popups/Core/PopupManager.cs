using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Foundations.Popups.Interfaces;

namespace Foundations.Popups.Core
{
    /// <summary>
    /// Core popup manager that handles creation, destruction, and lifecycle of all popups
    /// Implements IPopupManager interface following MVP pattern
    /// </summary>
    public class PopupManager : MonoBehaviour, IPopupManager
    {
        [Header("Popup Manager Settings")]
        [SerializeField] private Transform popupParent;
        [SerializeField] private Canvas popupCanvas;
        [SerializeField] private int defaultSortingOrder = 100;
        
        private readonly Dictionary<Type, List<IPopup>> _activePopups = new();
        private readonly Dictionary<Type, GameObject> _popupPrefabs = new();
        private int _currentSortingOrder;
        
        public event Action<IPopup> OnPopupShown;
        public event Action<IPopup> OnPopupHidden;
        public event Action<IPopup> OnPopupDestroyed;
        
        private void Awake()
        {
            InitializePopupManager();
        }
        
        private void InitializePopupManager()
        {
            if (!popupParent)
                popupParent = transform;
                
            if (!popupCanvas)
            {
                popupCanvas = GetComponent<Canvas>();
                if (!popupCanvas)
                {
                    popupCanvas = gameObject.AddComponent<Canvas>();
                    popupCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                }
            }
            
            _currentSortingOrder = defaultSortingOrder;
            popupCanvas.sortingOrder = _currentSortingOrder;
        }
        
        public T ShowPopup<T>() where T : class, IPopup
        {
            var popup = CreatePopup<T>();
            if (popup != null)
            {
                popup.Show();
                OnPopupShown?.Invoke(popup);
            }
            return popup;
        }
        
        public T ShowPopup<T, TData>(TData data) where T : class, IPopup<TData>
        {
            var popup = CreatePopup<T>();
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
            if (popup == null) return;
            
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
            var allPopups = _activePopups.Values.SelectMany(popupList => popupList).ToList();
            foreach (var popup in allPopups)
            {
                HidePopup(popup);
            }
        }
        
        public bool IsPopupActive<T>() where T : class, IPopup
        {
            return GetPopupsOfType<T>().Any(popup => popup.IsActive);
        }
        
        public IReadOnlyList<IPopup> GetActivePopups()
        {
            return _activePopups.Values
                .SelectMany(popupList => popupList)
                .Where(popup => popup.IsActive)
                .ToList();
        }
        
        private T CreatePopup<T>() where T : class, IPopup
        {
            var popupType = typeof(T);
            
            // Try to get prefab from dictionary first
            if (_popupPrefabs.TryGetValue(popupType, out GameObject prefab))
            {
                return InstantiatePopup<T>(prefab);
            }
            
            // Try to find prefab in Resources folder
            prefab = Resources.Load<GameObject>($"Popups/{popupType.Name}");
            if (prefab != null)
            {
                _popupPrefabs[popupType] = prefab;
                return InstantiatePopup<T>(prefab);
            }
            
            Debug.LogError($"Popup prefab for type {popupType.Name} not found in Resources/Popups/");
            return null;
        }
        
        private T InstantiatePopup<T>(GameObject prefab) where T : class, IPopup
        {
            var instance = Instantiate(prefab, popupParent);
            var popup = instance.GetComponent<T>();
            
            if (popup == null)
            {
                Debug.LogError($"Popup component of type {typeof(T).Name} not found on prefab {prefab.name}");
                Destroy(instance);
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
            var canvas = instance.GetComponent<Canvas>();
            if (canvas)
            {
                canvas.sortingOrder = ++_currentSortingOrder;
            }
            
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
