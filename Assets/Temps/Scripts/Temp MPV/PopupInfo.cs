using System;
using UnityEngine;

namespace UISystem.MVP
{
    /// <summary>
    /// Information about a popup in the system
    /// </summary>
    [Serializable]
    public class PopupInfo
    {
        public string popupId;
        public string popupType;
        public int priority;
        public bool isModal;
        public bool canCloseOnBackgroundClick;
        public bool destroyOnClose;
        public object? data;
        public IPresenter? presenter;
        
        public PopupInfo(string id, string type, int priority = 0, bool modal = false, bool closeOnBackground = true, bool destroyOnClose = true, object? data = null)
        {
            popupId = id;
            popupType = type;
            this.priority = priority;
            isModal = modal;
            canCloseOnBackgroundClick = closeOnBackground;
            this.destroyOnClose = destroyOnClose;
            this.data = data;
        }
        
        public override string ToString()
        {
            return $"PopupInfo[{popupType}]: {popupId} (Priority: {priority}, Modal: {isModal})";
        }
    }

    /// <summary>
    /// Popup display modes
    /// </summary>
    public enum PopupDisplayMode
    {
        /// <summary>
        /// Normal popup display
        /// </summary>
        Normal,
        
        /// <summary>
        /// Modal popup that blocks interaction with other UI
        /// </summary>
        Modal,
        
        /// <summary>
        /// Toast notification style popup
        /// </summary>
        Toast,
        
        /// <summary>
        /// Full screen popup
        /// </summary>
        FullScreen
    }

    /// <summary>
    /// Popup animation types
    /// </summary>
    public enum PopupAnimationType
    {
        None,
        Fade,
        Scale,
        SlideFromTop,
        SlideFromBottom,
        SlideFromLeft,
        SlideFromRight,
        Custom
    }
}
