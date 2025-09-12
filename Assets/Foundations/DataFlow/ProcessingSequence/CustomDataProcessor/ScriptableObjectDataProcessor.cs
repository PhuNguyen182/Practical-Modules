using System;
using Cysharp.Threading.Tasks;
using Foundations.DataFlow.MicroData;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Foundations.DataFlow.ProcessingSequence.CustomDataProcessor
{
    public class ScriptableObjectDataProcessor : IProcessSequence, IProcessSequenceData
    {
        private readonly Type _desiredDataType;
        private readonly string _dataConfigKey;
        
        public IGameData GameData { get; private set; }
        public bool IsFinished { get; private set; }
        
        public ScriptableObjectDataProcessor(string dataConfigKey, Type desiredDataType)
        {
            _dataConfigKey = dataConfigKey;
            _desiredDataType = desiredDataType;
        }

        public async UniTask Process()
        {
            // Use load from the Resources folder to get the scriptable object for target config data
            Object data = await Resources.LoadAsync(_dataConfigKey);
            if (data != null && _desiredDataType.IsInstanceOfType(data) &&
                typeof(IGameData).IsAssignableFrom(_desiredDataType))
            {
                GameData = data as IGameData;
                IsFinished = true;
            }
        }
    }
}
