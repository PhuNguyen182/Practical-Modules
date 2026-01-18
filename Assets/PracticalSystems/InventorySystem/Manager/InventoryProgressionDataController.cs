using System;
using System.Collections.Generic;
using Foundations.DataFlow.MasterDataController;
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

        private readonly IDataSerializer<InventoryProgressionData> _dataSerializer =
            new JsonDataSerializer<InventoryProgressionData>();

        protected override IDataSerializer<InventoryProgressionData> DataSerializer => this._dataSerializer;

        protected override IDataSaveService<InventoryProgressionData> DataSaveService { get; }

        private ItemData _cachedItemData;
        private InventoryItem _cachedInventoryItem;
        private readonly Dictionary<string, HashSet<int>> _itemTagMap = new();
        private InventoryConfigDataController _inventoryConfigDataController;

        public InventoryProgressionDataController()
        {
            DataSaveService = new FileDataSaveService<InventoryProgressionData>(this._dataSerializer);
        }

        public event Action<InventoryItem> OnItemAdded;
        public event Action<int, int, bool> OnItemRemoved;

        public override void InjectDataManager(IMainDataManager mainDataManager)
        {
            this._inventoryConfigDataController = mainDataManager.GetStaticDataHandler<InventoryConfigDataController>();
        }

        public override void Initialize()
        {
            var allItemData = this.GetAllInventoryItemData();
            for (int i = 0; i < allItemData.Count; i++)
            {
                this.AddItemToTagMap(allItemData[i]);
            }

            this.Save();
        }

        public bool HasItem(int itemId)
        {
            var itemData = this.GetSingleInventoryItemData(itemId);
            return itemData != null;
        }

        public InventoryItem GetSingleInventoryItemData(int itemId)
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

        public List<int> GetItemIdsByTags(params string[] queryTags)
        {
            if (queryTags is not { Length: > 0 })
                return null;

            using var smallestSet = HashSetPool<int>.Get(out var minimumSetOfItemIds);
            using var checkingSet = HashSetPool<int>.Get(out var currentSet);
            using var resultSet = HashSetPool<int>.Get(out var results);

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

            results = new HashSet<int>(minimumSetOfItemIds);
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
                Dictionary<int, InventoryItem> itemData = new()
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

        public void UpdateItem(InventoryItem item, bool forceUpdate = false)
        {
            if (!this.SourceData.InventoryCategoryItemData.ContainsKey(item.itemCategory)) 
                return;
            
            if (!this.SourceData.InventoryCategoryItemData[item.itemCategory].ItemData.ContainsKey(item.itemId)) 
                return;
            
            this.SourceData.InventoryCategoryItemData[item.itemCategory].ItemData[item.itemId] = item;
            if (forceUpdate)
                this.Save();
        }

        public bool RemoveItem(int itemId, int quantity = 1, bool forceRemove = false)
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
            _cachedItemData = this._inventoryConfigDataController.GetItemData(item);
            if (_cachedItemData.tags.Count <= 0)
                return;

            for (int i = 0; i < _cachedItemData.tags.Count; i++)
            {
                if (!_itemTagMap.ContainsKey(_cachedItemData.tags[i]))
                    _itemTagMap.Add(_cachedItemData.tags[i], new HashSet<int>());

                _itemTagMap[_cachedItemData.tags[i]].Add(item.itemId);
            }
        }

        private void RemoveItemFromTagMap(int itemId)
        {
            _cachedInventoryItem = this.GetSingleInventoryItemData(itemId);
            if (_cachedInventoryItem == null)
                return;

            _cachedItemData = this._inventoryConfigDataController.GetItemData(_cachedInventoryItem);
            if (_cachedItemData == null || _cachedItemData.tags.Count <= 0)
                return;

            for (int i = 0; i < _cachedItemData.tags.Count; i++)
            {
                if (!_itemTagMap.TryGetValue(_cachedItemData.tags[i], out var tagMap))
                    continue;

                tagMap.Remove(itemId);
                if (tagMap.Count <= 0)
                    _itemTagMap.Remove(_cachedItemData.tags[i]);
            }
        }
    }
}
