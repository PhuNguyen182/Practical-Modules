using System;
using PracticalSystems.InventorySystem.Models.Items;

namespace PracticalSystems.InventorySystem.Models.Manager
{
    [Serializable]
    public class InventoryItem
    {
        public string itemId;
        public int quantity;
        public ItemCategory itemCategory;
        public DateTime AcquiredDate = DateTime.Now;
    }
}