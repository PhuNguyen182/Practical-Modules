using System.Collections.Generic;

namespace PracticalSystems.InventorySystem.Models.Manager
{
    public class InventoryItemComparable : IComparer<InventoryItem>
    {
        public int Compare(InventoryItem x, InventoryItem y)
        {
            int idComparison = string.CompareOrdinal(x.itemId, y.itemId);
            int timeComparison = x.AcquiredDate.CompareTo(y.AcquiredDate);
            if (idComparison != 0)
                return idComparison;

            return timeComparison != 0 ? timeComparison : x.quantity.CompareTo(y.quantity);
        }
    }
}