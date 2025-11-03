using System.Collections.Generic;
using PracticalSystems.GameResourceSystem.Manager;
using PracticalSystems.InventorySystem.Models.Items;
using PracticalSystems.InventorySystem.Models.Manager;

namespace PracticalSystems.InventorySystem.Manager
{
    public class InventoryManager
    {
        private readonly InventoryConfigDataController _inventoryConfigDataController;
        private readonly InventoryProgressionDataController _inventoryProgressionDataController;
        private readonly GameResourceProgressDataController _gameResourceProgressDataController;
        
        public InventoryManager(InventoryConfigDataController inventoryConfigDataController,
            InventoryProgressionDataController inventoryProgressionDataController,
            GameResourceProgressDataController gameResourceProgressDataController)
        {
            this._inventoryConfigDataController = inventoryConfigDataController;
            this._inventoryProgressionDataController = inventoryProgressionDataController;
            this._gameResourceProgressDataController = gameResourceProgressDataController;
        }

        public bool CanBuyItem(ItemData itemData)
        {
            var spendResources = itemData.buyPricing;
            return this._gameResourceProgressDataController.CanSpendResourcesByGroup(spendResources);
        }
        
        public void AddItem(InventoryItem item)
        {
            this._inventoryProgressionDataController.AddItem(item);
        }

        public bool RemoveItem(string itemId, int quantity = 1, bool forceRemove = false)
        {
            return this._inventoryProgressionDataController.RemoveItem(itemId, quantity, forceRemove);
        }
        
        public bool HasItem(string itemId)
        {
            return this._inventoryProgressionDataController.HasItem(itemId);
        }

        public InventoryItem GetInventoryItem(string itemId)
        {
            return this._inventoryProgressionDataController.GetSingleInventoryItemData(itemId);
        }

        public List<InventoryItem> GetAllInventoryItemData()
        {
            return this._inventoryProgressionDataController.GetAllInventoryItemData();
        }

        public ItemData GetItemDataInfo(InventoryItem inventoryItem)
        {
            return this._inventoryConfigDataController.GetItemData(inventoryItem);
        }

        public List<string> GetItemIdsByTags(params string[] queryTags)
        {
            return this._inventoryProgressionDataController.GetItemIdsByTags(queryTags);
        }
    }
}
