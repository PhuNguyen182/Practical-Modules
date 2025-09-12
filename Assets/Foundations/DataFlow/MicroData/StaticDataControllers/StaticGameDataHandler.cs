using System;
using System.Collections.Generic;
using Foundations.DataFlow.ProcessingSequence;
using Foundations.DataFlow.ProcessingSequence.CustomDataProcessor;
using Cysharp.Threading.Tasks;

namespace Foundations.DataFlow.MicroData.StaticDataControllers
{
    /// <summary>
    /// This class is the base class for all static data type handlers.
    /// Usually use this class to handle data that does not change in the whole game cycle, like configuration data, etc.
    /// IMPORTANCE: The SourceData class and GameDataHandler class should (or must) be split into 2 individual .cs files
    /// to ensure in the case of using ScriptableObject as the SourceData class will work properly.
    /// </summary>
    /// <typeparam name="TData">Source Data is used to work with</typeparam>
    public abstract class StaticGameDataHandler<TData> : IStaticGameDataHandler where TData : class, IGameData
    {
        private IDataSequenceProcessor _dataSequenceProcessor;
        protected abstract TData SourceData { get; set; }
        
        public Type DataType => typeof(TData);
        
        /// <summary>
        /// Retrieve the current data, used for other classes to access the data.
        /// </summary>
        public TData ExposedSourceData => SourceData;
        
        /// <summary>
        /// Define how data is processed in the order that it is defined.
        /// </summary>
        public abstract List<DataProcessSequence> DataProcessSequences { get; }
        
        public async UniTask Initialize()
        {
            _dataSequenceProcessor = new DataSequenceProcessor();
            foreach (DataProcessSequence dataProcessSequence in DataProcessSequences)
            {
                IProcessSequence processSequence = GetDataProcessorByType(dataProcessSequence);
                if (processSequence == null)
                    continue;

                _dataSequenceProcessor.Append(processSequence);
            }

            await _dataSequenceProcessor.Execute();
            if (_dataSequenceProcessor.LatestProcessSequence is IProcessSequenceData processSequenceData)
                SourceData = processSequenceData.GameData as TData;
        }

        /// <summary>
        /// Get the data key from the GameDataAttribute. If not found, use the type name.
        /// </summary>
        /// <returns>Return the data key</returns>
        protected static string GetDataKey()
        {
            GameDataAttribute attribute = GetAttribute<TData>();
            return attribute?.DataKey ?? typeof(TData).Name;
        }

        private static GameDataAttribute GetAttribute<T>() where T : IGameData =>
            (GameDataAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(GameDataAttribute));

        private IProcessSequence GetDataProcessorByType(DataProcessSequence dataProcessSequence)
        {
            string dataKey = dataProcessSequence.DataKey;
            IProcessSequence processSequence = dataProcessSequence.DataProcessorType switch
            {
                DataProcessorType.FirebaseRemoteConfig => new FirebaseRemoteConfigDataProcessor(dataKey, DataType),
                DataProcessorType.ScriptableObjects => new ScriptableObjectDataProcessor(dataKey, DataType),
                _ => null    
            };
            
            return processSequence;
        }
    }
}
