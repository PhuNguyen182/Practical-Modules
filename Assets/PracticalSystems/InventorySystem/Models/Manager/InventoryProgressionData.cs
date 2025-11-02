using System;
using System.Collections.Generic;
using Foundations.DataFlow.MicroData;
using PracticalSystems.InventorySystem.Models.Items;

namespace PracticalSystems.InventorySystem.Models.Manager
{
    [Serializable]
    [GameData(nameof(InventoryProgressionData))]
    public class InventoryProgressionData : IGameData
    {
        public int Version { get; set; }
        
        public Dictionary<ItemCategory, InventoryCategoryItem> InventoryCategoryItemData = new();
    }
}
