using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PracticalSystems.ThemeSystem.Components;
using PracticalSystems.ThemeSystem.Core;
using PracticalSystems.ThemeSystem.Themes;

namespace PracticalSystems.ThemeSystem.Managers
{
    /// <summary>
    /// Manages character themes and character theme components
    /// </summary>
    public class CharacterThemeManager : BaseThemeManager
    {
        [Header("Character Theme Settings")]
        [SerializeField] private CharacterTheme[] availableThemes;
        [SerializeField] private CharacterTheme defaultTheme;
        [SerializeField] private bool autoApplyDefaultTheme = true;
        [SerializeField] private bool enableCharacterEffects = true;
        [SerializeField] private bool enableCharacterUI = true;
        
        [Header("Character Theme Components")]
        [SerializeField] private List<CharacterThemeComponent> characterThemeComponents = new List<CharacterThemeComponent>();
        
        public override string Category => "Character";
        
        public CharacterTheme[] AvailableThemes => availableThemes;
        public CharacterTheme DefaultTheme => defaultTheme;
        public bool EnableCharacterEffects => enableCharacterEffects;
        public bool EnableCharacterUI => enableCharacterUI;
        
        protected override void Initialize()
        {
            base.Initialize();
            
            // Auto-find character theme components if not manually assigned
            if (characterThemeComponents.Count == 0)
            {
                AutoFindCharacterThemeComponents();
            }
            
            // Apply default theme if specified
            if (autoApplyDefaultTheme && defaultTheme != null)
            {
                ApplyTheme(defaultTheme);
            }
        }
        
        /// <summary>
        /// Automatically finds all character theme components in the scene
        /// </summary>
        public void AutoFindCharacterThemeComponents()
        {
            characterThemeComponents.Clear();
            var foundComponents = FindObjectsOfType<CharacterThemeComponent>();
            characterThemeComponents.AddRange(foundComponents);
            
            if (debugMode)
                Debug.Log($"[Character Theme Manager] Found {characterThemeComponents.Count} character theme components");
        }
        
        public override void RegisterComponent(IThemeComponent component)
        {
            base.RegisterComponent(component);
            
            if (component is CharacterThemeComponent charComponent && !characterThemeComponents.Contains(charComponent))
            {
                characterThemeComponents.Add(charComponent);
                
                if (debugMode)
                    Debug.Log($"[Character Theme Manager] Registered character theme component: {charComponent.name}");
            }
        }
        
        public override void UnregisterComponent(IThemeComponent component)
        {
            base.UnregisterComponent(component);
            
            if (component is CharacterThemeComponent charComponent)
            {
                characterThemeComponents.Remove(charComponent);
                
                if (debugMode)
                    Debug.Log($"[Character Theme Manager] Unregistered character theme component: {charComponent.name}");
            }
        }
        
        /// <summary>
        /// Applies a character theme by name
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
            
            Debug.LogWarning($"[Character Theme Manager] Theme '{themeName}' not found");
            return false;
        }
        
        /// <summary>
        /// Applies a character theme by ID
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
            
