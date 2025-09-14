using Foundations.UIModules.UIPresenter;
using UnityEngine;

namespace Foundations.UIModules.Popups
{
    public class SimplePopup : MonoBehaviour, IPopupView
    {
        public IUIPresenter Presenter { get; }
    }
}