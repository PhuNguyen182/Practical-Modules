namespace Foundations.UIModules.UIPresenter
{
    public interface IUpdatePresenter
    {
        public void UpdatePresenter();
    }

    public interface IUpdatePresenter<T>
    {
        public T Data { get; }
        public void UpdatePresenter(T data);
    }
}