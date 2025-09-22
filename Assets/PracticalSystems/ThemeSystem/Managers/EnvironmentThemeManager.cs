using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PracticalSystems.ThemeSystem.Components;
using PracticalSystems.ThemeSystem.Core;
using PracticalSystems.ThemeSystem.Themes;

namespace PracticalSystems.ThemeSystem.Managers
{
    /// <summary>
    /// Manages environment themes and environment theme components
    /// </summary>
    public class EnvironmentThemeManager : BaseThemeManager
    {
        [Header("Environment Theme Settings")]
        [SerializeField] private EnvironmentTheme[] availableThemes;
        [SerializeField] private EnvironmentTheme defaultTheme;
        [SerializeField] private bool autoApplyDefaultTheme = true;
        [SerializeField] private bool enableTimeProgression = true;
        [SerializeField] private bool enableWeatherEffects = true;
        
        [Header("Environment Theme Components")]
        [SerializeField] private List<EnvironmentThemeComponent> environmentThemeComponents = new List<EnvironmentThemeComponent>();
        
        public override string Category => "Environment";
        
        public EnvironmentTheme[] AvailableThemes => availableThemes;
        public EnvironmentTheme DefaultTheme => defaultTheme;
        public bool EnableTimeProgression => enableTimeProgression;
        public bool EnableWeatherEffects => enableWeatherEffects;
        
        protected override void Initialize()
        {
            base.Initialize();
            
            // Auto-find environment theme components if not manually assigned
            if (environmentThemeComponents.Count == 0)
            {
                AutoFindEnvironmentThemeComponents();
            }
            
            // Apply default theme if specified
            if (autoApplyDefaultTheme && defaultTheme != null)
            {
                ApplyTheme(defaultTheme);
            }
        }
        
        /// <summary>
        /// Automatically finds all environment theme components in the scene
        /// </summary>
        public void AutoFindEnvironmentThemeComponents()
        {
            environmentThemeComponents.Clear();
            var foundComponents = FindObjectsOfType<EnvironmentThemeComponent>();
            environmentThemeComponents.AddRange(foundComponents);
            
            if (debugMode)
                Debug.Log($"[Environment Theme Manager] Found {environmentThemeComponents.Count} environment theme components");
        }
        
        public override void RegisterComponent(IThemeComponent component)
        {
            base.RegisterComponent(component);
            
            if (component is EnvironmentThemeComponent envComponent && !environmentThemeComponents.Contains(envComponent))
            {
                environmentThemeComponents.Add(envComponent);
                
                if (debugMode)
                    Debug.Log($"[Environment Theme Manager] Registered environment theme component: {envComponent.name}");
            }
        }
        
        public override void UnregisterComponent(IThemeComponent component)
        {
            base.UnregisterComponent(component);
            
            if (component is EnvironmentThemeComponent envComponent)
            {
                environmentThemeComponents.Remove(envComponent);
                
                if (debugMode)
                    Debug.Log($"[Environment Theme Manager] Unregistered environment theme component: {envComponent.name}");
            }
        }
        
        /// <summary>
        /// Applies an environment theme by name
        /// </summary>
        /// <param name="themeName">Name of the theme to apply</param>
        /// <returns>True if theme was found and applied</returns>
        public bool ApplyThemeByName(string themeName)
        {
            var theme = availableThemes?.FirstOrDefault(t => t.ThemeName == themeName);
            if (theme != null)
            {
                return ApplyTheme(theme);
            }
            
            Debug.LogWarning($"[Environment Theme Manager] Theme '{themeName}' not found");
            return false;
        }
        
        /// <summary>
        /// Applies an environment theme by ID
        /// </summary>
        /// <param name="themeId">ID of the theme to apply</param>
        /// <returns>True if theme was found and applied</returns>
        public bool ApplyThemeById(string themeId)
        {
            var theme = availableThemes?.FirstOrDefault(t => t.ThemeId == themeId);
            if (theme != null)
            {
                return ApplyTheme(theme);
            }
            
            Debug.LogWarning($"[Environment Theme Manager] Theme with ID '{themeId}' not found");
            return false;
        }
        
        /// <summary>
        /// Gets a theme by name
        /// </summary>
        /// <param name="themeName">Name of the theme</param>
        /// <returns>The theme if found, null otherwise</returns>
        public EnvironmentTheme GetThemeByName(string themeName)
        {
            return availableThemes?.FirstOrDefault(t => t.ThemeName == themeName);
        }
        
        /// <summary>
        /// Gets a theme by ID
        /// </summary>
        /// <param name="themeId">ID of the theme</param>
        /// <returns>The theme if found, null otherwise</returns>
        public EnvironmentTheme GetThemeById(string themeId)
        {
            return availableThemes?.FirstOrDefault(t => t.ThemeId == themeId);
        }
        
        /// <summary>
        /// Gets all available theme names
        /// </summary>
        /// <returns>Array of theme names</returns>
        public string[] GetAvailableThemeNames()
        {
            return availableThemes?.Select(t => t.ThemeName).ToArray() ?? new string[0];
        }
        
