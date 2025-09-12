using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Foundations.DataFlow.ProcessingSequence
{
    public class DataSequenceProcessor : IDataSequenceProcessor
    {
        private readonly Queue<IProcessSequence> _processSequences = new();
        public IProcessSequence LatestProcessSequence { get; private set; }

        public IDataSequenceProcessor Append(IProcessSequence processSequence)
        {
            _processSequences.Enqueue(processSequence);
            return this;
        }
        
        public async UniTask Execute()
        {
            foreach (IProcessSequence processSequence in _processSequences)
            {
                await processSequence.Process();
                LatestProcessSequence = processSequence;
                if (processSequence.IsFinished)
                    break;
            }
            
            _processSequences.Clear();
        }
    }
}
