using System;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.MVP
{
    /// <summary>
    /// Interface for creating and managing popups with data binding
    /// </summary>
    public interface IPopupCreator
    {
        /// <summary>
        /// Event fired when a popup is created
        /// </summary>
        event Action<IPresenter> OnPopupCreated;
        
        /// <summary>
        /// Create a popup with the specified type and data
        /// </summary>
        /// <param name="popupType">Type of popup to create</param>
        /// <param name="data">Data to bind to the popup</param>
        /// <param name="parent">Parent transform for the popup</param>
        /// <returns>The created popup presenter</returns>
        IPresenter? CreatePopup(string popupType, object? data = null, Transform? parent = null);
        
        /// <summary>
        /// Create a popup with typed data binding
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="popupType">Type of popup to create</param>
        /// <param name="data">Typed data to bind</param>
        /// <param name="parent">Parent transform for the popup</param>
        /// <returns>The created popup presenter</returns>
        IPresenter? CreatePopup<T>(string popupType, T data, Transform? parent = null);
        
        /// <summary>
        /// Register a popup type with its factory method
        /// </summary>
        /// <param name="popupType">Type identifier</param>
        /// <param name="factory">Factory method for creating the popup</param>
        void RegisterPopupType(string popupType, Func<object?, Transform?, IPresenter> factory);
        
        /// <summary>
        /// Register a typed popup factory
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="popupType">Type identifier</param>
        /// <param name="factory">Typed factory method</param>
        void RegisterPopupType<T>(string popupType, Func<T, Transform?, IPresenter> factory);
        
        /// <summary>
        /// Unregister a popup type
        /// </summary>
        /// <param name="popupType">Type identifier to unregister</param>
        void UnregisterPopupType(string popupType);
        
        /// <summary>
        /// Check if a popup type is registered
        /// </summary>
        /// <param name="popupType">Type identifier to check</param>
        /// <returns>True if registered</returns>
        bool IsPopupTypeRegistered(string popupType);
        
        /// <summary>
        /// Get all registered popup types
        /// </summary>
        /// <returns>List of registered popup type names</returns>
        List<string> GetRegisteredPopupTypes();
        
        /// <summary>
        /// Initialize the popup creator
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Clean up all registered popup types
        /// </summary>
        void Cleanup();
    }
}
