using System;
using Foundations.UIModules.Popups;
using UnityEngine;

namespace Foundations.UIModules.UIManager
{
    public interface IUIPopupManager
    {
        public TPopup ShowPopup<TPopup>(string popupName, Transform popupContainer = null,
            Action onPopupOpenAction = null, Action onPopupCloseAction = null)
            where TPopup : IPopup;
        
        public TPopup ShowPopup<TPopup, TModel>(string popupName, TModel modelData, Transform popupContainer = null,
            Action onPopupOpenAction = null, Action onPopupCloseAction = null)
            where TPopup : IPopup<TModel>;
        
        public void ClosePopup(string popupName);
        
        public void CloseCurrentPopup();
        
        public void CloseAllPopups();
    }
}
