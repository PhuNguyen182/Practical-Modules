using System.IO;
using Cysharp.Threading.Tasks;
using PracticalModules.TypeCreator.Core;
using UnityEngine;

namespace Foundations.SaveSystem.CustomDataSaverService
{
    /// <summary>
    /// Use this class to save data to files.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FileDataSaveService<T> : IDataSaveService<T>
    {
        private const string LocalDataPrefix = "GameData";
        
        private readonly IDataSerializer<T> _dataSerializer;
        private readonly string _filePath;
        private readonly string _fileExtension;

        public FileDataSaveService(IDataSerializer<T> dataSerializer)
        {
            _dataSerializer = dataSerializer;
            _filePath = Application.persistentDataPath;
            _fileExtension = _dataSerializer.FileExtension;
        }

        public async UniTask<T> LoadData(string name)
        {
            string dataPath = GetDataPath(name);
            if (!File.Exists(dataPath))
                return TypeFactory.Create<T>();

            using StreamReader streamReader = new(dataPath);
            string serializedData = await streamReader.ReadToEndAsync();
            T data = _dataSerializer.Deserialize(serializedData);
            return data;
        }

        public async UniTask SaveDataAsync(string name, T data)
        {
            string dataPath = GetDataPath(name);
            string directoryPath = GetDirectoryPath();
            
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string serializedData = _dataSerializer.Serialize(data);
            await using FileStream fileStream = new(dataPath, FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true);
            await using StreamWriter writer = new(fileStream);
            await writer.WriteLineAsync(serializedData);
        }

        public void SaveData(string name, T data)
        {
            string dataPath = GetDataPath(name);
            string directoryPath = GetDirectoryPath();
            
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string serializedData = _dataSerializer.Serialize(data);
            using FileStream fileStream = new(dataPath, FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: false);
            using StreamWriter writer = new(fileStream);
            writer.WriteLineAsync(serializedData);
        }

        public void DeleteData(string name)
        {
            string dataPath = GetDataPath(name);
            if (!File.Exists(dataPath))
                return;
            
            File.Delete(dataPath);
        }
        
        private string GetDataPath(string name)
        {
            string dataPath = Path.Combine(_filePath, LocalDataPrefix, $"{name}{_fileExtension}");
            return dataPath;
        }
        
        private string GetDirectoryPath() => Path.Combine(_filePath, LocalDataPrefix);
    }
}
