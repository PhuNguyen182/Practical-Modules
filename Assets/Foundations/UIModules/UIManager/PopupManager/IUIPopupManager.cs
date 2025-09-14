using System;
using System.Collections.Generic;
using Foundations.UIModules.Popups;

namespace Foundations.UIModules.UIManager.PopupManager
{
    public interface IUIPopupManager<TPopupViewData, TPresenterData>
    {
        public void Initialize();
        public SimplePopup CreatePopup(string popupId, Action onPopupOpened = null, Action onPopupClosed = null);

        public BasePopupPresenter<TPopupViewData, TPresenterData> ShowPopup(string popupId,
            TPresenterData presenterData, Action onPopupOpened = null, Action onPopupClosed = null);

        public void ClosePopup(string popupId);
        public void CloseAllPopups();
        public void CloseAllPopupsExcept(string keepPopupId);
        public BasePopupPresenter<TPopupViewData, TPresenterData> GetPopupInfo(string popupId);
        public bool IsPopupShown(string popupId);
        public List<BasePopupPresenter<TPopupViewData, TPresenterData>> GetAllShownPopups();
        public BasePopupPresenter<TPopupViewData, TPresenterData> GetTopPopup();
        public List<BasePopupPresenter<TPopupViewData, TPresenterData>> GetPopupStack();
        public void BringPopupToFront(string popupId);
        public void SetPopupPriority(string popupId, int priority);
        public void Cleanup();
    }
}
