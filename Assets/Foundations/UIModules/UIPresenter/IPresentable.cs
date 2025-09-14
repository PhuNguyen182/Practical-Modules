namespace Foundations.UIModules.UIPresenter
{
    public interface IPresentable
    {
        
    }
    
    public interface IPresentable<T> : IPresentable
    {
        public IUIPresenter<T> Presenter { get; }
    }
}