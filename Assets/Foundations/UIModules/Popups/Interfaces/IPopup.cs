using System;
using UnityEngine;

namespace Foundations.UIModules.Popups.Interfaces
{
    /// <summary>
    /// Base interface for all popups
    /// </summary>
    public interface IPopup
    {
        public bool ForceDestroy { get; }
        
        /// <summary>
        /// Popup type identifier
        /// </summary>
        public Type PopupType { get; }
        
        /// <summary>
        /// Whether this popup is currently active
        /// </summary>
        public bool IsActive { get; }
        
        /// <summary>
        /// Priority for popup stacking (higher number = higher priority)
        /// </summary>
        public int Priority { get; }
        
        /// <summary>
        /// Whether this popup can be closed by clicking outside
        /// </summary>
        public bool CanCloseOnOutsideClick { get; }
        
        public Transform Transform { get; }
        
        /// <summary>
        /// Shows the popup
        /// </summary>
        public void Show();
        
        /// <summary>
        /// Hides the popup
        /// </summary>
        public void Hide();
        
        /// <summary>
        /// Destroys the popup
        /// </summary>
        public void Destroy();
        
        /// <summary>
        /// Events
        /// </summary>
        public event Action<IPopup> OnShown;
        public event Action<IPopup> OnHidden;
        public event Action<IPopup> OnDestroyed;
    }
}
