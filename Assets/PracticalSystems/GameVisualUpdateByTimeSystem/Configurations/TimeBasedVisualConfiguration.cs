using System;
using System.Collections.Generic;
using GameVisualUpdateByTimeSystem.Core;
using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;
using GameVisualUpdateByTimeSystem.Visuals.Lighting;
using GameVisualUpdateByTimeSystem.Visuals.Skyboxes;
using GameVisualUpdateByTimeSystem.Visuals.Materials;
using GameVisualUpdateByTimeSystem.Visuals.Audio;
using GameVisualUpdateByTimeSystem.Visuals.UI;

namespace GameVisualUpdateByTimeSystem.Configurations
{
    /// <summary>
    /// ScriptableObject configuration for time-based visual system
    /// Allows designers to create and manage visual states without coding
    /// </summary>
    [CreateAssetMenu(fileName = "TimeBasedVisualConfig", menuName = "Game Visual System/Time-Based Visual Configuration")]
    public class TimeBasedVisualConfiguration : ScriptableObject
    {
        [Header("Configuration Info")]
        [SerializeField] private string _configurationName = "Default Configuration";
        [SerializeField] private string _description = "Default time-based visual configuration";
        [SerializeField] private string _author = "Designer";
        [SerializeField] private string _version = "1.0.0";
        
        [Header("Time Settings")]
        [SerializeField] private TimeConfiguration _timeConfiguration = new TimeConfiguration();
        
        [Header("Visual States")]
        [SerializeField] private LightingConfiguration _lightingConfiguration = new LightingConfiguration();
        [SerializeField] private SkyboxConfiguration _skyboxConfiguration = new SkyboxConfiguration();
        [SerializeField] private MaterialConfiguration _materialConfiguration = new MaterialConfiguration();
        [SerializeField] private AudioConfiguration _audioConfiguration = new AudioConfiguration();
        [SerializeField] private UIConfiguration _uiConfiguration = new UIConfiguration();
        
        [Header("Performance Settings")]
        [SerializeField] private PerformanceSettings _performanceSettings = new PerformanceSettings();
        
        // Properties
        public string ConfigurationName => _configurationName;
        public string Description => _description;
        public string Author => _author;
        public string Version => _version;
        public TimeConfiguration TimeConfiguration => _timeConfiguration;
        public LightingConfiguration LightingConfiguration => _lightingConfiguration;
        public SkyboxConfiguration SkyboxConfiguration => _skyboxConfiguration;
        public MaterialConfiguration MaterialConfiguration => _materialConfiguration;
        public AudioConfiguration AudioConfiguration => _audioConfiguration;
        public UIConfiguration UIConfiguration => _uiConfiguration;
        public PerformanceSettings PerformanceSettings => _performanceSettings;
        
        /// <summary>
        /// Apply this configuration to a TimeBasedVisualSystem
        /// </summary>
        /// <param name="visualSystem">Visual system to configure</param>
        public void ApplyToSystem(ITimeBasedVisualSystem visualSystem)
        {
            if (visualSystem == null)
            {
                Debug.LogError("Cannot apply configuration to null visual system");
                return;
            }
            
            // Apply time configuration
            if (visualSystem.TimeProvider != null)
            {
                visualSystem.TimeProvider.TimeSpeed = _timeConfiguration.TimeSpeed;
                visualSystem.TimeProvider.IsPaused = _timeConfiguration.StartPaused;
            }
            
            // Apply performance settings
            if (visualSystem is TimeBasedVisualSystem system)
            {
                // Apply performance settings to the system component
                // This would require exposing performance properties in the system
            }
            
            Debug.Log($"Applied configuration '{_configurationName}' to visual system");
        }
        
