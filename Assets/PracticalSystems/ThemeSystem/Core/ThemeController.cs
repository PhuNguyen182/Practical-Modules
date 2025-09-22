using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using PracticalSystems.ThemeSystem.Components;
using PracticalSystems.ThemeSystem.Managers;
using PracticalSystems.ThemeSystem.Themes;

namespace PracticalSystems.ThemeSystem.Core
{
    /// <summary>
    /// Main theme controller that orchestrates all theme managers and provides a unified interface for theme management
    /// </summary>
    public class ThemeController : MonoBehaviour
    {
        [Header("Theme Managers")]
        [SerializeField] private UIThemeManager uiThemeManager;
        [SerializeField] private EnvironmentThemeManager environmentThemeManager;
        [SerializeField] private AudioThemeManager audioThemeManager;
        [SerializeField] private CharacterThemeManager characterThemeManager;
        
        [Header("Theme Controller Settings")]
        [SerializeField] private bool autoInitialize = true;
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool enableThemeTransitions = true;
        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Global Theme Settings")]
        [SerializeField] private ThemePreset[] themePresets;
        [SerializeField] private ThemePreset defaultPreset;
        [SerializeField] private bool autoApplyDefaultPreset = true;
        
        private List<IThemeManager> themeManagers = new List<IThemeManager>();
        private List<IThemeComponent> registeredComponents = new List<IThemeComponent>();
        private Dictionary<string, ITheme> activeThemes = new Dictionary<string, ITheme>();
        private bool isTransitioning = false;
        
        public bool IsInitialized { get; private set; }
        public bool IsTransitioning => isTransitioning;
        public bool DebugMode => debugMode;
        public bool EnableThemeTransitions => enableThemeTransitions;
        public float TransitionDuration => transitionDuration;
        public AnimationCurve TransitionCurve => transitionCurve;
        
        public event Action<string, ITheme> OnThemeApplied;
        public event Action<string, ITheme> OnThemeApplying;
        public event Action<ThemePreset> OnPresetApplied;
        public event Action<ThemePreset> OnPresetApplying;
        public event Action<bool> OnTransitionStateChanged;
        
        protected virtual void Start()
        {
            if (autoInitialize)
            {
                Initialize();
            }
        }
        
        /// <summary>
        /// Initializes the theme controller and all theme managers
        /// </summary>
        public virtual void Initialize()
        {
            if (IsInitialized)
            {
                if (debugMode)
                    Debug.LogWarning("[Theme Controller] Already initialized");
                return;
            }
            
            if (debugMode)
                Debug.Log("[Theme Controller] Initializing...");
            
            // Auto-find theme managers if not assigned
            AutoFindThemeManagers();
            
            // Initialize theme managers
            InitializeThemeManagers();
            
            // Register for theme manager events
            RegisterThemeManagerEvents();
            
            // Apply default preset if specified
            if (autoApplyDefaultPreset && defaultPreset != null)
            {
                ApplyPreset(defaultPreset);
            }
            
            IsInitialized = true;
            
            if (debugMode)
                Debug.Log($"[Theme Controller] Initialized with {themeManagers.Count} theme managers");
        }
        
        /// <summary>
        /// Automatically finds theme managers in the scene
        /// </summary>
        private void AutoFindThemeManagers()
        {
            if (uiThemeManager == null)
                uiThemeManager = FindObjectOfType<UIThemeManager>();
                
            if (environmentThemeManager == null)
                environmentThemeManager = FindObjectOfType<EnvironmentThemeManager>();
                
            if (audioThemeManager == null)
                audioThemeManager = FindObjectOfType<AudioThemeManager>();
                
            if (characterThemeManager == null)
                characterThemeManager = FindObjectOfType<CharacterThemeManager>();
        }
        
