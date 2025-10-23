using System;
using System.Collections.Generic;
using Foundations.SaveSystem;
using Foundations.SaveSystem.CustomDataSaverService;
using Foundations.SaveSystem.CustomDataSerializerServices;
using Foundations.DataFlow.MicroData.DynamicDataControllers;
using PracticalModules.GameResourceSystem.Handlers;
using PracticalModules.GameResourceSystem.Models;

namespace PracticalModules.GameResourceSystem.Manager
{
    public class GameResourceProgressDataController : DynamicGameDataHandler<GameResourceProgressData>
    {
        protected override GameResourceProgressData SourceData { get; set; }

        protected override IDataSerializer<GameResourceProgressData> DataSerializer =>
            new JsonDataSerializer<GameResourceProgressData>();

        protected override IDataSaveService<GameResourceProgressData> DataSaveService =>
            new FileDataSaveService<GameResourceProgressData>(DataSerializer);

        private readonly Dictionary<GameResourceType, BaseGameResourceHandler> _gameResourceHandlers = new();
        
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
        
        public void SpendResourcesByType(GameResourceType resourceType, int amount)
        {
            if (!this._gameResourceHandlers.TryGetValue(resourceType, out var handler))
            {
                Debug.Log($"This resource type ({resourceType}) does not exist.");
                return;
            }

            if (handler.CanSpendResources(amount))
            {
                handler.SpendResources(amount);
                this.Save();
            }
            
            int currentAmount = handler.GetResourceAmount();
            int needMoreAmountToSpent = amount - currentAmount;
            Debug.Log($"Do not enough resource! You need more {needMoreAmountToSpent} {resourceType} to spend!");
        }
    }
}
