using System;
using Cysharp.Threading.Tasks;

namespace Foundations.DataFlow.MasterDataController
{
    public interface ICustomDataManager : IDisposable
    {
        public UniTask InitializeDataHandlers();
    }
}
