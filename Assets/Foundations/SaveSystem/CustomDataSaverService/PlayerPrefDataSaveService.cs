using UnityEngine;
using Cysharp.Threading.Tasks;
using PracticalModules.TypeCreator.Core;

namespace Foundations.SaveSystem.CustomDataSaverService
{
    /// <summary>
    /// Use this class to save data to PlayerPrefs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PlayerPrefDataSaveService<T> : IDataSaveService<T>
    {
        private readonly IDataSerializer<T> _dataSerializer;

        public PlayerPrefDataSaveService(IDataSerializer<T> dataSerializer)
        {
            _dataSerializer = dataSerializer;
        }

        public async UniTask<T> LoadData(string name)
        {
            if (!PlayerPrefs.HasKey(name))
                return TypeFactory.Create<T>();

            await UniTask.CompletedTask;
            string serializedData = PlayerPrefs.GetString(name);
            T data = _dataSerializer.Deserialize(serializedData);
            return data;
        }

        public UniTask SaveDataAsync(string name, T data)
        {
            string serializedData = _dataSerializer.Serialize(data);
            PlayerPrefs.SetString(name, serializedData);
            return UniTask.CompletedTask;
        }

        public void SaveData(string name, T data)
        {
            string serializedData = _dataSerializer.Serialize(data);
            PlayerPrefs.SetString(name, serializedData);
        }

        public void DeleteData(string name) => PlayerPrefs.DeleteKey(name);
    }
}
