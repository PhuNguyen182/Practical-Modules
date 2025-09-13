using System;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.MVP
{
    /// <summary>
    /// Interface for managing UI overlays that block input
    /// </summary>
    public interface IUIOverlayManager
    {
        /// <summary>
        /// Event fired when an overlay is shown
        /// </summary>
        event Action<string> OnOverlayShown;
        
        /// <summary>
        /// Event fired when an overlay is hidden
        /// </summary>
        event Action<string> OnOverlayHidden;
        
        /// <summary>
        /// Show an overlay with blocking
        /// </summary>
        /// <param name="overlayId">Unique identifier for the overlay</param>
        /// <param name="color">Color of the overlay (default: transparent black)</param>
        /// <param name="blockInput">Whether to block input</param>
        /// <param name="priority">Priority for stacking</param>
        void ShowOverlay(string overlayId, Color? color = null, bool blockInput = true, int priority = 0);
        
        /// <summary>
        /// Hide an overlay
        /// </summary>
        /// <param name="overlayId">ID of overlay to hide</param>
        /// <param name="animate">Whether to animate the hide</param>
        void HideOverlay(string overlayId, bool animate = true);
        
        /// <summary>
        /// Hide all overlays
        /// </summary>
        /// <param name="animate">Whether to animate the hide</param>
        void HideAllOverlays(bool animate = true);
        
        /// <summary>
        /// Check if an overlay is currently shown
        /// </summary>
        /// <param name="overlayId">ID of overlay to check</param>
        /// <returns>True if overlay is shown</returns>
        bool IsOverlayShown(string overlayId);
        
        /// <summary>
        /// Get all active overlay IDs
        /// </summary>
        /// <returns>List of active overlay IDs</returns>
        List<string> GetActiveOverlayIds();
        
        /// <summary>
        /// Check if any overlay is currently blocking input
        /// </summary>
        /// <returns>True if input is blocked</returns>
        bool IsInputBlocked();
        
        /// <summary>
        /// Get the top overlay (highest priority)
        /// </summary>
        /// <returns>Top overlay ID or null if none</returns>
        string GetTopOverlay();
        
        /// <summary>
        /// Set overlay priority
        /// </summary>
        /// <param name="overlayId">ID of overlay</param>
        /// <param name="priority">New priority</param>
        void SetOverlayPriority(string overlayId, int priority);
        
        /// <summary>
        /// Initialize the overlay manager
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Clean up all overlays
        /// </summary>
        void Cleanup();
    }
}
