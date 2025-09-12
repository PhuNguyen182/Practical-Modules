namespace Foundations.SaveSystem
{
    public interface IDataSerializer<T>
    {
        public string FileExtension { get; }
        public string Serialize(T data);
        public T Deserialize(string name);
    }
}
