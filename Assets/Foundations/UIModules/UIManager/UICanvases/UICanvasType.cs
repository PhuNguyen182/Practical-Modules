using System;

namespace Foundations.UIModules.UIManager.UICanvases
{
    /// <summary>
    /// Enumeration of different canvas types in the UI system
    /// </summary>
    [Serializable]
    public enum UICanvasType
    {
        /// <summary>
        /// Default value. Do not use this value
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Main UI canvas for normal UI elements
        /// </summary>
        Main = 1,
        
        /// <summary>
        /// Canvas for popups with stacking support
        /// </summary>
        Popup = 2,
        
        /// <summary>
        /// Canvas for overlays that block input
        /// </summary>
        Overlay = 3,
        
        /// <summary>
        /// Canvas for loading screens
        /// </summary>
        Loading = 4,
        
        /// <summary>
        /// Canvas for notifications and alerts
        /// </summary>
        Notification = 5,
        
        /// <summary>
        /// Canvas for debugging and developer tools
        /// </summary>
        Debug = 6,
    }
}
