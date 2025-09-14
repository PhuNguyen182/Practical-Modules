using System;

namespace Foundations.UIModules.UIView
{
    public interface IShowable
    {
        public Action OnViewShow { get; set; }
        
        public void Show();
    }
}