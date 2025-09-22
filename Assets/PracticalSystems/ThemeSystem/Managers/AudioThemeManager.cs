using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PracticalSystems.ThemeSystem.Components;
using PracticalSystems.ThemeSystem.Core;
using PracticalSystems.ThemeSystem.Themes;

namespace PracticalSystems.ThemeSystem.Managers
{
    /// <summary>
    /// Manages audio themes and audio theme components
    /// </summary>
    public class AudioThemeManager : BaseThemeManager
    {
        [Header("Audio Theme Settings")]
        [SerializeField] private AudioTheme[] availableThemes;
        [SerializeField] private AudioTheme defaultTheme;
        [SerializeField] private bool autoApplyDefaultTheme = true;
        [SerializeField] private bool enableAudioFading = true;
        [SerializeField] private bool enableSpatialAudio = true;
        
        [Header("Audio Theme Components")]
        [SerializeField] private List<AudioThemeComponent> audioThemeComponents = new List<AudioThemeComponent>();
        
        public override string Category => "Audio";
        
        public AudioTheme[] AvailableThemes => availableThemes;
        public AudioTheme DefaultTheme => defaultTheme;
        public bool EnableAudioFading => enableAudioFading;
        public bool EnableSpatialAudio => enableSpatialAudio;
        
        protected override void Initialize()
        {
            base.Initialize();
            
            // Auto-find audio theme components if not manually assigned
            if (audioThemeComponents.Count == 0)
            {
                AutoFindAudioThemeComponents();
            }
            
            // Apply default theme if specified
            if (autoApplyDefaultTheme && defaultTheme != null)
            {
                ApplyTheme(defaultTheme);
            }
        }
        
        /// <summary>
        /// Automatically finds all audio theme components in the scene
        /// </summary>
        public void AutoFindAudioThemeComponents()
        {
            audioThemeComponents.Clear();
            var foundComponents = FindObjectsOfType<AudioThemeComponent>();
            audioThemeComponents.AddRange(foundComponents);
            
            if (debugMode)
                Debug.Log($"[Audio Theme Manager] Found {audioThemeComponents.Count} audio theme components");
        }
        
        public override void RegisterComponent(IThemeComponent component)
        {
            base.RegisterComponent(component);
            
            if (component is AudioThemeComponent audioComponent && !audioThemeComponents.Contains(audioComponent))
            {
                audioThemeComponents.Add(audioComponent);
                
                if (debugMode)
                    Debug.Log($"[Audio Theme Manager] Registered audio theme component: {audioComponent.name}");
            }
        }
        
        public override void UnregisterComponent(IThemeComponent component)
        {
            base.UnregisterComponent(component);
            
            if (component is AudioThemeComponent audioComponent)
            {
                audioThemeComponents.Remove(audioComponent);
                
                if (debugMode)
                    Debug.Log($"[Audio Theme Manager] Unregistered audio theme component: {audioComponent.name}");
            }
        }
        
        /// <summary>
        /// Applies an audio theme by name
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
            
            Debug.LogWarning($"[Audio Theme Manager] Theme '{themeName}' not found");
            return false;
        }
        
        /// <summary>
        /// Applies an audio theme by ID
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
            
