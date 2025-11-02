using System.Collections.Generic;
using Foundations.SaveSystem;
using Foundations.DataFlow.MicroData.DynamicDataControllers;
using Foundations.SaveSystem.CustomDataSaverService;
using Foundations.SaveSystem.CustomDataSerializerServices;
using PracticalSystems.InventorySystem.Models.Items;
using PracticalSystems.InventorySystem.Models.Manager;
using ZLinq;

namespace PracticalSystems.InventorySystem.Manager
{
    public class InventoryProgressionDataController : DynamicGameDataHandler<InventoryProgressionData>
    {
        protected override InventoryProgressionData SourceData { get; set; }

        protected override IDataSerializer<InventoryProgressionData> DataSerializer =>
            new JsonDataSerializer<InventoryProgressionData>();

        protected override IDataSaveService<InventoryProgressionData> DataSaveService =>
            new FileDataSaveService<InventoryProgressionData>(this.DataSerializer);
        
        public override void Initialize()
        {
            
        }
        
        public InventoryItem GetSingleInventoryItemData(string itemId)
        {
            foreach (var categoryItemData in this.SourceData.InventoryCategoryItemData)
            {
                var categoryData = categoryItemData.Value;
                if (categoryData.ItemData.TryGetValue(itemId, out InventoryItem itemData))
                    return itemData;
            }
            
            return null;
        }

        public List<InventoryItem> GetInventoryItemDataByCategory(ItemCategory category)
        {
            var categoryItemData = this.SourceData.InventoryCategoryItemData[category];
            var itemData = categoryItemData.ItemData;
            var allItemData = itemData.Values.AsValueEnumerable().ToList();
            return allItemData;
        }

        public void AddItem(InventoryItem item)
        {
            var category = item.itemCategory;
            if (this.SourceData.InventoryCategoryItemData.ContainsKey(category))
            {
                var categoryData = this.SourceData.InventoryCategoryItemData[category];
                categoryData.AddItem(item);
                this.SourceData.InventoryCategoryItemData[category] = categoryData;
            }
            else
            {
                Dictionary<string, InventoryItem> itemData = new()
                {
                    { item.itemId, item }
                };
                
                this.SourceData.InventoryCategoryItemData.Add(category, new InventoryCategoryItem()
                {
                    ItemData = itemData,
                });
            }
        }
        
        public bool RemoveItem(string itemId, int quantity = 1, bool forceRemove = false)
        {
            foreach (var categoryItemData in this.SourceData.InventoryCategoryItemData)
            {
                var categoryData = categoryItemData.Value;
                if (categoryData.RemoveItem(itemId, quantity, forceRemove))
                    return true;
            }
            
            return false;
        }
    }
}
