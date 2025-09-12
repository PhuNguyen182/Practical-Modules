using UnityEngine;

namespace Foundations.UIModules.UIManager
{
    public interface IUICanvasManager
    {
        public void AddCanvas(Transform canvasTransform, UICanvasType type);
        
        public void RemoveCanvas(UICanvasType type);
        
        public Transform GetCanvas(UICanvasType type);
    }
}
