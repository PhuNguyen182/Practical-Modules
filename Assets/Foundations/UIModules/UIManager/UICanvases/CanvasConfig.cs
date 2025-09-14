using System;

namespace Foundations.UIModules.UIManager.UICanvases
{
    /// <summary>
    /// Canvas configuration data
    /// </summary>
    [Serializable]
    public class CanvasConfig
    {
        public UICanvasType canvasType;
        public int sortOrder;
        public bool isPersistent;
        public string canvasName;
        
        public CanvasConfig(UICanvasType type, int order = 0, bool persistent = false, string name = null)
        {
            canvasType = type;
            sortOrder = order;
            isPersistent = persistent;
            canvasName = name ?? $"{type} Canvas";
        }
    }
}