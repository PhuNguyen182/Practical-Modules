using System;
using System.Collections.Generic;
using PracticalSystems.InventorySystem.Models.Items;

namespace PracticalSystems.InventorySystem.Models.Manager
{
    [Serializable]
    public class InventoryItem
    {
        public int itemId;
        public int quantity;
        public ItemCategory itemCategory;
        public List<string> tags = new();
    }
}