using Cysharp.Threading.Tasks;
using Foundations.DataFlow.MicroData;

namespace Foundations.DataFlow.ProcessingSequence
{
    public interface IProcessSequence
    {
        public bool IsFinished { get; }
        
        public UniTask Process();
    }
    
    public interface IProcessSequenceData
    {
        public IGameData GameData { get; }
    }
}