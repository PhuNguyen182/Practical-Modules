using System;
using System.Collections.Generic;
using Foundations.DataFlow.ProcessingSequence;
using Cysharp.Threading.Tasks;
using Foundations.DataFlow.MasterDataController;

namespace Foundations.DataFlow.MicroData.StaticDataControllers
{
    public interface IStaticGameDataHandler
    {
        public UniTask Initialize();
        public void InjectDataManager(IMainDataManager mainDataManager);
    }
}
