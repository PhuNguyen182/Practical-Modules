namespace Foundations.UIModules.UIPresenter
{
    public interface IDataUpdater<T>
    {
        public T Data { get; }
        public void UpdateData(T data);
    }
}