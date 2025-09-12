using System;
using Cysharp.Threading.Tasks;
using Foundations.DataFlow.MicroData;

namespace Foundations.DataFlow.ProcessingSequence.CustomDataProcessor
{
    public class FirebaseRemoteConfigDataProcessor : IProcessSequence, IProcessSequenceData
    {
        private readonly Type _desiredDataType;
        private readonly string _remoteConfigKey;
        
        public bool IsFinished { get; private set; }
        public IGameData GameData { get; private set; }

        public FirebaseRemoteConfigDataProcessor(string remoteConfigKey, Type desiredDataType)
        {
            _remoteConfigKey = remoteConfigKey;
            _desiredDataType = desiredDataType;
        }

        public async UniTask Process()
        {
            // To do: This part of the function should add logic to get remote data from Firebase Remote Config
            // If the remote config get the desired value from the passed key successfully, set the IsFinished to true 
            // Firebase remote config or any remote config always use JSON serializer, so does not matter what ICustomSerializer are using
            await UniTask.CompletedTask;
        }

    }
}
