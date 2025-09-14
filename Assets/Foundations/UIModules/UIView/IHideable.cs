using System;

namespace Foundations.UIModules.UIView
{
    public interface IHideable
    {
        public Action OnViewHide { get; set; }
        
        public void Hide();
    }
}