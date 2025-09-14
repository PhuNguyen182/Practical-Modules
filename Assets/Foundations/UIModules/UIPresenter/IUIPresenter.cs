using System;
using Foundations.UIModules.UIView;

namespace Foundations.UIModules.UIPresenter
{
    public interface IUIPresenter : IUpdatePresenter
    {
        public IUIView View { get; }
    }

    public interface IUIPresenter<T> : IUpdatePresenter<T>
    {
        public Action<T> OnDataUpdated { get; set; }
        public IUIView<T> View { get; }
    }
}
