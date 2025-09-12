using System;
using Cysharp.Threading.Tasks;
using Foundations.DataFlow.MicroData.DynamicDataControllers;
using Foundations.DataFlow.MicroData.StaticDataControllers;

namespace Foundations.DataFlow.MasterDataController
{
    public interface IMainDataManager : IDisposable
    {
        public TStaticGameDataHandler? GetStaticDataHandler<TStaticGameDataHandler>()
            where TStaticGameDataHandler : class, IStaticGameDataHandler;

        public TDynamicGameDataHandler? GetDynamicDataHandler<TDynamicGameDataHandler>()
            where TDynamicGameDataHandler : class, IDynamicGameDataHandler;

        public void SaveAllData();
        public UniTask SaveAllDataAsync();
        
        public void DeleteSingleData(Type dataType);
        public void DeleteAllData();
    }
}