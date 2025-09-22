using UnityEngine;
using System.Collections.Generic;

namespace PracticalSystems.ThemeSystem.Core
{
    /// <summary>
    /// Base interface for all theme types
    /// </summary>
    public interface ITheme
    {
        /// <summary>
        /// Unique identifier for this theme
        /// </summary>
        string ThemeId { get; }
        
        /// <summary>
        /// Human-readable name for this theme
        /// </summary>
        string ThemeName { get; }
        
        /// <summary>
        /// Description of this theme
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Theme category (e.g., "UI", "Environment", "Audio")
        /// </summary>
        string Category { get; }
        
        /// <summary>
        /// Whether this theme is currently active
        /// </summary>
        bool IsActive { get; }
        
        /// <summary>
        /// Priority for theme application (higher values override lower ones)
        /// </summary>
        int Priority { get; }
        
        /// <summary>
        /// Applies this theme to the specified component
        /// </summary>
        /// <param name="component">The component to apply the theme to</param>
        /// <returns>True if successfully applied, false otherwise</returns>
        bool ApplyTo(IThemeComponent component);
        
        /// <summary>
        /// Gets all properties defined in this theme
        /// </summary>
        /// <returns>Dictionary of property names to values</returns>
        Dictionary<string, object> GetProperties();
        
        /// <summary>
        /// Gets a specific property value
        /// </summary>
        /// <typeparam name="T">The type of the property value</typeparam>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="defaultValue">Default value if property not found</param>
        /// <returns>The property value or default</returns>
        T GetProperty<T>(string propertyName, T defaultValue = default(T));
        
        /// <summary>
        /// Sets a property value
        /// </summary>
        /// <typeparam name="T">The type of the property value</typeparam>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="value">The value to set</param>
        void SetProperty<T>(string propertyName, T value);
    }
}
