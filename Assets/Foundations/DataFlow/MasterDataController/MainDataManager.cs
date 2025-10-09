using System;
using Foundations.DataFlow.MicroData.DynamicDataControllers;
using Foundations.DataFlow.MicroData.StaticDataControllers;
using Cysharp.Threading.Tasks;

namespace Foundations.DataFlow.MasterDataController
{
    public class MainDataManager : IMainDataManager
    {
        private IStaticCustomDataManager _staticCustomDataManager;
        private IDynamicCustomDataManager _dynamicCustomDataManager;
        
        public async UniTask InitializeDataHandlers()
        {
            // Initialize static data first
            _staticCustomDataManager = new StaticCustomDataManager();
            await _staticCustomDataManager.InitializeDataHandlers();
            
            // Then initialize dynamic data later
            _dynamicCustomDataManager = new DynamicCustomDataManager();
            await _dynamicCustomDataManager.InitializeDataHandlers();
        }

        public TStaticGameDataHandler GetStaticDataHandler<TStaticGameDataHandler>()
            where TStaticGameDataHandler : class, IStaticGameDataHandler
            => _staticCustomDataManager?.GetDataHandler<TStaticGameDataHandler>();

        public TDynamicGameDataHandler GetDynamicDataHandler<TDynamicGameDataHandler>()
            where TDynamicGameDataHandler : class, IDynamicGameDataHandler =>
            _dynamicCustomDataManager?.GetDataHandler<TDynamicGameDataHandler>();

        /// <summary>
        /// Save data asynchronously. Use it when a player is playing the game and wants to save data frequently
        /// or automatically in a period of time.
        /// </summary>
        /// <returns></returns>
        public UniTask SaveAllDataAsync()
        {
            if (_dynamicCustomDataManager != null) 
                return _dynamicCustomDataManager.SaveAllDataAsync();
            
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Save data synchronously. Use it when the player is out of the game or temporarily paused.
        /// </summary>
        public void SaveAllData() => _dynamicCustomDataManager?.SaveAllData();
        
        public void DeleteSingleData(Type dataType) => _dynamicCustomDataManager?.DeleteSingleData(dataType);
        
        public void DeleteAllData() => _dynamicCustomDataManager?.DeleteAllData();
        
        public void Dispose()
        {
            SaveAllData();
            _staticCustomDataManager?.Dispose();
            _dynamicCustomDataManager?.Dispose();
        }
    }
}