        /// <summary>
        /// Initializes all theme managers
        /// </summary>
        private void InitializeThemeManagers()
        {
            themeManagers.Clear();
            
            if (uiThemeManager != null)
            {
                themeManagers.Add(uiThemeManager);
                if (debugMode)
                    Debug.Log("[Theme Controller] Added UI Theme Manager");
            }
            
            if (environmentThemeManager != null)
            {
                themeManagers.Add(environmentThemeManager);
                if (debugMode)
                    Debug.Log("[Theme Controller] Added Environment Theme Manager");
            }
            
            if (audioThemeManager != null)
            {
                themeManagers.Add(audioThemeManager);
                if (debugMode)
                    Debug.Log("[Theme Controller] Added Audio Theme Manager");
            }
            
            if (characterThemeManager != null)
            {
                themeManagers.Add(characterThemeManager);
                if (debugMode)
                    Debug.Log("[Theme Controller] Added Character Theme Manager");
            }
        }
        
        /// <summary>
        /// Registers for theme manager events
        /// </summary>
        private void RegisterThemeManagerEvents()
        {
            foreach (var manager in themeManagers)
            {
                manager.OnThemeApplied += (theme) => OnThemeApplied?.Invoke(manager.Category, theme);
                manager.OnThemeApplying += (theme) => OnThemeApplying?.Invoke(manager.Category, theme);
            }
        }
        
        /// <summary>
        /// Registers a theme component with the appropriate manager
        /// </summary>
        /// <param name="component">The component to register</param>
        public virtual void RegisterComponent(IThemeComponent component)
        {
            if (component == null) return;
            
            if (!registeredComponents.Contains(component))
            {
                registeredComponents.Add(component);
                
                // Find the appropriate manager for this component
                var manager = GetManagerForComponent(component);
                if (manager != null)
                {
                    manager.RegisterComponent(component);
                    
                    if (debugMode)
                        Debug.Log($"[Theme Controller] Registered component {component} with {manager.Category} manager");
                }
                else
                {
                    Debug.LogWarning($"[Theme Controller] No suitable manager found for component {component}");
                }
            }
        }
        
        /// <summary>
        /// Unregisters a theme component from all managers
        /// </summary>
        /// <param name="component">The component to unregister</param>
        public virtual void UnregisterComponent(IThemeComponent component)
        {
            if (component == null) return;
            
            if (registeredComponents.Contains(component))
            {
                registeredComponents.Remove(component);
                
                // Remove from all managers
                foreach (var manager in themeManagers)
                {
                    manager.UnregisterComponent(component);
                }
                
                if (debugMode)
                    Debug.Log($"[Theme Controller] Unregistered component {component}");
            }
        }
        
        /// <summary>
        /// Gets the appropriate manager for a component
        /// </summary>
        /// <param name="component">The component</param>
        /// <returns>The appropriate manager or null</returns>
        private IThemeManager GetManagerForComponent(IThemeComponent component)
        {
            if (component is UIThemeComponent)
                return uiThemeManager;
            else if (component is EnvironmentThemeComponent)
                return environmentThemeManager;
            else if (component is AudioThemeComponent)
                return audioThemeManager;
            else if (component is CharacterThemeComponent)
                return characterThemeManager;
                
            return null;
        }
        
        /// <summary>
        /// Applies a theme to a specific category
        /// </summary>
        /// <param name="category">The theme category</param>
        /// <param name="theme">The theme to apply</param>
        /// <param name="useTransition">Whether to use smooth transitions</param>
        public virtual void ApplyTheme(string category, ITheme theme, bool useTransition = true)
        {
            if (isTransitioning && useTransition)
            {
                Debug.LogWarning($"[Theme Controller] Cannot apply theme while transitioning");
                return;
            }
            
            var manager = GetManagerByCategory(category);
            if (manager != null)
            {
                if (useTransition && enableThemeTransitions)
                {
                    StartCoroutine(TransitionToTheme(manager, theme));
                }
                else
                {
                    manager.ApplyTheme(theme);
                    activeThemes[category] = theme;
                }
                
                if (debugMode)
                    Debug.Log($"[Theme Controller] Applied theme '{theme.ThemeName}' to category '{category}'");
            }
            else
            {
                Debug.LogWarning($"[Theme Controller] No manager found for category '{category}'");
            }
        }
        
