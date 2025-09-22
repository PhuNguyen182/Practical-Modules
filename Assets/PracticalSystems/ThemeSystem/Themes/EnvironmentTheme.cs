using PracticalSystems.ThemeSystem.Components;
using PracticalSystems.ThemeSystem.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PracticalSystems.ThemeSystem.Themes
{
    /// <summary>
    /// Environment theme data structure
    /// </summary>
    [CreateAssetMenu(fileName = "Environment Theme", menuName = "Theme System/Environment Theme")]
    public class EnvironmentTheme : BaseTheme
    {
        [Header("Environment Theme Settings")]
        [SerializeField] private EnvironmentThemeData themeData = new EnvironmentThemeData();
        
        public EnvironmentThemeData ThemeData => themeData;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            category = "Environment";
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            category = "Environment";
        }
        
        public override bool ApplyTo(IThemeComponent component)
        {
            if (component is EnvironmentThemeComponent envComponent)
            {
                envComponent.ApplyEnvironmentTheme(themeData);
                return base.ApplyTo(component);
            }
            return false;
        }
    }
    
    /// <summary>
    /// Environment theme data container
    /// </summary>
    [System.Serializable]
    public class EnvironmentThemeData
    {
        [Header("Lighting")]
        public Color ambientLightColor = Color.white;
        public float ambientIntensity = 1f;
        public Color fogColor = Color.gray;
        public float fogDensity = 0.01f;
        public FogMode fogMode = FogMode.ExponentialSquared;
        public float fogStartDistance = 10f;
        public float fogEndDistance = 100f;
        
        [Header("Skybox")]
        public Material skyboxMaterial;
        public Color skyboxTint = Color.white;
        public float skyboxExposure = 1f;
        public float skyboxRotation = 0f;
        
        [Header("Post-Processing")]
        public bool enablePostProcessing = true;
        public Color colorGradingColorFilter = Color.white;
        public float colorGradingContrast = 0f;
        public float colorGradingSaturation = 0f;
        public float colorGradingHueShift = 0f;
        public float bloomIntensity = 0.5f;
        public float bloomThreshold = 1f;
        public float vignetteIntensity = 0.2f;
        public float vignetteSmoothness = 0.2f;
        
        [Header("Materials")]
        public Material[] environmentMaterials;
        public Color materialTint = Color.white;
        public float materialMetallic = 0f;
        public float materialSmoothness = 0.5f;
        
        [Header("Particles")]
        public Color particleColor = Color.white;
        public float particleIntensity = 1f;
        public bool enableParticles = true;
        
        [Header("Weather Effects")]
        public bool enableWeather = false;
        public float rainIntensity = 0f;
        public float snowIntensity = 0f;
        public float windIntensity = 0f;
        
        [Header("Time of Day")]
        public float timeOfDay = 12f; // 0-24 hours
        public bool enableTimeProgression = false;
        public float timeSpeed = 1f;
        
        [Header("Atmosphere")]
        public Color atmosphereColor = Color.blue;
        public float atmosphereIntensity = 1f;
        public float atmosphereThickness = 1f;
        public float normalMapIntensity;
    }
}
