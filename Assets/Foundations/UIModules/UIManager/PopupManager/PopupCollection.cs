using System;
using System.Collections.Generic;
using Foundations.UIModules.UIPresenter;
using UnityEngine;

namespace Foundations.UIModules.UIManager.PopupManager
{
    [Serializable]
    public class PopupKeyValueInfo
    {
        public string popupId;
        public GameObject popupPrefab;
    }
    
    [CreateAssetMenu(fileName = "PopupCollection", menuName = "UI/Popup Collection")]
    public class PopupCollection : ScriptableObject
    {
        [SerializeField] private List<PopupKeyValueInfo> popupKeyValues;
        
        public Dictionary<string, IUIPresenter> PopupPrefabs { get; private set; }
        
        public void Initialize()
        {
            PopupPrefabs = new();
            foreach (var popupKeyValue in popupKeyValues)
            {
                if (!popupKeyValue.popupPrefab) 
                    continue;

                if (popupKeyValue.popupPrefab.TryGetComponent(out IUIPresenter presenter))
                    PopupPrefabs.Add(popupKeyValue.popupId, presenter);
            }
        }
    }
}
