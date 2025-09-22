using System.Collections.Generic;
using System;

namespace PracticalSystems.ThemeSystem.Core
{
    /// <summary>
    /// Interface for theme managers that handle specific theme categories
    /// </summary>
    public interface IThemeManager
    {
        /// <summary>
        /// The category this manager handles
        /// </summary>
        string Category { get; }
        
        /// <summary>
        /// Whether this manager is currently active
        /// </summary>
        bool IsActive { get; }
        
        /// <summary>
        /// Currently applied theme
        /// </summary>
        ITheme CurrentTheme { get; }
        
        /// <summary>
        /// Event fired when a theme is applied
        /// </summary>
        event Action<ITheme> OnThemeApplied;
        
        /// <summary>
        /// Event fired when a theme is about to be applied
        /// </summary>
        event Action<ITheme> OnThemeApplying;
        
        /// <summary>
        /// Applies a theme to all registered components
        /// </summary>
        /// <param name="theme">The theme to apply</param>
        /// <returns>True if successfully applied</returns>
        bool ApplyTheme(ITheme theme);
        
        /// <summary>
        /// Registers a component to receive theme updates
        /// </summary>
        /// <param name="component">The component to register</param>
        void RegisterComponent(IThemeComponent component);
        
        /// <summary>
        /// Unregisters a component from theme updates
        /// </summary>
        /// <param name="component">The component to unregister</param>
        void UnregisterComponent(IThemeComponent component);
        
        /// <summary>
        /// Gets all registered components
        /// </summary>
        /// <returns>List of registered components</returns>
        List<IThemeComponent> GetRegisteredComponents();
        
        /// <summary>
        /// Refreshes the current theme on all registered components
        /// </summary>
        void RefreshCurrentTheme();
    }
}
