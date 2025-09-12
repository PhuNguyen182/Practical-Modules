using System;
using Cysharp.Threading.Tasks;
using Foundations.DataFlow.MicroData.DynamicDataControllers;

namespace Foundations.DataFlow.MasterDataController
{
    public interface IDynamicCustomDataManager : ICustomDataManager
    {
        public TDataHandler? GetDataHandler<TDataHandler>() where TDataHandler : class, IDynamicGameDataHandler;
        public void SaveAllData();
        public void DeleteAllData();
        public void DeleteSingleData(Type dataType);
        public UniTask SaveAllDataAsync();
    }
}