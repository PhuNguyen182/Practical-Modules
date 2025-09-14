using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Foundations.UIModules.UIManager.UICanvases;
using PracticalModules.Patterns.SimpleObjectPooling;
using Foundations.UIModules.Popups;

namespace Foundations.UIModules.UIManager.PopupManager
{
    public class UIPopupManager<TPopupViewData, TPresenterData> : IUIPopupManager<TPopupViewData, TPresenterData>
    {
        private const int MaxPopupCount = 10;
        
        private readonly IUICanvasManager _canvasManager;
        private readonly PopupCollection _popupCollection;
        private readonly Dictionary<string, BasePopupPresenter<TPopupViewData, TPresenterData>> _activePopups;
        private readonly List<BasePopupPresenter<TPopupViewData, TPresenterData>> _popupStack;
        
        public UIPopupManager(IUICanvasManager canvasManager, PopupCollection popupCollection)
        {
            _canvasManager = canvasManager;
            _popupCollection = popupCollection;
            _activePopups = new();
            _popupStack = new();
            Initialize();
        }
        
        public void Initialize()
        {
            _popupCollection.Initialize();
        }

        public SimplePopup CreatePopup(string popupId, Action onPopupOpened = null, Action onPopupClosed = null)
        {
            return null;
        }

        public BasePopupPresenter<TPopupViewData, TPresenterData> ShowPopup(string popupId,
            TPresenterData presenterData, Action onPopupOpened = null, Action onPopupClosed = null)
        {
            if (string.IsNullOrEmpty(popupId))
            {
                Debug.LogError("Popup ID cannot be null or empty!");
                return null;
            }
            
            if (_activePopups.TryGetValue(popupId, out var popup))
            {
                Debug.LogWarning($"Popup with ID {popupId} already exists!");
                return popup;
            }
            
            if (_activePopups.Count >= MaxPopupCount)
            {
                Debug.LogWarning($"Maximum popup count ({MaxPopupCount}) reached!");
                return null;
            }
            
            if (!_popupCollection.PopupPrefabs.TryGetValue(popupId, out var popupInfo))
                return null;

            if (popupInfo is BasePopupPresenter<TPopupViewData, TPresenterData> popupPresenter)
            {
                var spawnPopup = ObjectPoolManager.Spawn(popupPresenter);
                _activePopups[popupId] = spawnPopup;
                _popupStack.Add(spawnPopup);
                SortPopup();

                ShowPopupInternal(popupPresenter);
                Debug.Log($"Shown popup: {popupInfo}");
                return popupPresenter;
            }
            
            return null;
        }

        public void ClosePopup(string popupId)
        {
            if (!_activePopups.TryGetValue(popupId, out var popup))
                return;
            
            HidePopupInternal(popup);
            _activePopups.Remove(popupId);
            _popupStack.Remove(popup);
        }

        public void CloseAllPopups()
        {
            var popupIds = new List<string>(_activePopups.Keys);
            foreach (var popupId in popupIds)
            {
                ClosePopup(popupId);
            }
        }

        public void CloseAllPopupsExcept(string keepPopupId)
        {
            var popupIds = new List<string>(_activePopups.Keys);
            foreach (var popupId in popupIds)
            {
                if (string.CompareOrdinal(popupId, keepPopupId) == 0)
                    continue;

                ClosePopup(popupId);
            }
        }

        public BasePopupPresenter<TPopupViewData, TPresenterData> GetPopupInfo(string popupId)
        {
            return _activePopups.GetValueOrDefault(popupId);
        }

        public bool IsPopupShown(string popupId)
        {
            return _activePopups.ContainsKey(popupId);
        }

        public List<BasePopupPresenter<TPopupViewData, TPresenterData>> GetAllShownPopups()
        {
            return new List<BasePopupPresenter<TPopupViewData, TPresenterData>>(_activePopups.Values);
        }

        public BasePopupPresenter<TPopupViewData, TPresenterData> GetTopPopup()
        {
            return _popupStack.LastOrDefault();
        }

        public List<BasePopupPresenter<TPopupViewData, TPresenterData>> GetPopupStack()
        {
            return _popupStack;
        }

        public void BringPopupToFront(string popupId)
        {
            if (!_activePopups.TryGetValue(popupId, out var popup)) 
                return;
            
            int highestPriority = _popupStack.Count > 0 ? _popupStack[0].Priority : 0;
            SetPopupPriority(popupId, highestPriority + 1);
        }

        public void SetPopupPriority(string popupId, int priority)
        {
            if (!_activePopups.TryGetValue(popupId, out var popup)) 
                return;
            
            popup.Priority = priority;
            SortPopup();
            UpdatePopupVisualOrder();
        }

        public void Cleanup()
        {
            CloseAllPopups();
            _activePopups.Clear();
            _popupStack.Clear();
        }
        
        private void ShowPopupInternal(BasePopupPresenter<TPopupViewData, TPresenterData> popupInfo)
        {
            if (popupInfo != null)
                popupInfo.Show();
            
            UpdatePopupVisualOrder();
        }
        
        private void HidePopupInternal(BasePopupPresenter<TPopupViewData, TPresenterData> popupInfo)
        {
            if (popupInfo)
                popupInfo.Hide();
        }
        
        private void UpdatePopupVisualOrder()
        {
            var popupCanvas = _canvasManager.GetCanvas(UICanvasType.Popup);
            if (!popupCanvas) 
                return;
            
            for (int i = 0; i < _popupStack.Count; i++)
            {
                var popup = _popupStack[i];
                if (popup != null)
                {
                    popup.transform.SetSiblingIndex(i);
                }
            }
        }

        private void SortPopup()
        {
            _popupStack.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }
    }
}
