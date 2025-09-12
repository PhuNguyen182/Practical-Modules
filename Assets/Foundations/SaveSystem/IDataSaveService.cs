using Cysharp.Threading.Tasks;

namespace Foundations.SaveSystem
{
    public interface IDataSaveService<T>
    {
        public UniTask<T> LoadData(string name);
        public UniTask SaveDataAsync(string name, T data);
        public void SaveData(string name, T data);
        public void DeleteData(string name);
    }
}
