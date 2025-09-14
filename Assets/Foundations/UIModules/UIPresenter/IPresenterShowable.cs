using System;

namespace Foundations.UIModules.UIPresenter
{
    public interface IPresenterShowable
    {
        public Action OnShow { get; set; }
        
        public void Show();
    }
}