        /// <summary>
        /// Applies a theme preset to all categories
        /// </summary>
        /// <param name="preset">The preset to apply</param>
        /// <param name="useTransition">Whether to use smooth transitions</param>
        public virtual void ApplyPreset(ThemePreset preset, bool useTransition = true)
        {
            if (preset == null)
            {
                Debug.LogWarning("[Theme Controller] Cannot apply null preset");
                return;
            }
            
            if (isTransitioning && useTransition)
            {
                Debug.LogWarning("[Theme Controller] Cannot apply preset while transitioning");
                return;
            }
            
            if (useTransition && enableThemeTransitions)
            {
                StartCoroutine(TransitionToPreset(preset));
            }
            else
            {
                ApplyPresetImmediate(preset);
            }
            
            if (debugMode)
                Debug.Log($"[Theme Controller] Applied preset '{preset.presetName}'");
        }
        
        /// <summary>
        /// Applies a preset immediately without transitions
        /// </summary>
        /// <param name="preset">The preset to apply</param>
        private void ApplyPresetImmediate(ThemePreset preset)
        {
            OnPresetApplying?.Invoke(preset);
            
            if (preset.uiTheme != null && uiThemeManager != null)
            {
                uiThemeManager.ApplyTheme(preset.uiTheme);
                activeThemes["UI"] = preset.uiTheme;
            }
            
            if (preset.environmentTheme != null && environmentThemeManager != null)
            {
                environmentThemeManager.ApplyTheme(preset.environmentTheme);
                activeThemes["Environment"] = preset.environmentTheme;
            }
            
            if (preset.audioTheme != null && audioThemeManager != null)
            {
                audioThemeManager.ApplyTheme(preset.audioTheme);
                activeThemes["Audio"] = preset.audioTheme;
            }
            
            if (preset.characterTheme != null && characterThemeManager != null)
            {
                characterThemeManager.ApplyTheme(preset.characterTheme);
                activeThemes["Character"] = preset.characterTheme;
            }
            
            OnPresetApplied?.Invoke(preset);
        }
        
        /// <summary>
        /// Transitions to a theme with smooth animation
        /// </summary>
        /// <param name="manager">The theme manager</param>
        /// <param name="theme">The target theme</param>
        private IEnumerator TransitionToTheme(IThemeManager manager, ITheme theme)
        {
            isTransitioning = true;
            OnTransitionStateChanged?.Invoke(true);
            
            OnThemeApplying?.Invoke(manager.Category, theme);
            
            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = transitionCurve.Evaluate(elapsed / transitionDuration);
                
                // Apply theme at the end of transition
                if (elapsed >= transitionDuration)
                {
                    manager.ApplyTheme(theme);
                    activeThemes[manager.Category] = theme;
                }
                
                yield return null;
            }
            
            isTransitioning = false;
            OnTransitionStateChanged?.Invoke(false);
        }
        
        /// <summary>
        /// Transitions to a preset with smooth animation
        /// </summary>
        /// <param name="preset">The target preset</param>
        private IEnumerator TransitionToPreset(ThemePreset preset)
        {
            isTransitioning = true;
            OnTransitionStateChanged?.Invoke(true);
            
            OnPresetApplying?.Invoke(preset);
            
            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = transitionCurve.Evaluate(elapsed / transitionDuration);
                
                // Apply preset at the end of transition
                if (elapsed >= transitionDuration)
                {
                    ApplyPresetImmediate(preset);
                }
                
                yield return null;
            }
            
