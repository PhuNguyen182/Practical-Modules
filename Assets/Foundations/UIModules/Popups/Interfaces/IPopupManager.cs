using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Foundations.UIModules.Popups.Interfaces
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
        /// <param name="popupName">Name of popup</param>
        /// <returns>Popup instance</returns>
        public UniTask<T> ShowPopup<T>(string popupName) where T : class, IPopup;

        /// <summary>
        /// Shows a popup with data
        /// </summary>
        /// <typeparam name="T">Popup type</typeparam>
        /// <typeparam name="TData">Data type</typeparam>
        /// <param name="popupName">Name of popup</param>
        /// <param name="data">Data to pass to popup</param>
        /// <returns>Popup instance</returns>
        public UniTask<T> ShowPopup<T, TData>(string popupName, TData data) where T : class, IPopup<TData>;
        
        /// <summary>
        /// Hides the specified popup
        /// </summary>
        /// <param name="popup">Popup to hide</param>
        public void HidePopup(IPopup popup);
        
        /// <summary>
        /// Hides popup by type
        /// </summary>
        /// <typeparam name="T">Popup type</typeparam>
        public void HidePopup<T>() where T : class, IPopup;
        
        /// <summary>
        /// Hides all active popups
        /// </summary>
        public void HideAllPopups();
        
        /// <summary>
        /// Checks if a popup of given type is currently active
        /// </summary>
        /// <typeparam name="T">Popup type</typeparam>
        /// <returns>True if popup is active</returns>
        public bool IsPopupActive<T>() where T : class, IPopup;
        
        /// <summary>
        /// Gets all currently active popups
        /// </summary>
        /// <returns>List of active popups</returns>
        public IReadOnlyList<IPopup> GetActivePopups();
        
        /// <summary>
        /// Events
        /// </summary>
        public event Action<IPopup> OnPopupShown;
        public event Action<IPopup> OnPopupHidden;
        public event Action<IPopup> OnPopupDestroyed;
    }
}
