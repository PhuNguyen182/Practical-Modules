using System;
using UnityEngine;
using UnityEngine.UI;

namespace Foundations.UIModules.UIManager.UICanvases
{
    [Serializable]
    public struct CanvasSettings
    {
        public UICanvasType canvasType;
        
        [Header("Canvas Settings")]
        public RenderMode canvasRenderMode;
        public SortingLayer SortingLayer;
        
        [Header("Canvas Scaler Settings")]
        public CanvasScaler.ScreenMatchMode screenMatchMode;
        public Vector2 referenceResolution;
        public float matchWidthOrHeight;
    }
}