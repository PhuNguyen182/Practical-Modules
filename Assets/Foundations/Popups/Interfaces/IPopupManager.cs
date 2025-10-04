using System;
using System.Collections.Generic;

namespace Foundations.Popups.Interfaces
{
    /// <summary>
    /// Interface for managing popups in the application
    /// Handles creation, destruction, and lifecycle of all popups
    /// </summary>
    public interface IPopupManager
    {
        /// <summary>
        /// Shows a popup with the given type
        /// </summary>
        /// <typeparam name="T">Popup type</typeparam>
        /// <returns>Popup instance</returns>
        T ShowPopup<T>() where T : class, IPopup;
        
        /// <summary>
        /// Shows a popup with data
        /// </summary>
        /// <typeparam name="T">Popup type</typeparam>
        /// <typeparam name="TData">Data type</typeparam>
        /// <param name="data">Data to pass to popup</param>
        /// <returns>Popup instance</returns>
        T ShowPopup<T, TData>(TData data) where T : class, IPopup<TData>;
        
        /// <summary>
        /// Hides the specified popup
        /// </summary>
        /// <param name="popup">Popup to hide</param>
        void HidePopup(IPopup popup);
        
        /// <summary>
        /// Hides popup by type
        /// </summary>
        /// <typeparam name="T">Popup type</typeparam>
        void HidePopup<T>() where T : class, IPopup;
        
        /// <summary>
        /// Hides all active popups
        /// </summary>
        void HideAllPopups();
        
        /// <summary>
        /// Checks if a popup of given type is currently active
        /// </summary>
        /// <typeparam name="T">Popup type</typeparam>
        /// <returns>True if popup is active</returns>
        bool IsPopupActive<T>() where T : class, IPopup;
        
        /// <summary>
        /// Gets all currently active popups
        /// </summary>
        /// <returns>List of active popups</returns>
        IReadOnlyList<IPopup> GetActivePopups();
        
        /// <summary>
        /// Events
        /// </summary>
        event Action<IPopup> OnPopupShown;
        event Action<IPopup> OnPopupHidden;
        event Action<IPopup> OnPopupDestroyed;
    }
}
