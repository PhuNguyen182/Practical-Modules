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

    [Serializable]
    public class InventoryCategoryItem
    {
        public Dictionary<string, InventoryItem> ItemData = new();

        public void AddItem(InventoryItem item)
        {
            string itemId = item.itemId;
            if (this.ItemData.TryAdd(itemId, item)) 
                return;
            
            var existingItem = this.ItemData[itemId];
            existingItem.quantity += item.quantity;
            this.ItemData[itemId] = existingItem;
        }
        
        public bool RemoveItem(string itemId, int quantity = 1, bool forceRemove = false)
        {
            if (!this.ItemData.TryGetValue(itemId, out InventoryItem item))
                return false;

            if (forceRemove)
            {
                this.ItemData.Remove(itemId);
                return true;
            }
            
            int currentQuantity = item.quantity;
            int offset = currentQuantity - quantity;
            
            if (offset > 0)
            {
                item.quantity = offset;
                this.ItemData[itemId] = item;
            }
            else
            {
                this.ItemData.Remove(itemId);
            }
            
            return true;
        }
    }
}
