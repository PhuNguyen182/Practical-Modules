using System;
using System.Collections.Generic;
using Foundations.DataFlow.MicroData;
using UnityEngine;

namespace PracticalSystems.InventorySystem.Models.Set
{
    [Serializable]
    [GameData(nameof(ItemSetConfigData))]
    [CreateAssetMenu(fileName = "ItemSetConfigData", menuName = "Scriptable Objects/Inventory/ConfigData/ItemSetConfigData")]
    public class ItemSetConfigData : ScriptableObject, IGameData
    {
        public int Version { get; set; }
        
        [SerializeField] public List<ItemSetData> itemSetData = new();
    }
}