            Debug.LogWarning($"[Character Theme Manager] Theme with ID '{themeId}' not found");
            return false;
        }
        
        /// <summary>
        /// Gets a theme by name
        /// </summary>
        /// <param name="themeName">Name of the theme</param>
        /// <returns>The theme if found, null otherwise</returns>
        public CharacterTheme GetThemeByName(string themeName)
        {
            return availableThemes?.FirstOrDefault(t => t.ThemeName == themeName);
        }
        
        /// <summary>
        /// Gets a theme by ID
        /// </summary>
        /// <param name="themeId">ID of the theme</param>
        /// <returns>The theme if found, null otherwise</returns>
        public CharacterTheme GetThemeById(string themeId)
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
        public void AddTheme(CharacterTheme theme)
        {
            if (theme == null) return;
            
            if (availableThemes == null)
            {
                availableThemes = new CharacterTheme[] { theme };
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
                Debug.Log($"[Character Theme Manager] Added theme: {theme.ThemeName}");
        }
        
        /// <summary>
        /// Removes a theme from the available themes
        /// </summary>
        /// <param name="theme">The theme to remove</param>
        public void RemoveTheme(CharacterTheme theme)
        {
            if (theme == null || availableThemes == null) return;
            
            var themesList = availableThemes.ToList();
            if (themesList.Contains(theme))
            {
                themesList.Remove(theme);
                availableThemes = themesList.ToArray();
                
                if (debugMode)
                    Debug.Log($"[Character Theme Manager] Removed theme: {theme.ThemeName}");
            }
        }
        
        /// <summary>
        /// Refreshes all character theme components with the current theme
        /// </summary>
        public void RefreshAllCharacterComponents()
        {
            foreach (var component in characterThemeComponents)
            {
                if (component != null)
                {
                    component.RefreshTheme();
                }
            }
            
            if (debugMode)
                Debug.Log($"[Character Theme Manager] Refreshed {characterThemeComponents.Count} character theme components");
        }
        
        /// <summary>
        /// Updates character stats for all components
        /// </summary>
        public void UpdateAllCharacterStats()
        {
            foreach (var component in characterThemeComponents)
            {
                if (component != null)
                {
                    component.UpdateCharacterStats();
                }
            }
            
            if (debugMode)
                Debug.Log($"[Character Theme Manager] Updated stats for {characterThemeComponents.Count} character components");
        }
        
        /// <summary>
        /// Plays a character sound effect across all components
        /// </summary>
        /// <param name="soundIndex">Index of the character sound to play</param>
        public void PlayCharacterSound(int soundIndex)
        {
            foreach (var component in characterThemeComponents)
            {
                if (component != null)
                {
                    component.PlayCharacterSound(soundIndex);
                }
            }
        }
        
        /// <summary>
        /// Gets statistics about character theme components
        /// </summary>
        /// <returns>Character component statistics</returns>
        public CharacterComponentStats GetCharacterComponentStats()
        {
            return new CharacterComponentStats
            {
                totalCharacterComponents = characterThemeComponents.Count,
                activeCharacterComponents = characterThemeComponents.Count(c => c != null),
                nullCharacterComponents = characterThemeComponents.Count(c => c == null),
                totalAvailableThemes = availableThemes?.Length ?? 0,
                characterEffectsEnabled = enableCharacterEffects,
                characterUIEnabled = enableCharacterUI
            };
        }
        
        /// <summary>
        /// Validates all character theme components
        /// </summary>
        /// <returns>True if all components are valid</returns>
        public bool ValidateCharacterComponents()
        {
            bool allValid = true;
            
            foreach (var component in characterThemeComponents)
            {
                if (component == null)
                {
                    Debug.LogWarning($"[Character Theme Manager] Found null character theme component");
                    allValid = false;
                }
                else
                {
                    // Validate component has required elements
                    var renderers = component.GetComponentsInChildren<Renderer>();
                    var animators = component.GetComponentsInChildren<Animator>();
                    
                    if (renderers.Length == 0 && animators.Length == 0)
                    {
                        Debug.LogWarning($"[Character Theme Manager] Character theme component '{component.name}' has no renderers or animators");
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
                    Debug.LogWarning($"[Character Theme Manager] Removed null themes from available themes");
                }
            }
            
            // Remove null components
            characterThemeComponents.RemoveAll(c => c == null);
        }
        
        [ContextMenu("Auto-Find Character Theme Components")]
        private void AutoFindCharacterThemeComponentsMenu()
        {
            AutoFindCharacterThemeComponents();
        }
        
        [ContextMenu("Validate Character Components")]
        private void ValidateCharacterComponentsMenu()
        {
            bool isValid = ValidateCharacterComponents();
            Debug.Log($"[Character Theme Manager] Character components validation: {(isValid ? "PASSED" : "FAILED")}");
        }
        
        [ContextMenu("Refresh All Character Components")]
        private void RefreshAllCharacterComponentsMenu()
        {
            RefreshAllCharacterComponents();
        }
        
        [ContextMenu("Update All Character Stats")]
        private void UpdateAllCharacterStatsMenu()
        {
            UpdateAllCharacterStats();
        }
    }
    
    /// <summary>
    /// Statistics about character theme components
    /// </summary>
    [System.Serializable]
    public class CharacterComponentStats
    {
        public int totalCharacterComponents;
        public int activeCharacterComponents;
        public int nullCharacterComponents;
        public int totalAvailableThemes;
        public bool characterEffectsEnabled;
        public bool characterUIEnabled;
    }
}
