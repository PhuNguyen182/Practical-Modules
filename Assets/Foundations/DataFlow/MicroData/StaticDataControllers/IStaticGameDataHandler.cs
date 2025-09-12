using System;
using System.Collections.Generic;
using Foundations.DataFlow.ProcessingSequence;
using Cysharp.Threading.Tasks;

namespace Foundations.DataFlow.MicroData.StaticDataControllers
{
    public interface IStaticGameDataHandler
    {
        public UniTask Initialize();
    }
}