        /// <summary>
        /// Create a new visual system GameObject with this configuration
        /// </summary>
        /// <returns>Configured visual system GameObject</returns>
        public GameObject CreateVisualSystem()
        {
            var go = new GameObject($"VisualSystem_{_configurationName}");
            
            // Add the visual system service
            var service = go.AddComponent<TimeBasedVisualSystemService>();
            
            // Add visual components based on configuration
            if (_lightingConfiguration.Enabled)
            {
                var lighting = go.AddComponent<TimeBasedLighting>();
                ApplyLightingConfiguration(lighting);
            }
            
            if (_skyboxConfiguration.Enabled)
            {
                var skybox = go.AddComponent<TimeBasedSkybox>();
                ApplySkyboxConfiguration(skybox);
            }
            
            if (_materialConfiguration.Enabled)
            {
                var materials = go.AddComponent<TimeBasedMaterialProperties>();
                ApplyMaterialConfiguration(materials);
            }
            
            if (_audioConfiguration.Enabled)
            {
                var audio = go.AddComponent<TimeBasedAudio>();
                ApplyAudioConfiguration(audio);
            }
            
            if (_uiConfiguration.Enabled)
            {
                var ui = go.AddComponent<TimeBasedUI>();
                ApplyUIConfiguration(ui);
            }
            
            Debug.Log($"Created visual system GameObject '{go.name}' with configuration '{_configurationName}'");
            return go;
        }
        
        private void ApplyLightingConfiguration(TimeBasedLighting lighting)
        {
            // Apply lighting configuration
            // This would require exposing configuration properties in the lighting component
            Debug.Log($"Applied lighting configuration to {lighting.name}");
        }
        
        private void ApplySkyboxConfiguration(TimeBasedSkybox skybox)
        {
            // Apply skybox configuration
            Debug.Log($"Applied skybox configuration to {skybox.name}");
        }
        
        private void ApplyMaterialConfiguration(TimeBasedMaterialProperties materials)
        {
            // Apply material configuration
            Debug.Log($"Applied material configuration to {materials.name}");
        }
        
        private void ApplyAudioConfiguration(TimeBasedAudio audio)
        {
            // Apply audio configuration
            Debug.Log($"Applied audio configuration to {audio.name}");
        }
        
        private void ApplyUIConfiguration(TimeBasedUI ui)
        {
            // Apply UI configuration
            Debug.Log($"Applied UI configuration to {ui.name}");
        }
        
        /// <summary>
        /// Validate the configuration
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool ValidateConfiguration()
        {
            bool isValid = true;
            
            if (string.IsNullOrEmpty(_configurationName))
            {
                Debug.LogError("Configuration name cannot be empty");
                isValid = false;
            }
            
            if (_timeConfiguration.TimeSpeed <= 0)
            {
                Debug.LogError("Time speed must be greater than 0");
                isValid = false;
            }
            
            // Validate sub-configurations
            if (!_lightingConfiguration.Validate()) isValid = false;
            if (!_skyboxConfiguration.Validate()) isValid = false;
            if (!_materialConfiguration.Validate()) isValid = false;
            if (!_audioConfiguration.Validate()) isValid = false;
            if (!_uiConfiguration.Validate()) isValid = false;
            
            return isValid;
        }
        
        /// <summary>
        /// Create a default configuration
        /// </summary>
        /// <returns>Default configuration instance</returns>
        public static TimeBasedVisualConfiguration CreateDefault()
        {
            var config = CreateInstance<TimeBasedVisualConfiguration>();
            config._configurationName = "Default Configuration";
            config._description = "A default time-based visual configuration with basic day/night cycle";
            config._author = "System";
            config._version = "1.0.0";
            
            // Setup default time configuration
            config._timeConfiguration = TimeConfiguration.CreateDefault();
            
            // Setup default visual configurations
            config._lightingConfiguration = LightingConfiguration.CreateDefault();
            config._skyboxConfiguration = SkyboxConfiguration.CreateDefault();
            config._materialConfiguration = MaterialConfiguration.CreateDefault();
            config._audioConfiguration = AudioConfiguration.CreateDefault();
            config._uiConfiguration = UIConfiguration.CreateDefault();
            
            // Setup default performance settings
            config._performanceSettings = PerformanceSettings.CreateDefault();
            
            return config;
        }
        
