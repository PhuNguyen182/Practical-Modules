using System;
using System.Collections.Generic;
using UnityEngine;

namespace Foundations.UIModules.UIManager.UICanvases
{
    /// <summary>
    /// Interface for managing UI canvases in the MVP system
    /// </summary>
    public interface IUICanvasManager
    {
    /// <summary>
        /// Event fired when a canvas is created
        /// </summary>
        public Action<UICanvasType, Canvas> OnCanvasCreated { get; set; }
        
        /// <summary>
        /// Event fired when a canvas is destroyed
        /// </summary>
        public Action<UICanvasType> OnCanvasDestroyed { get; set; }
        
        /// <summary>
        /// Initialize the canvas manager
        /// </summary>
        public void Initialize();
        
        /// <summary>
        /// Get a canvas by type
        /// </summary>
        /// <param name="canvasType">Type of canvas to get</param>
        /// <returns>The canvas or null if not found</returns>
        public Canvas GetCanvas(UICanvasType canvasType);
        
        /// <summary>
        /// Create a new canvas
        /// </summary>
        /// <param name="canvasType">Type of canvas to create</param>
        /// <param name="config">Configuration for the canvas</param>
        /// <returns>The created canvas</returns>
        public Canvas CreateCanvas(CanvasConfig config = null);
        
        /// <summary>
        /// Destroy a canvas
        /// </summary>
        /// <param name="canvasType">Type of canvas to destroy</param>
        void DestroyCanvas(UICanvasType canvasType);
        
        /// <summary>
        /// Check if a canvas exists
        /// </summary>
        /// <param name="canvasType">Type of canvas to check</param>
        /// <returns>True if canvas exists</returns>
        bool HasCanvas(UICanvasType canvasType);
        
        /// <summary>
        /// Get all active canvases
        /// </summary>
        /// <returns>Dictionary of canvas types and their canvases</returns>
        public Dictionary<UICanvasType, Canvas> GetAllCanvases();
        
        /// <summary>
        /// Set the sort order of a canvas
        /// </summary>
        /// <param name="canvasType">Type of canvas</param>
        /// <param name="sortOrder">New sort order</param>
        public void SetCanvasSortOrder(UICanvasType canvasType, int sortOrder);
        
        /// <summary>
        /// Show/hide a canvas
        /// </summary>
        /// <param name="canvasType">Type of canvas</param>
        /// <param name="active">Whether to show or hide</param>
        public void SetCanvasActive(UICanvasType canvasType, bool active);
        
        /// <summary>
        /// Clean up all canvases
        /// </summary>
        public void Cleanup();
    }
}
