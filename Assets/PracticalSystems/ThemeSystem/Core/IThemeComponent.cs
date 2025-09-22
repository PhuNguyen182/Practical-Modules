using UnityEngine;

namespace PracticalSystems.ThemeSystem.Core
{
    /// <summary>
    /// Interface for components that can be themed
    /// </summary>
    public interface IThemeComponent
    {
        /// <summary>
        /// Applies the specified theme to this component
        /// </summary>
        /// <param name="theme">The theme to apply</param>
        void ApplyTheme(ITheme theme);
        
        /// <summary>
        /// Gets the current theme applied to this component
        /// </summary>
        /// <returns>The currently applied theme</returns>
        ITheme GetCurrentTheme();
        
        /// <summary>
        /// Whether this component supports the specified theme type
        /// </summary>
        /// <param name="themeType">The theme type to check</param>
        /// <returns>True if supported, false otherwise</returns>
        bool SupportsThemeType(System.Type themeType);
    }
}
