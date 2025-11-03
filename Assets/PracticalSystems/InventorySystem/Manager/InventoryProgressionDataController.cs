using System;
using System.Collections.Generic;
using Foundations.SaveSystem;
using Foundations.DataFlow.MicroData.DynamicDataControllers;
using Foundations.SaveSystem.CustomDataSaverService;
using Foundations.SaveSystem.CustomDataSerializerServices;
using PracticalSystems.InventorySystem.Models.Items;
using PracticalSystems.InventorySystem.Models.Manager;
using UnityEngine.Pool;
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

        private readonly Dictionary<string, HashSet<string>> _itemTagMap = new();

        public event Action<InventoryItem> OnItemAdded;
        public event Action<string, int, bool> OnItemRemoved;

        public override void Initialize()
        {
            var allItemData = this.GetAllInventoryItemData();
            foreach (var inventoryItem in allItemData)
            {
                this.AddItemToTagMap(inventoryItem);
            }
        }

        public bool HasItem(string itemId)
        {
            var itemData = this.GetSingleInventoryItemData(itemId);
            return itemData != null;
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

        public List<InventoryItem> GetAllInventoryItemData()
        {
            List<InventoryItem> itemDataList = new();

            foreach (var categoryItemData in this.SourceData.InventoryCategoryItemData)
            {
                var categoryData = categoryItemData.Value;
                var itemData = categoryData.ItemData;
                itemDataList.AddRange(itemData.Values);
            }

            return itemDataList;
        }

        public List<string> GetItemIdsByTags(params string[] queryTags)
        {
            if (queryTags is not { Length: > 0 })
                return null;

            using var smallestSet = HashSetPool<string>.Get(out var minimumSetOfItemIds);
            using var checkingSet = HashSetPool<string>.Get(out var currentSet);
            using var resultSet = HashSetPool<string>.Get(out var results);

            string minimumTag = "";
            minimumSetOfItemIds = this._itemTagMap[queryTags[0]];
            
            for (int i = 0; i < queryTags.Length; i++)
            {
                if (!_itemTagMap.ContainsKey(queryTags[i]))
                    return null;

                currentSet = this._itemTagMap[queryTags[i]];
                if (currentSet.Count < minimumSetOfItemIds.Count)
                {
                    minimumTag = queryTags[i];
                    minimumSetOfItemIds = currentSet;
                }
            }

            results = new HashSet<string>(minimumSetOfItemIds);
            for (int i = 0; i < queryTags.Length; i++)
            {
                if (string.CompareOrdinal(queryTags[i], minimumTag) == 0)
                    continue;

                results.IntersectWith(this._itemTagMap[queryTags[i]]);
            }

            return results.AsValueEnumerable().ToList();
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

                this.AddItemToTagMap(item);
            }

            this.Save();
            this.OnItemAdded?.Invoke(item);
        }

        public bool RemoveItem(string itemId, int quantity = 1, bool forceRemove = false)
        {
            foreach (var categoryItemData in this.SourceData.InventoryCategoryItemData)
            {
                var categoryData = categoryItemData.Value;
                var removeStatus = categoryData.RemoveItem(itemId, quantity, forceRemove);
                if (removeStatus != ItemRemoveStatus.NotRemove)
                {
                    this.Save();
                    this.OnItemRemoved?.Invoke(itemId, quantity, forceRemove);
                }

                if (removeStatus == ItemRemoveStatus.Removed)
                    this.RemoveItemFromTagMap(itemId);

                return true;
            }

            return false;
        }

        private void AddItemToTagMap(InventoryItem item)
        {
            if (item.tags.Count <= 0) 
                return;
            
            foreach (var tag in item.tags)
            {
                if (!_itemTagMap.ContainsKey(tag))
                    _itemTagMap.Add(tag, new HashSet<string>());

                _itemTagMap[tag].Add(item.itemId);
            }
        }

        private void RemoveItemFromTagMap(string itemId)
        {
            var inventoryItem = this.GetSingleInventoryItemData(itemId);
            if (inventoryItem == null || inventoryItem.tags.Count <= 0)
                return;

            foreach (var tag in inventoryItem.tags)
            {
                if (!_itemTagMap.TryGetValue(tag, out var tagMap))
                    continue;

                tagMap.Remove(itemId);
                if (tagMap.Count <= 0)
                    _itemTagMap.Remove(tag);
            }
        }
    }
}
