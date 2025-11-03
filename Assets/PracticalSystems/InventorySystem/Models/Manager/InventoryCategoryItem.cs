using System;
using System.Collections.Generic;

namespace PracticalSystems.InventorySystem.Models.Manager
{
    [Serializable]
    public class InventoryCategoryItem
    {
        public Dictionary<int, InventoryItem> ItemData = new();

        public void AddItem(InventoryItem item)
        {
            int itemId = item.itemId;
            if (this.ItemData.TryAdd(itemId, item))
                return;

            var existingItem = this.ItemData[itemId];
            existingItem.quantity += item.quantity;
            this.ItemData[itemId] = existingItem;
        }

        public ItemRemoveStatus RemoveItem(int itemId, int quantity = 1, bool forceRemove = false)
        {
            if (!this.ItemData.TryGetValue(itemId, out InventoryItem item))
                return ItemRemoveStatus.NotRemove;

            if (forceRemove)
            {
                this.ItemData.Remove(itemId);
                return ItemRemoveStatus.Removed;
            }

            int currentQuantity = item.quantity;
            int offset = currentQuantity - quantity;

            if (offset > 0)
            {
                item.quantity = offset;
                this.ItemData[itemId] = item;
                return ItemRemoveStatus.StillHasItem;
            }

            this.ItemData.Remove(itemId);
            return ItemRemoveStatus.Removed;
        }
    }
}