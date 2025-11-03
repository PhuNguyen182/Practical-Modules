using System.Collections.Generic;

namespace PracticalSystems.InventorySystem.Models.Manager
{
    public class InventoryItemComparable : IComparer<InventoryItem>
    {
        public int Compare(InventoryItem x, InventoryItem y)
        {
            int idComparison = x.itemId.CompareTo(y.itemId);
            if (idComparison != 0)
                return idComparison;

            return x.quantity.CompareTo(y.quantity);
        }
    }
}