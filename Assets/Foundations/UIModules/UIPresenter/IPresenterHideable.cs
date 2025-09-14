using System;

namespace Foundations.UIModules.UIPresenter
{
    public interface IPresenterHideable
    {
        public Action OnHide { get; set; }
        
        public void Hide();
    }
}