            Debug.LogWarning($"[Audio Theme Manager] Theme with ID '{themeId}' not found");
            return false;
        }
        
        /// <summary>
        /// Gets a theme by name
        /// </summary>
        /// <param name="themeName">Name of the theme</param>
        /// <returns>The theme if found, null otherwise</returns>
        public AudioTheme GetThemeByName(string themeName)
        {
            return availableThemes?.FirstOrDefault(t => t.ThemeName == themeName);
        }
        
        /// <summary>
        /// Gets a theme by ID
        /// </summary>
        /// <param name="themeId">ID of the theme</param>
        /// <returns>The theme if found, null otherwise</returns>
        public AudioTheme GetThemeById(string themeId)
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
        public void AddTheme(AudioTheme theme)
        {
            if (theme == null) return;
            
            if (availableThemes == null)
            {
                availableThemes = new AudioTheme[] { theme };
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
                Debug.Log($"[Audio Theme Manager] Added theme: {theme.ThemeName}");
        }
        
        /// <summary>
        /// Removes a theme from the available themes
        /// </summary>
        /// <param name="theme">The theme to remove</param>
        public void RemoveTheme(AudioTheme theme)
        {
            if (theme == null || availableThemes == null) return;
            
            var themesList = availableThemes.ToList();
            if (themesList.Contains(theme))
            {
                themesList.Remove(theme);
                availableThemes = themesList.ToArray();
                
                if (debugMode)
                    Debug.Log($"[Audio Theme Manager] Removed theme: {theme.ThemeName}");
            }
        }
        
        /// <summary>
        /// Refreshes all audio theme components with the current theme
        /// </summary>
        public void RefreshAllAudioComponents()
        {
            foreach (var component in audioThemeComponents)
            {
                if (component != null)
                {
                    component.RefreshTheme();
                }
            }
            
            if (debugMode)
                Debug.Log($"[Audio Theme Manager] Refreshed {audioThemeComponents.Count} audio theme components");
        }
        
        /// <summary>
        /// Stops all audio across all components
        /// </summary>
        public void StopAllAudio()
        {
            foreach (var component in audioThemeComponents)
            {
                if (component != null)
                {
                    component.StopAllAudio();
                }
            }
            
            if (debugMode)
                Debug.Log($"[Audio Theme Manager] Stopped all audio across {audioThemeComponents.Count} components");
        }
        
        /// <summary>
        /// Pauses all audio across all components
        /// </summary>
        public void PauseAllAudio()
        {
            foreach (var component in audioThemeComponents)
            {
                if (component != null)
                {
                    component.PauseAllAudio();
                }
            }
            
            if (debugMode)
                Debug.Log($"[Audio Theme Manager] Paused all audio across {audioThemeComponents.Count} components");
        }
        
        /// <summary>
        /// Resumes all audio across all components
        /// </summary>
        public void ResumeAllAudio()
        {
            foreach (var component in audioThemeComponents)
            {
                if (component != null)
                {
                    component.ResumeAllAudio();
                }
            }
            
            if (debugMode)
                Debug.Log($"[Audio Theme Manager] Resumed all audio across {audioThemeComponents.Count} components");
        }
        
        /// <summary>
        /// Plays a UI sound effect across all components
        /// </summary>
        /// <param name="soundIndex">Index of the UI sound to play</param>
        public void PlayUISound(int soundIndex)
        {
            foreach (var component in audioThemeComponents)
            {
                if (component != null)
                {
                    component.PlayUISound(soundIndex);
                }
            }
        }
        
        /// <summary>
        /// Gets statistics about audio theme components
        /// </summary>
        /// <returns>Audio component statistics</returns>
        public AudioComponentStats GetAudioComponentStats()
        {
            return new AudioComponentStats
            {
                totalAudioComponents = audioThemeComponents.Count,
                activeAudioComponents = audioThemeComponents.Count(c => c != null),
                nullAudioComponents = audioThemeComponents.Count(c => c == null),
                totalAvailableThemes = availableThemes?.Length ?? 0,
                audioFadingEnabled = enableAudioFading,
                spatialAudioEnabled = enableSpatialAudio
            };
        }
        
        /// <summary>
        /// Validates all audio theme components
        /// </summary>
        /// <returns>True if all components are valid</returns>
        public bool ValidateAudioComponents()
        {
            bool allValid = true;
            
            foreach (var component in audioThemeComponents)
            {
                if (component == null)
                {
                    Debug.LogWarning($"[Audio Theme Manager] Found null audio theme component");
                    allValid = false;
                }
                else
                {
                    // Validate component has audio sources
                    var audioSources = component.GetComponentsInChildren<AudioSource>();
                    if (audioSources.Length == 0)
                    {
                        Debug.LogWarning($"[Audio Theme Manager] Audio theme component '{component.name}' has no audio sources");
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
                    Debug.LogWarning($"[Audio Theme Manager] Removed null themes from available themes");
                }
            }
            
            // Remove null components
            audioThemeComponents.RemoveAll(c => c == null);
        }
        
        [ContextMenu("Auto-Find Audio Theme Components")]
        private void AutoFindAudioThemeComponentsMenu()
        {
            AutoFindAudioThemeComponents();
        }
        
        [ContextMenu("Validate Audio Components")]
        private void ValidateAudioComponentsMenu()
        {
            bool isValid = ValidateAudioComponents();
            Debug.Log($"[Audio Theme Manager] Audio components validation: {(isValid ? "PASSED" : "FAILED")}");
        }
        
        [ContextMenu("Refresh All Audio Components")]
        private void RefreshAllAudioComponentsMenu()
        {
            RefreshAllAudioComponents();
        }
        
        [ContextMenu("Stop All Audio")]
        private void StopAllAudioMenu()
        {
            StopAllAudio();
        }
    }
    
    /// <summary>
    /// Statistics about audio theme components
    /// </summary>
    [System.Serializable]
    public class AudioComponentStats
    {
        public int totalAudioComponents;
        public int activeAudioComponents;
        public int nullAudioComponents;
        public int totalAvailableThemes;
        public bool audioFadingEnabled;
        public bool spatialAudioEnabled;
    }
}
