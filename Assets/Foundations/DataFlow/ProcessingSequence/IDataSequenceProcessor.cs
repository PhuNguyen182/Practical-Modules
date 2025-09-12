using Cysharp.Threading.Tasks;

namespace Foundations.DataFlow.ProcessingSequence
{
    public interface IDataSequenceProcessor
    {
        public IProcessSequence LatestProcessSequence { get; }
        public IDataSequenceProcessor Append(IProcessSequence processSequence);
        public UniTask Execute();
    }
}