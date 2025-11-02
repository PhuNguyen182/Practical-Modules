using System.Collections.Generic;
using Foundations.DataFlow.MicroData.StaticDataControllers;
using PracticalSystems.InventorySystem.Models.Manager;
using Foundations.DataFlow.ProcessingSequence;
using PracticalSystems.InventorySystem.Models.Items;
using ZLinq;

namespace PracticalSystems.InventorySystem.Manager
{
    public class InventoryConfigDataController : StaticGameDataHandler<InventoryConfigData>
    {
        protected override InventoryConfigData SourceData { get; set; }

        public override List<DataProcessSequence> DataProcessSequences => new()
        {
            new DataProcessSequence(this.GetDataKey(), DataProcessorType.ScriptableObjects)
        };

        protected override void OnDataInitialized()
        {
            
        }

        public ItemData GetItemData(InventoryItem inventoryItem)
        {
            var itemCategory = inventoryItem.itemCategory;
            foreach (var category in this.SourceData.inventoryCategoryItemDatabase.Keys)
            {
                if (category != itemCategory) 
                    continue;
                
                var data = this.SourceData.inventoryCategoryItemDatabase[itemCategory];
                if (!data.itemData.TryGetValue(inventoryItem.itemId, out var itemData)) 
                    continue;
                
                return itemData;
            }
            
            return null;
        }
        
        public List<ItemData> GetAllItemDataInfoWithCategory(ItemCategory category)
        {
            if (!this.SourceData.inventoryCategoryItemDatabase.ContainsKey(category))
                return null;
            
            var categoryItemDatabase = this.SourceData.inventoryCategoryItemDatabase[category];
            var itemData = categoryItemDatabase.itemData;
            var allItemData = itemData.Values.AsValueEnumerable().ToList();
            return allItemData;
        }
    }
}