        /// <summary>
        /// Adds a new theme to the available themes
        /// </summary>
        /// <param name="theme">The theme to add</param>
        public void AddTheme(EnvironmentTheme theme)
        {
            if (theme == null) return;
            
            if (availableThemes == null)
            {
                availableThemes = new EnvironmentTheme[] { theme };
            }
            else
            {
                var themesList = availableThemes.ToList();
                if (!themesList.Contains(theme))
                {
                    themesList.Add(theme);
                    availableThemes = themesList.ToArray();
                }
            }
            
            if (debugMode)
                Debug.Log($"[Environment Theme Manager] Added theme: {theme.ThemeName}");
        }
        
        /// <summary>
        /// Removes a theme from the available themes
        /// </summary>
        /// <param name="theme">The theme to remove</param>
        public void RemoveTheme(EnvironmentTheme theme)
        {
            if (theme == null || availableThemes == null) return;
            
            var themesList = availableThemes.ToList();
            if (themesList.Contains(theme))
            {
                themesList.Remove(theme);
                availableThemes = themesList.ToArray();
                
                if (debugMode)
                    Debug.Log($"[Environment Theme Manager] Removed theme: {theme.ThemeName}");
            }
        }
        
        /// <summary>
        /// Refreshes all environment theme components with the current theme
        /// </summary>
        public void RefreshAllEnvironmentComponents()
        {
            foreach (var component in environmentThemeComponents)
            {
                if (component != null)
                {
                    component.RefreshTheme();
                }
            }
            
            if (debugMode)
                Debug.Log($"[Environment Theme Manager] Refreshed {environmentThemeComponents.Count} environment theme components");
        }
        
        /// <summary>
        /// Applies weather effects to all environment components
        /// </summary>
        public void ApplyWeatherEffects()
        {
            foreach (var component in environmentThemeComponents)
            {
                if (component != null)
                {
                    component.ApplyWeatherEffects();
                }
            }
            
            if (debugMode)
                Debug.Log($"[Environment Theme Manager] Applied weather effects to {environmentThemeComponents.Count} components");
        }
        
        /// <summary>
        /// Updates time of day for all environment components
        /// </summary>
        public void UpdateTimeOfDay()
        {
            if (!enableTimeProgression) return;
            
            foreach (var component in environmentThemeComponents)
            {
                if (component != null)
                {
                    // Time progression is handled in the component's Update method
                    // This method can be used to manually trigger updates if needed
                }
            }
        }
        
        /// <summary>
        /// Gets statistics about environment theme components
        /// </summary>
        /// <returns>Environment component statistics</returns>
        public EnvironmentComponentStats GetEnvironmentComponentStats()
        {
            return new EnvironmentComponentStats
            {
                totalEnvironmentComponents = environmentThemeComponents.Count,
                activeEnvironmentComponents = environmentThemeComponents.Count(c => c != null),
                nullEnvironmentComponents = environmentThemeComponents.Count(c => c == null),
                totalAvailableThemes = availableThemes?.Length ?? 0,
                timeProgressionEnabled = enableTimeProgression,
                weatherEffectsEnabled = enableWeatherEffects
            };
        }
        
        /// <summary>
        /// Validates all environment theme components
        /// </summary>
        /// <returns>True if all components are valid</returns>
        public bool ValidateEnvironmentComponents()
        {
            bool allValid = true;
            
            foreach (var component in environmentThemeComponents)
            {
                if (component == null)
                {
                    Debug.LogWarning($"[Environment Theme Manager] Found null environment theme component");
                    allValid = false;
                }
                else
                {
                    // Validate component has required elements
                    var renderers = component.GetComponentsInChildren<Renderer>();
                    var lights = component.GetComponentsInChildren<Light>();
                    
                    if (renderers.Length == 0 && lights.Length == 0)
                    {
                        Debug.LogWarning($"[Environment Theme Manager] Environment theme component '{component.name}' has no renderers or lights");
                        allValid = false;
                    }
                }
            }
            
            return allValid;
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            // Remove null themes
            if (availableThemes != null)
            {
                var validThemes = availableThemes.Where(t => t != null).ToArray();
                if (validThemes.Length != availableThemes.Length)
                {
                    availableThemes = validThemes;
                    Debug.LogWarning($"[Environment Theme Manager] Removed null themes from available themes");
                }
            }
            
            // Remove null components
            environmentThemeComponents.RemoveAll(c => c == null);
        }
        
        [ContextMenu("Auto-Find Environment Theme Components")]
        private void AutoFindEnvironmentThemeComponentsMenu()
        {
            AutoFindEnvironmentThemeComponents();
        }
        
        [ContextMenu("Validate Environment Components")]
        private void ValidateEnvironmentComponentsMenu()
        {
            bool isValid = ValidateEnvironmentComponents();
            Debug.Log($"[Environment Theme Manager] Environment components validation: {(isValid ? "PASSED" : "FAILED")}");
        }
        
        [ContextMenu("Refresh All Environment Components")]
        private void RefreshAllEnvironmentComponentsMenu()
        {
            RefreshAllEnvironmentComponents();
        }
        
        [ContextMenu("Apply Weather Effects")]
        private void ApplyWeatherEffectsMenu()
        {
            ApplyWeatherEffects();
        }
    }
    
    /// <summary>
    /// Statistics about environment theme components
    /// </summary>
    [System.Serializable]
    public class EnvironmentComponentStats
    {
        public int totalEnvironmentComponents;
        public int activeEnvironmentComponents;
        public int nullEnvironmentComponents;
        public int totalAvailableThemes;
        public bool timeProgressionEnabled;
        public bool weatherEffectsEnabled;
    }
}