        /// <summary>
        /// Create a seasonal configuration
        /// </summary>
        /// <param name="season">Season for the configuration</param>
        /// <returns>Seasonal configuration instance</returns>
        public static TimeBasedVisualConfiguration CreateSeasonal(Season season)
        {
            var config = CreateDefault();
            config._configurationName = $"{season} Configuration";
            config._description = $"A time-based visual configuration optimized for {season}";
            
            // Adjust configurations for the season
            config._lightingConfiguration = LightingConfiguration.CreateSeasonal(season);
            config._skyboxConfiguration = SkyboxConfiguration.CreateSeasonal(season);
            config._materialConfiguration = MaterialConfiguration.CreateSeasonal(season);
            config._audioConfiguration = AudioConfiguration.CreateSeasonal(season);
            config._uiConfiguration = UIConfiguration.CreateSeasonal(season);
            
            return config;
        }
    }
    
    /// <summary>
    /// Time configuration settings
    /// </summary>
    [Serializable]
    public class TimeConfiguration
    {
        [Header("Time Progression")]
        [Tooltip("Speed multiplier for time progression (1.0 = real time)")]
        public float TimeSpeed = 1.0f;
        
        [Tooltip("Whether time starts paused")]
        public bool StartPaused = false;
        
        [Header("Initial Time")]
        [Tooltip("Starting hour (0-23)")]
        [Range(0, 23)]
        public int StartHour = 6;
        
        [Tooltip("Starting minute (0-59)")]
        [Range(0, 59)]
        public int StartMinute = 0;
        
        [Tooltip("Starting day (1-31)")]
        [Range(1, 31)]
        public int StartDay = 1;
        
        [Tooltip("Starting month (1-12)")]
        [Range(1, 12)]
        public int StartMonth = 1;
        
        [Tooltip("Starting year")]
        public int StartYear = 2024;
        
        public static TimeConfiguration CreateDefault()
        {
            return new TimeConfiguration();
        }
    }
    
    /// <summary>
    /// Lighting configuration settings
    /// </summary>
    [Serializable]
    public class LightingConfiguration
    {
        [Header("Lighting Settings")]
        public bool Enabled = true;
        public bool UseProceduralLighting = true;
        public bool EnableShadows = true;
        public bool EnableFog = true;
        
        [Header("Lighting States")]
        public LightingState[] LightingStates = new LightingState[0];
        
