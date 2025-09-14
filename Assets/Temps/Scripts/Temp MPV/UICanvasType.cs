using System;

namespace UISystem.MVP
{
    /// <summary>
    /// Enumeration of different canvas types in the UI system
    /// </summary>
    [Serializable]
    public enum UICanvasType
    {
        /// <summary>
        /// Main UI canvas for normal UI elements
        /// </summary>
        Main,
        
        /// <summary>
        /// Canvas for popups with stacking support
        /// </summary>
        Popup,
        
        /// <summary>
        /// Canvas for overlays that block input
        /// </summary>
        Overlay,
        
        /// <summary>
        /// Canvas for loading screens
        /// </summary>
        Loading,
        
        /// <summary>
        /// Canvas for notifications and alerts
        /// </summary>
        Notification,
        
        /// <summary>
        /// Canvas for debugging and developer tools
        /// </summary>
        Debug
    }

    /// <summary>
    /// Canvas configuration data
    /// </summary>
    [Serializable]
    public class CanvasConfig
    {
        public UICanvasType canvasType;
        public int sortOrder;
        public bool isPersistent;
        public string canvasName = string.Empty;
        
        public CanvasConfig(UICanvasType type, int order = 0, bool persistent = false, string? name = null)
        {
            canvasType = type;
            sortOrder = order;
            isPersistent = persistent;
            canvasName = name ?? type.ToString() + "Canvas";
        }
    }
}
