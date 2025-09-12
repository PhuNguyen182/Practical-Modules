using System;

namespace Foundations.UIModules.Popups
{
    public interface IPopup
    {
        public void Close();
    }
    
    public interface IPopup<in TModel> : IPopup
    {
        public void BindData(TModel modelData);
        public void SetOnOpenAction(Action onOpenAction);
        public void SetOnCloseAction(Action onCloseAction);
    }
}