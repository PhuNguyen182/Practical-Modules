using PracticalSystems.InventorySystem.Models.Items;
using PracticalSystems.InventorySystem.Models.Manager;

namespace PracticalSystems.InventorySystem.Manager
{
    public class InventoryManager
    {
        private readonly InventoryConfigDataController _inventoryConfigDataController;
        private readonly InventoryProgressionDataController _inventoryProgressionDataController;
        
        public InventoryManager(InventoryConfigDataController inventoryConfigDataController,
            InventoryProgressionDataController inventoryProgressionDataController)
        {
            this._inventoryConfigDataController = inventoryConfigDataController;
            this._inventoryProgressionDataController = inventoryProgressionDataController;
        }
        
        public void AddItem(InventoryItem item)
        {
            this._inventoryProgressionDataController.AddItem(item);
        }

        public void RemoveItem(string itemId, int quantity = 1, bool forceRemove = false)
        {
            this._inventoryProgressionDataController.RemoveItem(itemId, quantity, forceRemove);
        }

        public InventoryItem GetInventoryItem(string itemId)
        {
            return this._inventoryProgressionDataController.GetSingleInventoryItemData(itemId);
        }

        public ItemData GetItemDataInfo(InventoryItem inventoryItem)
        {
            return this._inventoryConfigDataController.GetItemData(inventoryItem);
        }
    }
}
