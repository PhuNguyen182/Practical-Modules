using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PracticalSystems.ThemeSystem.Components;
using PracticalSystems.ThemeSystem.Core;
using PracticalSystems.ThemeSystem.Themes;

namespace PracticalSystems.ThemeSystem.Managers
{
    /// <summary>
    /// Manages UI themes and UI theme components
    /// </summary>
    public class UIThemeManager : BaseThemeManager
    {
        [Header("UI Theme Settings")]
        [SerializeField] private UITheme[] availableThemes;
        [SerializeField] private UITheme defaultTheme;
        [SerializeField] private bool autoApplyDefaultTheme = true;
        [SerializeField] private bool enableThemeTransitions = true;
        
        [Header("UI Theme Components")]
        [SerializeField] private List<UIThemeComponent> uiThemeComponents = new List<UIThemeComponent>();
        
        public override string Category => "UI";
        
        public UITheme[] AvailableThemes => availableThemes;
        public UITheme DefaultTheme => defaultTheme;
        public bool EnableThemeTransitions => enableThemeTransitions;
        
        protected override void Initialize()
        {
            base.Initialize();
            
            // Auto-find UI theme components if not manually assigned
            if (uiThemeComponents.Count == 0)
            {
                AutoFindUIThemeComponents();
            }
            
            // Apply default theme if specified
            if (autoApplyDefaultTheme && defaultTheme != null)
            {
                ApplyTheme(defaultTheme);
            }
        }
        
        /// <summary>
        /// Automatically finds all UI theme components in the scene
        /// </summary>
        public void AutoFindUIThemeComponents()
        {
            uiThemeComponents.Clear();
            var foundComponents = FindObjectsOfType<UIThemeComponent>();
            uiThemeComponents.AddRange(foundComponents);
            
            if (debugMode)
                Debug.Log($"[UI Theme Manager] Found {uiThemeComponents.Count} UI theme components");
        }
        
        public override void RegisterComponent(IThemeComponent component)
        {
            base.RegisterComponent(component);
            
            if (component is UIThemeComponent uiComponent && !uiThemeComponents.Contains(uiComponent))
            {
                uiThemeComponents.Add(uiComponent);
                
                if (debugMode)
                    Debug.Log($"[UI Theme Manager] Registered UI theme component: {uiComponent.name}");
            }
        }
        
        public override void UnregisterComponent(IThemeComponent component)
        {
            base.UnregisterComponent(component);
            
            if (component is UIThemeComponent uiComponent)
            {
                uiThemeComponents.Remove(uiComponent);
                
                if (debugMode)
                    Debug.Log($"[UI Theme Manager] Unregistered UI theme component: {uiComponent.name}");
            }
        }
        
        /// <summary>
        /// Applies a UI theme by name
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
            
            Debug.LogWarning($"[UI Theme Manager] Theme '{themeName}' not found");
            return false;
        }
        
        /// <summary>
        /// Applies a UI theme by ID
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
            
            Debug.LogWarning($"[UI Theme Manager] Theme with ID '{themeId}' not found");
            return false;
        }
        
        /// <summary>
        /// Gets a theme by name
        /// </summary>
        /// <param name="themeName">Name of the theme</param>
        /// <returns>The theme if found, null otherwise</returns>
        public UITheme GetThemeByName(string themeName)
        {
            return availableThemes?.FirstOrDefault(t => t.ThemeName == themeName);
        }
        
        /// <summary>
        /// Gets a theme by ID
        /// </summary>
        /// <param name="themeId">ID of the theme</param>
        /// <returns>The theme if found, null otherwise</returns>
        public UITheme GetThemeById(string themeId)
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
        public void AddTheme(UITheme theme)
        {
            if (theme == null) return;
            
            if (availableThemes == null)
            {
                availableThemes = new UITheme[] { theme };
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
                Debug.Log($"[UI Theme Manager] Added theme: {theme.ThemeName}");
        }
        
        /// <summary>
        /// Removes a theme from the available themes
        /// </summary>
        /// <param name="theme">The theme to remove</param>
        public void RemoveTheme(UITheme theme)
        {
            if (theme == null || availableThemes == null) return;
            
            var themesList = availableThemes.ToList();
            if (themesList.Contains(theme))
            {
                themesList.Remove(theme);
                availableThemes = themesList.ToArray();
                
                if (debugMode)
                    Debug.Log($"[UI Theme Manager] Removed theme: {theme.ThemeName}");
            }
        }
        
        /// <summary>
        /// Refreshes all UI theme components with the current theme
        /// </summary>
        public void RefreshAllUIComponents()
        {
            foreach (var component in uiThemeComponents)
            {
                if (component != null)
                {
                    component.RefreshTheme();
                }
            }
            
            if (debugMode)
                Debug.Log($"[UI Theme Manager] Refreshed {uiThemeComponents.Count} UI theme components");
        }
        
        /// <summary>
        /// Gets statistics about UI theme components
        /// </summary>
        /// <returns>UI component statistics</returns>
        public UIComponentStats GetUIComponentStats()
        {
            return new UIComponentStats
            {
                totalUIComponents = uiThemeComponents.Count,
                activeUIComponents = uiThemeComponents.Count(c => c != null),
                nullUIComponents = uiThemeComponents.Count(c => c == null),
                totalAvailableThemes = availableThemes?.Length ?? 0
            };
        }
        
        /// <summary>
        /// Validates all UI theme components
        /// </summary>
        /// <returns>True if all components are valid</returns>
        public bool ValidateUIComponents()
        {
            bool allValid = true;
            
            foreach (var component in uiThemeComponents)
            {
                if (component == null)
                {
                    Debug.LogWarning($"[UI Theme Manager] Found null UI theme component");
                    allValid = false;
                }
                else
                {
                    // Validate component has required elements
                    if (component.GetComponentsInChildren<UnityEngine.UI.Image>().Length == 0 &&
                        component.GetComponentsInChildren<TMPro.TMP_Text>().Length == 0)
                    {
                        Debug.LogWarning($"[UI Theme Manager] UI theme component '{component.name}' has no UI elements");
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
                    Debug.LogWarning($"[UI Theme Manager] Removed null themes from available themes");
                }
            }
            
            // Remove null components
            uiThemeComponents.RemoveAll(c => c == null);
        }
        
        [ContextMenu("Auto-Find UI Theme Components")]
        private void AutoFindUIThemeComponentsMenu()
        {
            AutoFindUIThemeComponents();
        }
        
        [ContextMenu("Validate UI Components")]
        private void ValidateUIComponentsMenu()
        {
            bool isValid = ValidateUIComponents();
            Debug.Log($"[UI Theme Manager] UI components validation: {(isValid ? "PASSED" : "FAILED")}");
        }
        
        [ContextMenu("Refresh All UI Components")]
        private void RefreshAllUIComponentsMenu()
        {
            RefreshAllUIComponents();
        }
    }
    
    /// <summary>
    /// Statistics about UI theme components
    /// </summary>
    [System.Serializable]
    public class UIComponentStats
    {
        public int totalUIComponents;
        public int activeUIComponents;
        public int nullUIComponents;
        public int totalAvailableThemes;
    }
}
