using System;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.MVP
{
    /// <summary>
    /// Interface for managing popups in the MVP system
    /// </summary>
    public interface IUIPopupManager
    {
        /// <summary>
        /// Event fired when a popup is shown
        /// </summary>
        event Action<PopupInfo> OnPopupShown;
        
        /// <summary>
        /// Event fired when a popup is hidden
        /// </summary>
        event Action<PopupInfo> OnPopupHidden;
        
        /// <summary>
        /// Event fired when a popup is closed
        /// </summary>
        event Action<PopupInfo> OnPopupClosed;
        
        /// <summary>
        /// Event fired when popup stack changes
        /// </summary>
        event Action<List<PopupInfo>> OnPopupStackChanged;
        
        /// <summary>
        /// Show a popup
        /// </summary>
        /// <param name="popupId">Unique identifier for the popup</param>
        /// <param name="popupType">Type of popup to show</param>
        /// <param name="data">Data to pass to the popup</param>
        /// <param name="priority">Priority for stacking (higher = on top)</param>
        /// <param name="modal">Whether the popup is modal</param>
        /// <returns>The popup info</returns>
        PopupInfo? ShowPopup(string popupId, string popupType, object? data = null, int priority = 0, bool modal = false);
        
        /// <summary>
        /// Hide a popup
        /// </summary>
        /// <param name="popupId">ID of popup to hide</param>
        /// <param name="animate">Whether to animate the hide</param>
        void HidePopup(string popupId, bool animate = true);
        
        /// <summary>
        /// Close a popup (hide and optionally destroy)
        /// </summary>
        /// <param name="popupId">ID of popup to close</param>
        /// <param name="animate">Whether to animate the close</param>
        void ClosePopup(string popupId, bool animate = true);
        
        /// <summary>
        /// Close all popups
        /// </summary>
        /// <param name="animate">Whether to animate the close</param>
        void CloseAllPopups(bool animate = true);
        
        /// <summary>
        /// Close all popups except the specified one
        /// </summary>
        /// <param name="keepPopupId">ID of popup to keep open</param>
        /// <param name="animate">Whether to animate the close</param>
        void CloseAllPopupsExcept(string keepPopupId, bool animate = true);
        
        /// <summary>
        /// Get popup info by ID
        /// </summary>
        /// <param name="popupId">ID of popup</param>
        /// <returns>Popup info or null if not found</returns>
        PopupInfo GetPopupInfo(string popupId);
        
        /// <summary>
        /// Check if a popup is currently shown
        /// </summary>
        /// <param name="popupId">ID of popup to check</param>
        /// <returns>True if popup is shown</returns>
        bool IsPopupShown(string popupId);
        
        /// <summary>
        /// Get all currently shown popups
        /// </summary>
        /// <returns>List of popup infos</returns>
        List<PopupInfo> GetAllShownPopups();
        
        /// <summary>
        /// Get the top popup (highest priority)
        /// </summary>
        /// <returns>Top popup info or null if none</returns>
        PopupInfo GetTopPopup();
        
        /// <summary>
        /// Get the popup stack (ordered by priority)
        /// </summary>
        /// <returns>List of popup infos ordered by priority</returns>
        List<PopupInfo> GetPopupStack();
        
        /// <summary>
        /// Bring popup to front (highest priority)
        /// </summary>
        /// <param name="popupId">ID of popup to bring to front</param>
        void BringPopupToFront(string popupId);
        
        /// <summary>
        /// Set popup priority
        /// </summary>
        /// <param name="popupId">ID of popup</param>
        /// <param name="priority">New priority</param>
        void SetPopupPriority(string popupId, int priority);
        
        /// <summary>
        /// Initialize the popup manager
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Clean up all popups
        /// </summary>
        void Cleanup();
    }
}
