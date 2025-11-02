using System;
using Foundations.DataFlow.MicroData;
using PracticalSystems.InventorySystem.Models.Items;
using UnityEngine.Rendering;
using UnityEngine;

namespace PracticalSystems.InventorySystem.Models.Manager
{
    [Serializable]
    [GameData(nameof(InventoryConfigData))]
    [CreateAssetMenu(fileName = "InventoryConfigData", menuName = "Scriptable Objects/Inventory/ConfigData/InventoryConfigData")]
    public class InventoryConfigData : ScriptableObject, IGameData
    {
        public int Version { get; set; }
        public InventoryCategoryItemDictionary inventoryCategoryItemDatabase = new();
    }
    
    [Serializable]
    public class InventoryCategoryItemDictionary : SerializedDictionary<ItemCategory, InventoryCategoryItemDatabase>
    {
        
    }
}
