using System.Collections.Generic;
using Foundations.DataFlow.MasterDataController;
using Foundations.DataFlow.MicroData.StaticDataControllers;
using Foundations.DataFlow.ProcessingSequence;
using PracticalSystems.InventorySystem.Models.Items;
using PracticalSystems.InventorySystem.Models.Set;
using ZLinq;

namespace PracticalSystems.InventorySystem.Manager
{
    public class ItemSetConfigDataController : StaticGameDataHandler<ItemSetConfigData>
    {
        protected override ItemSetConfigData SourceData { get; set; }
        public override List<DataProcessSequence> DataProcessSequences => new()
        {
            new DataProcessSequence(this.GetDataKey(), DataProcessorType.ScriptableObjects)
        };
        
        private Dictionary<string, ItemSetData> _itemSetData = new();

        public override void InjectDataManager(IMainDataManager mainDataManager)
        {
            
        }

        protected override void OnDataInitialized()
        {
            this._itemSetData = this.SourceData.itemSetData.AsValueEnumerable()
                .ToDictionary(key => key.setId, value => value);
        }

        public List<ItemSetData> GetAllItemSetData()
        {
            return this.SourceData.itemSetData;
        }

        public ItemSetData GetItemSetConfigData(string setId)
        {
            return this._itemSetData[setId];
        }

        public bool IsValidItemSet(List<ItemData> itemSetData)
        {
            string setName = this.GetSetNameFromItemCollection(itemSetData);
            if (string.IsNullOrEmpty(setName) || !this._itemSetData.TryGetValue(setName, out var setInfo))
                return false;

            foreach (var itemData in itemSetData)
            {
                if (!setInfo.requiredItemIds.Contains(itemData.setId))
                    return false;
            }
            
            return true;
        }

        private string GetSetNameFromItemCollection(List<ItemData> itemSetData)
        {
            bool haveSameSetId = HaveSameSetId(itemSetData);
            return haveSameSetId ? itemSetData[0].setId : null;
        }
        
        private bool HaveSameSetId(List<ItemData> itemSetData)
        {
            var firstSetItem = itemSetData[0].setId;
            if (string.IsNullOrEmpty(firstSetItem))
                return false;
            
            foreach (var itemData in itemSetData)
            {
                if (string.IsNullOrEmpty(itemData.setId))
                    return false;
                
                bool isDifferenceSet = string.CompareOrdinal(firstSetItem, itemData.setId) != 0;
                if (isDifferenceSet)
                    return false;
            }
            
            return true;
        }
    }
}
