using System;
using Cysharp.Threading.Tasks;
using Foundations.SaveSystem;

namespace Foundations.DataFlow.MicroData.DynamicDataControllers
{
    /// <summary>
    /// This class is the base class for all dynamic data type handlers.
    /// Usually use this class to handle data that change frequently, like progression data or player preference data, etc.
    /// NOTE: The SourceData class and GameDataHandler class should be split into 2 individual .cs files to make code more readable.
    /// </summary>
    /// <typeparam name="TData">Source Data is used to work with</typeparam>
    public abstract class DynamicGameDataHandler<TData> : IDynamicGameDataHandler where TData : IGameData
    {
        protected abstract TData SourceData { get; set; }
        
        /// <summary>
        /// Define how data is serialized and deserialized into the format that the data save service can handle.
        /// For example, JSON, XML, etc.
        /// </summary>
        protected abstract IDataSerializer<TData> DataSerializer { get; }
        
        /// <summary>
        /// Define where the data is saved and loaded from. For example, PlayerPrefs, File, etc.
        /// </summary>
        protected abstract IDataSaveService<TData> DataSaveService { get; }
        
        public Type DataType => typeof(TData);
        
        /// <summary>
        /// Retrieve the current data, used for other classes to access the data.
        /// </summary>
        public TData ExposedSourceData => SourceData;

        public abstract void Initialize();

        public async UniTask Load() => SourceData = await DataSaveService.LoadData(DataType.Name);
        
        public UniTask SaveAsync() => DataSaveService.SaveDataAsync(DataType.Name, SourceData);
        
        public void Save() => DataSaveService.SaveData(DataType.Name, SourceData);

        public void Delete() => DataSaveService.DeleteData(DataType.Name);
    }
}
