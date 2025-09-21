using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameVisualUpdateByTimeSystem.Core.Interfaces
{
    /// <summary>
    /// Main system interface for managing time-based visual updates
    /// </summary>
    public interface ITimeBasedVisualSystem
    {
        /// <summary>
        /// Current time provider
        /// </summary>
        ITimeProvider TimeProvider { get; }
        
        /// <summary>
        /// Whether the system is currently running
        /// </summary>
        bool IsRunning { get; }
        
        /// <summary>
        /// Event fired when a visual component is registered
        /// </summary>
        event Action<ITimeBasedVisual> OnVisualRegistered;
        
        /// <summary>
        /// Event fired when a visual component is unregistered
        /// </summary>
        event Action<ITimeBasedVisual> OnVisualUnregistered;
        
        /// <summary>
        /// Register a visual component for time-based updates
        /// </summary>
        /// <param name="visual">Visual component to register</param>
        void RegisterVisual(ITimeBasedVisual visual);
        
        /// <summary>
        /// Unregister a visual component
        /// </summary>
        /// <param name="visual">Visual component to unregister</param>
        void UnregisterVisual(ITimeBasedVisual visual);
        
        /// <summary>
        /// Get a registered visual component by ID
        /// </summary>
        /// <param name="visualId">ID of the visual component</param>
        /// <returns>Visual component or null if not found</returns>
        ITimeBasedVisual GetVisual(string visualId);
        
        /// <summary>
        /// Get all registered visual components
        /// </summary>
        /// <returns>List of all registered visual components</returns>
        IReadOnlyList<ITimeBasedVisual> GetAllVisuals();
        
        /// <summary>
        /// Get visual components by type
        /// </summary>
        /// <typeparam name="T">Type of visual component</typeparam>
        /// <returns>List of visual components of specified type</returns>
        IReadOnlyList<T> GetVisualsByType<T>() where T : class, ITimeBasedVisual;
        
        /// <summary>
        /// Initialize the system
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Shutdown the system and cleanup resources
        /// </summary>
        void Shutdown();
        
        /// <summary>
        /// Update the system (called by player loop)
        /// </summary>
        /// <param name="deltaTime">Time since last update</param>
        void UpdateSystem(float deltaTime);
        
        /// <summary>
        /// Set time provider
        /// </summary>
        /// <param name="timeProvider">Time provider to use</param>
        void SetTimeProvider(ITimeProvider timeProvider);
        
        /// <summary>
        /// Enable/disable automatic updates
        /// </summary>
        /// <param name="enabled">Whether to enable automatic updates</param>
        void SetAutoUpdate(bool enabled);
    }
}