            isTransitioning = false;
            OnTransitionStateChanged?.Invoke(false);
        }
        
        /// <summary>
        /// Gets a manager by category name
        /// </summary>
        /// <param name="category">The category name</param>
        /// <returns>The manager or null</returns>
        private IThemeManager GetManagerByCategory(string category)
        {
            return themeManagers.FirstOrDefault(m => m.Category == category);
        }
        
        /// <summary>
        /// Gets the currently active theme for a category
        /// </summary>
        /// <param name="category">The category</param>
        /// <returns>The active theme or null</returns>
        public ITheme GetActiveTheme(string category)
        {
            activeThemes.TryGetValue(category, out var theme);
            return theme;
        }
        
        /// <summary>
        /// Gets all active themes
        /// </summary>
        /// <returns>Dictionary of active themes</returns>
        public Dictionary<string, ITheme> GetAllActiveThemes()
        {
            return new Dictionary<string, ITheme>(activeThemes);
        }
        
        /// <summary>
        /// Gets a preset by name
        /// </summary>
        /// <param name="presetName">The preset name</param>
        /// <returns>The preset or null</returns>
        public ThemePreset GetPresetByName(string presetName)
        {
            return themePresets?.FirstOrDefault(p => p.presetName == presetName);
        }
        
        /// <summary>
        /// Gets all available preset names
        /// </summary>
        /// <returns>Array of preset names</returns>
        public string[] GetAvailablePresetNames()
        {
            return themePresets?.Select(p => p.presetName).ToArray() ?? new string[0];
        }
        
        /// <summary>
        /// Refreshes all themes across all managers
        /// </summary>
        public void RefreshAllThemes()
        {
            foreach (var manager in themeManagers)
            {
                manager.RefreshCurrentTheme();
            }
            
            if (debugMode)
                Debug.Log("[Theme Controller] Refreshed all themes");
        }
        
        /// <summary>
        /// Gets statistics about the theme system
        /// </summary>
        /// <returns>Theme system statistics</returns>
        public ThemeSystemStats GetSystemStats()
        {
            return new ThemeSystemStats
            {
                totalManagers = themeManagers.Count,
                totalRegisteredComponents = registeredComponents.Count,
                activeThemes = activeThemes.Count,
                totalPresets = themePresets?.Length ?? 0,
                isTransitioning = isTransitioning,
                isInitialized = IsInitialized
            };
        }
        
        /// <summary>
        /// Validates the theme system
        /// </summary>
        /// <returns>True if valid</returns>
        public bool ValidateSystem()
        {
            bool isValid = true;
            
            // Check if managers are assigned
            if (themeManagers.Count == 0)
            {
                Debug.LogWarning("[Theme Controller] No theme managers assigned");
                isValid = false;
            }
            
            // Check for null components
            var nullComponents = registeredComponents.Count(c => c == null);
            if (nullComponents > 0)
            {
                Debug.LogWarning($"[Theme Controller] Found {nullComponents} null components");
                isValid = false;
            }
            
            return isValid;
        }
        
        protected virtual void OnDestroy()
        {
            // Unregister all components
            foreach (var component in registeredComponents.ToList())
            {
                UnregisterComponent(component);
            }
            
            registeredComponents.Clear();
            themeManagers.Clear();
            activeThemes.Clear();
        }
        
        [ContextMenu("Initialize Theme Controller")]
        private void InitializeMenu()
        {
            Initialize();
        }
        
        [ContextMenu("Refresh All Themes")]
        private void RefreshAllThemesMenu()
        {
            RefreshAllThemes();
        }
        
        [ContextMenu("Validate System")]
        private void ValidateSystemMenu()
        {
            bool isValid = ValidateSystem();
            Debug.Log($"[Theme Controller] System validation: {(isValid ? "PASSED" : "FAILED")}");
        }
    }
    
    /// <summary>
    /// Theme preset that contains themes for all categories
    /// </summary>
    [System.Serializable]
    public class ThemePreset
    {
        [Header("Preset Information")]
        public string presetName;
        public string description;
        
        [Header("Themes")]
        public UITheme uiTheme;
        public EnvironmentTheme environmentTheme;
        public AudioTheme audioTheme;
        public CharacterTheme characterTheme;
    }
    
    /// <summary>
    /// Statistics about the theme system
    /// </summary>
    [System.Serializable]
    public class ThemeSystemStats
    {
        public int totalManagers;
        public int totalRegisteredComponents;
        public int activeThemes;
        public int totalPresets;
        public bool isTransitioning;
        public bool isInitialized;
    }
}
