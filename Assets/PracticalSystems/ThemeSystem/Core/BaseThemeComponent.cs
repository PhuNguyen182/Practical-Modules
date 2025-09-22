using UnityEngine;
using System;

namespace PracticalSystems.ThemeSystem.Core
{
    /// <summary>
    /// Base class for components that can be themed
    /// </summary>
    public abstract class BaseThemeComponent : MonoBehaviour, IThemeComponent
    {
        [Header("Theme Settings")]
        [SerializeField] protected bool autoRegister = true;
        [SerializeField] protected bool applyThemeOnStart = true;
        [SerializeField] protected string[] supportedThemeTypes = new string[0];
        
        protected ITheme currentTheme;
        protected bool isRegistered = false;
        
        protected virtual void Start()
        {
            if (applyThemeOnStart && autoRegister)
            {
                RegisterWithThemeManager();
            }
        }
        
        protected virtual void OnDestroy()
        {
            if (isRegistered)
            {
                UnregisterFromThemeManager();
            }
        }
        
        /// <summary>
        /// Registers this component with the appropriate theme manager
        /// </summary>
        protected virtual void RegisterWithThemeManager()
        {
            var themeController = FindObjectOfType<ThemeController>();
            if (themeController != null)
            {
                themeController.RegisterComponent(this);
                isRegistered = true;
            }
            else
            {
                Debug.LogWarning($"ThemeController not found for {gameObject.name}. Cannot register theme component.");
            }
        }
        
        /// <summary>
        /// Unregisters this component from theme managers
        /// </summary>
        protected virtual void UnregisterFromThemeManager()
        {
            var themeController = FindObjectOfType<ThemeController>();
            if (themeController != null)
            {
                themeController.UnregisterComponent(this);
                isRegistered = false;
            }
        }
        
        public virtual void ApplyTheme(ITheme theme)
        {
            if (theme == null || !SupportsThemeType(theme.GetType()))
                return;
                
            currentTheme = theme;
            OnThemeApplied(theme);
        }
        
        public virtual ITheme GetCurrentTheme()
        {
            return currentTheme;
        }
        
        public virtual bool SupportsThemeType(Type themeType)
        {
            if (supportedThemeTypes == null || supportedThemeTypes.Length == 0)
                return true; // Support all types if none specified
                
            string typeName = themeType.Name;
            foreach (var supportedType in supportedThemeTypes)
            {
                if (string.Equals(supportedType, typeName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Called when a theme is applied to this component
        /// Override this method to implement theme-specific logic
        /// </summary>
        /// <param name="theme">The theme being applied</param>
        protected abstract void OnThemeApplied(ITheme theme);
        
        /// <summary>
        /// Gets a theme property with type conversion
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>The property value or default</returns>
        protected virtual T GetThemeProperty<T>(string propertyName, T defaultValue = default(T))
        {
            if (currentTheme == null)
                return defaultValue;
                
            return currentTheme.GetProperty(propertyName, defaultValue);
        }
        
        /// <summary>
        /// Refreshes the current theme application
        /// </summary>
        [ContextMenu("Refresh Theme")]
        public virtual void RefreshTheme()
        {
            if (currentTheme != null)
            {
                ApplyTheme(currentTheme);
            }
        }
    }
}
