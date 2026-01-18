using System;
using System.Collections.Generic;
using Foundations.DataFlow.MasterDataController;
using Foundations.DataFlow.MicroData.DynamicDataControllers;
using Foundations.SaveSystem;
using Foundations.SaveSystem.CustomDataSaverService;
using Foundations.SaveSystem.CustomDataSerializerServices;
using PracticalSystems.GameResourceSystem.Handlers;
using PracticalSystems.GameResourceSystem.Models;

namespace PracticalSystems.GameResourceSystem.Manager
{
    public class GameResourceProgressDataController : DynamicGameDataHandler<GameResourceProgressData>
    {
        protected override GameResourceProgressData SourceData { get; set; }
        
        private readonly IDataSerializer<GameResourceProgressData> _dataSerializer =
            new JsonDataSerializer<GameResourceProgressData>();

        protected override IDataSerializer<GameResourceProgressData> DataSerializer => this._dataSerializer;

        protected override IDataSaveService<GameResourceProgressData> DataSaveService { get; }

        private readonly Dictionary<GameResourceType, BaseGameResourceHandler> _gameResourceHandlers = new();
        
        public GameResourceProgressDataController()
        {
            DataSaveService = new FileDataSaveService<GameResourceProgressData>(this._dataSerializer);   
        }
        
        public override void Initialize()
        {
            foreach (GameResourceType resourceType in Enum.GetValues(typeof(GameResourceType)))
            {
                this.SourceData.GameResourceData.TryAdd(resourceType, new ResourceData
                {
                    resourceType = resourceType,
                    amount = 0
                });
            }

            this._gameResourceHandlers.TryAdd(GameResourceType.Coin,
                new CoinResourceHandler(this.SourceData.GameResourceData[GameResourceType.Coin]));
        }

        public override void InjectDataManager(IMainDataManager mainDataManager)
        {
            
        }

        public int GetResourceAmountByType(GameResourceType resourceType)
        {
            if (this._gameResourceHandlers.TryGetValue(resourceType, out var handler))
                return handler.GetResourceAmount();
            
            return -1;
        }

        public void SetResourceAmountByType(GameResourceType resourceType, int amount)
        {
            if (!this._gameResourceHandlers.TryGetValue(resourceType, out var handler)) 
                return;
            
            handler.SetResourceAmount(amount);
            this.Save();
        }

        public void EarnResourcesByType(GameResourceType resourceType, int amount)
        {
            if (!this._gameResourceHandlers.TryGetValue(resourceType, out var handler)) 
                return;
            
            handler.EarnResources(amount);
            this.Save();
        }

        public bool CanSpendResourcesByType(GameResourceType resourceType, int amount)
        {
            return this._gameResourceHandlers.TryGetValue(resourceType, out var handler) &&
                   handler.CanSpendResources(amount);
        }

        public bool CanSpendResourcesByGroup(ResourceData[] resourceData)
        {
            for (int i = 0; i < resourceData.Length; i++)
            {
                if (!this.CanSpendResourcesByType(resourceData[i].resourceType, resourceData[i].amount))
                    return false;
            }
            
            return true;
        }

        public void SpendResourcesByType(GameResourceType resourceType, int amount)
        {
            if (!this.CanSpendResourcesByType(resourceType, amount))
            {
                Debug.LogError($"Cannot spend {resourceType}!");
                return;
            }

            BaseGameResourceHandler handler = this._gameResourceHandlers[resourceType];
            handler.SpendResources(amount);
            this.Save();
        }

        public void SpendResourcesByGroup(ResourceData[] resourceData)
        {
            if (!this.CanSpendResourcesByGroup(resourceData))
            {
                Debug.Log("Do not enough resource to spend!");
                return;
            }

            for (int i = 0; i < resourceData.Length; i++)
            {
                ResourceData resourceDataItem = resourceData[i];
                BaseGameResourceHandler handler = this._gameResourceHandlers[resourceDataItem.resourceType];
                handler.SpendResources(resourceDataItem.amount);
            }
            
            this.Save();
        }
    }
}
