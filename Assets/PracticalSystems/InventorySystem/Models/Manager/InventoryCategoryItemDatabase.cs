using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using PracticalSystems.InventorySystem.Models.Items;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace PracticalSystems.InventorySystem.Models.Manager
{
    [Serializable]
    [CreateAssetMenu(fileName = "InventoryCategoryItemDatabase", menuName = "Scriptable Objects/Inventory/ConfigData/InventoryCategoryItemDatabase")]
    public class InventoryCategoryItemDatabase : ScriptableObject
    {
        [SerializeField] public ItemDataDictionary itemData = new();

        #if UNITY_EDITOR
        [Space(20)]
        [Header("Editor Only")]
        [JsonIgnore]
        public List<ItemData> itemDataList = new();
        
        [Button]
        private void BuildItemData()
        {
            var kvp = itemDataList.ToDictionary(key => key.itemId, value => value);
            itemData.Clear();
            foreach (var item in kvp)
                itemData.Add(item.Key, item.Value);
        }
        #endif
    }
    
    [Serializable]
    public class ItemDataDictionary : SerializedDictionary<int, ItemData>
    {
        
    }
}