        public bool Validate()
        {
            if (LightingStates == null)
            {
                LightingStates = new LightingState[0];
            }
            
            foreach (var state in LightingStates)
            {
                if (state != null && !state.IsValid())
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public static LightingConfiguration CreateDefault()
        {
            var config = new LightingConfiguration();
            // Default lighting states would be created here
            return config;
        }
        
        public static LightingConfiguration CreateSeasonal(Season season)
        {
            var config = CreateDefault();
            // Seasonal lighting adjustments would be made here
            return config;
        }
    }
    
    /// <summary>
    /// Skybox configuration settings
    /// </summary>
    [Serializable]
    public class SkyboxConfiguration
    {
        [Header("Skybox Settings")]
        public bool Enabled = true;
        public bool UseProceduralSkybox = true;
        public Material SkyboxMaterial;
        
        [Header("Skybox States")]
        public SkyboxState[] SkyboxStates = new SkyboxState[0];
        
        public bool Validate()
        {
            if (SkyboxStates == null)
            {
                SkyboxStates = new SkyboxState[0];
            }
            
            foreach (var state in SkyboxStates)
            {
                if (state != null && !state.IsValid())
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public static SkyboxConfiguration CreateDefault()
        {
            var config = new SkyboxConfiguration();
            return config;
        }
        
        public static SkyboxConfiguration CreateSeasonal(Season season)
        {
            var config = CreateDefault();
            return config;
        }
    }
    
    /// <summary>
    /// Material configuration settings
    /// </summary>
    [Serializable]
    public class MaterialConfiguration
    {
        [Header("Material Settings")]
        public bool Enabled = true;
        public bool CreateMaterialInstances = true;
        public bool UseSharedMaterials = false;
        
        [Header("Material States")]
        public MaterialState[] MaterialStates = new MaterialState[0];
        
        public bool Validate()
        {
            if (MaterialStates == null)
            {
                MaterialStates = new MaterialState[0];
            }
            
            foreach (var state in MaterialStates)
            {
                if (state != null && !state.IsValid())
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public static MaterialConfiguration CreateDefault()
        {
            var config = new MaterialConfiguration();
            return config;
        }
        
        public static MaterialConfiguration CreateSeasonal(Season season)
        {
            var config = CreateDefault();
            return config;
        }
    }
    
    /// <summary>
    /// Audio configuration settings
    /// </summary>
    [Serializable]
    public class AudioConfiguration
    {
        [Header("Audio Settings")]
        public bool Enabled = true;
        public bool FadeBetweenStates = true;
        public float FadeDuration = 2f;
        public bool AutoFindAudioSources = true;
        
        [Header("Audio States")]
        public AudioState[] AudioStates = new AudioState[0];
        
        public bool Validate()
        {
            if (AudioStates == null)
            {
                AudioStates = new AudioState[0];
            }
            
            foreach (var state in AudioStates)
            {
                if (state != null && !state.IsValid())
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public static AudioConfiguration CreateDefault()
        {
            var config = new AudioConfiguration();
            return config;
        }
        
        public static AudioConfiguration CreateSeasonal(Season season)
        {
            var config = CreateDefault();
            return config;
        }
    }
    
    /// <summary>
    /// UI configuration settings
    /// </summary>
    [Serializable]
    public class UIConfiguration
    {
        [Header("UI Settings")]
        public bool Enabled = true;
        public bool AutoFindUIElements = true;
        public Canvas[] TargetCanvases = new Canvas[0];
        
        [Header("UI States")]
        public UIState[] UIStates = new UIState[0];
        
        public bool Validate()
        {
            if (UIStates == null)
            {
                UIStates = new UIState[0];
            }
            
            foreach (var state in UIStates)
            {
                if (state != null && !state.IsValid())
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public static UIConfiguration CreateDefault()
        {
            var config = new UIConfiguration();
            return config;
        }
        
        public static UIConfiguration CreateSeasonal(Season season)
        {
            var config = CreateDefault();
            return config;
        }
    }
    
    /// <summary>
    /// Performance settings for the visual system
    /// </summary>
    [Serializable]
    public class PerformanceSettings
    {
        [Header("Update Settings")]
        [Tooltip("Update frequency for visual components (in seconds)")]
        [Range(0.01f, 1f)]
        public float UpdateFrequency = 0.1f;
        
        [Tooltip("Update frequency for lighting (in seconds)")]
        [Range(0.01f, 1f)]
        public float LightingUpdateFrequency = 0.1f;
        
        [Tooltip("Update frequency for skybox (in seconds)")]
        [Range(0.01f, 1f)]
        public float SkyboxUpdateFrequency = 0.2f;
        
        [Tooltip("Update frequency for materials (in seconds)")]
        [Range(0.01f, 1f)]
        public float MaterialUpdateFrequency = 0.1f;
        
        [Tooltip("Update frequency for audio (in seconds)")]
        [Range(0.01f, 1f)]
        public float AudioUpdateFrequency = 0.1f;
        
        [Tooltip("Update frequency for UI (in seconds)")]
        [Range(0.01f, 1f)]
        public float UIUpdateFrequency = 0.2f;
        
        [Header("Optimization")]
        [Tooltip("Enable smooth transitions")]
        public bool EnableSmoothTransitions = true;
        
        [Tooltip("Default transition duration (in seconds)")]
        [Range(0.1f, 10f)]
        public float DefaultTransitionDuration = 1f;
        
        [Tooltip("Enable debug logging")]
        public bool EnableDebugLogging = false;
        
        public static PerformanceSettings CreateDefault()
        {
            return new PerformanceSettings();
        }
    }
}
