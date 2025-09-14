using Foundations.UIModules.UIPresenter;

namespace Foundations.UIModules.UIView
{
    public abstract class BaseUIViewGeneric<T>: BaseUIView, IUIView<T>, IPresentable<T>
    {
        public T Data { get; private set; }
        
        public abstract IUIPresenter<T> Presenter { get; }
        
        public virtual void UpdateData(T data)
        {
            Data = data;
        }
    }
}
