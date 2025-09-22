using UnityEngine;
using UnityEngine.Rendering;
using PracticalSystems.ThemeSystem.Core;
using PracticalSystems.ThemeSystem.Themes;
using UnityEngine.Rendering.PostProcessing;
using Bloom = UnityEngine.Rendering.Universal.Bloom;
using Vignette = UnityEngine.Rendering.Universal.Vignette;

namespace PracticalSystems.ThemeSystem.Components
{
    /// <summary>
    /// Component that applies environment themes to lighting, skybox, and post-processing
    /// </summary>
    public class EnvironmentThemeComponent : BaseThemeComponent
    {
        [Header("Environment References")]
        [SerializeField] private Light[] lights;
        [SerializeField] private Renderer[] environmentRenderers;
        [SerializeField] private ParticleSystem[] particleSystems;
        [SerializeField] private Volume postProcessVolume;
        [SerializeField] private Material skyboxMaterial;
        
        [Header("Environment Settings")]
        [SerializeField] private bool applyLighting = true;
        [SerializeField] private bool applySkybox = true;
        [SerializeField] private bool applyPostProcessing = true;
        [SerializeField] private bool applyMaterials = true;
        [SerializeField] private bool applyParticles = true;
        
        private EnvironmentThemeData currentThemeData;
        private ColorGrading colorGrading;
        private Bloom bloom;
        private Vignette vignette;
        private Fog fog;
        
        protected override void Start()
        {
            base.Start();
            InitializePostProcessing();
            AutoFindEnvironmentElements();
        }
        
        /// <summary>
        /// Initializes post-processing components
        /// </summary>
        private void InitializePostProcessing()
        {
            if (postProcessVolume != null && postProcessVolume.profile != null)
            {
                //postProcessVolume.profile.get.TryGet(out colorGrading);
                postProcessVolume.profile.TryGet(out bloom);
                postProcessVolume.profile.TryGet(out vignette);
                //postProcessVolume.profile.TryGet(out fog);
            }
        }
        
        /// <summary>
        /// Automatically finds environment elements in the scene
        /// </summary>
        private void AutoFindEnvironmentElements()
        {
            if (lights == null || lights.Length == 0)
                lights = FindObjectsOfType<Light>();
                
            if (environmentRenderers == null || environmentRenderers.Length == 0)
            {
                var allRenderers = FindObjectsOfType<Renderer>();
                var envRenderers = new System.Collections.Generic.List<Renderer>();
                
                foreach (var renderer in allRenderers)
                {
                    // Filter for environment objects (not UI, not characters)
                    if (!renderer.CompareTag("UI") && !renderer.CompareTag("Player") && !renderer.CompareTag("Character"))
                    {
                        envRenderers.Add(renderer);
                    }
                }
                
                environmentRenderers = envRenderers.ToArray();
            }
            
            if (particleSystems == null || particleSystems.Length == 0)
                particleSystems = FindObjectsOfType<ParticleSystem>();
                
            if (postProcessVolume == null)
                postProcessVolume = FindObjectOfType<Volume>();
        }
        
        protected override void OnThemeApplied(ITheme theme)
        {
            if (theme is EnvironmentTheme envTheme)
            {
                ApplyEnvironmentTheme(envTheme.ThemeData);
            }
        }
        
        /// <summary>
        /// Applies environment theme data to all environment elements
        /// </summary>
        /// <param name="themeData">The environment theme data to apply</param>
        public void ApplyEnvironmentTheme(EnvironmentThemeData themeData)
        {
            currentThemeData = themeData;
            
            if (applyLighting)
                ApplyLightingTheme(themeData);
                
            if (applySkybox)
                ApplySkyboxTheme(themeData);
                
            if (applyPostProcessing)
                ApplyPostProcessingTheme(themeData);
                
            if (applyMaterials)
                ApplyMaterialTheme(themeData);
                
            if (applyParticles)
                ApplyParticleTheme(themeData);
                
            // Apply fog
            ApplyFogTheme(themeData);
        }
        
        private void ApplyLightingTheme(EnvironmentThemeData themeData)
        {
            // Apply ambient lighting
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = themeData.ambientLightColor;
            RenderSettings.ambientEquatorColor = themeData.ambientLightColor;
            RenderSettings.ambientGroundColor = themeData.ambientLightColor * 0.5f;
            RenderSettings.ambientIntensity = themeData.ambientIntensity;
            
            // Apply to scene lights
            foreach (var light in lights)
            {
                if (light == null) continue;
                
                // Apply light color and intensity based on light type
                switch (light.type)
                {
                    case LightType.Directional:
                        light.color = themeData.ambientLightColor;
                        light.intensity = themeData.ambientIntensity;
                        break;
                        
                    case LightType.Point:
                    case LightType.Spot:
                        light.color = themeData.ambientLightColor;
                        light.intensity = themeData.ambientIntensity * 0.5f;
                        break;
                }
            }
        }
        
        private void ApplySkyboxTheme(EnvironmentThemeData themeData)
        {
            if (themeData.skyboxMaterial != null)
            {
                RenderSettings.skybox = themeData.skyboxMaterial;
                
                // Apply skybox properties
                RenderSettings.skybox.SetColor("_Tint", themeData.skyboxTint);
                RenderSettings.skybox.SetFloat("_Exposure", themeData.skyboxExposure);
                RenderSettings.skybox.SetFloat("_Rotation", themeData.skyboxRotation);
            }
        }
        
        private void ApplyPostProcessingTheme(EnvironmentThemeData themeData)
        {
            if (!themeData.enablePostProcessing || postProcessVolume == null)
                return;
                
            // Apply Color Grading
            if (colorGrading != null)
            {
                colorGrading.active = true;
                colorGrading.colorFilter.value = themeData.colorGradingColorFilter;
                colorGrading.contrast.value = themeData.colorGradingContrast;
                colorGrading.saturation.value = themeData.colorGradingSaturation;
                colorGrading.hueShift.value = themeData.colorGradingHueShift;
            }
            
            // Apply Bloom
            if (bloom != null)
            {
                bloom.active = true;
                bloom.intensity.value = themeData.bloomIntensity;
                bloom.threshold.value = themeData.bloomThreshold;
            }
            
            // Apply Vignette
            if (vignette != null)
            {
                vignette.active = true;
                vignette.intensity.value = themeData.vignetteIntensity;
                vignette.smoothness.value = themeData.vignetteSmoothness;
            }
        }
        
        private void ApplyMaterialTheme(EnvironmentThemeData themeData)
        {
            foreach (var renderer in environmentRenderers)
            {
                if (renderer == null) continue;
                
                var materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null) continue;
                    
                    var material = materials[i];
                    
                    // Apply material tint
                    if (material.HasProperty("_Color"))
                    {
                        material.color = Color.Lerp(material.color, themeData.materialTint, 0.5f);
                    }
                    
                    // Apply metallic and smoothness
                    if (material.HasProperty("_Metallic"))
                    {
                        material.SetFloat("_Metallic", themeData.materialMetallic);
                    }
                    
                    if (material.HasProperty("_Smoothness"))
                    {
                        material.SetFloat("_Smoothness", themeData.materialSmoothness);
                    }
                    
                    // Apply normal map intensity
                    if (material.HasProperty("_BumpScale"))
                    {
                        material.SetFloat("_BumpScale", themeData.normalMapIntensity);
                    }
                }
            }
        }
        
        private void ApplyParticleTheme(EnvironmentThemeData themeData)
        {
            foreach (var particleSystem in particleSystems)
            {
                if (particleSystem == null) continue;
                
                var main = particleSystem.main;
                main.startColor = themeData.particleColor;
                
                if (themeData.enableParticles)
                {
                    particleSystem.Play();
                }
                else
                {
                    particleSystem.Stop();
                }
            }
        }
        
        private void ApplyFogTheme(EnvironmentThemeData themeData)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = themeData.fogColor;
            RenderSettings.fogMode = themeData.fogMode;
            RenderSettings.fogDensity = themeData.fogDensity;
            RenderSettings.fogStartDistance = themeData.fogStartDistance;
            RenderSettings.fogEndDistance = themeData.fogEndDistance;
        }
        
        /// <summary>
        /// Updates time of day based on theme data
        /// </summary>
        private void UpdateTimeOfDay()
        {
            if (currentThemeData == null || !currentThemeData.enableTimeProgression)
                return;
                
            // Update time of day
            currentThemeData.timeOfDay += Time.deltaTime * currentThemeData.timeSpeed / 3600f; // Convert to hours
            
            if (currentThemeData.timeOfDay >= 24f)
                currentThemeData.timeOfDay = 0f;
                
            // Update lighting based on time of day
            UpdateLightingForTimeOfDay(currentThemeData.timeOfDay);
        }
        
        private void UpdateLightingForTimeOfDay(float timeOfDay)
        {
            Color dayColor = Color.white;
            Color nightColor = new Color(0.3f, 0.3f, 0.6f, 1f);
            Color dawnColor = new Color(1f, 0.7f, 0.5f, 1f);
            Color duskColor = new Color(1f, 0.5f, 0.3f, 1f);
            
            Color currentColor = dayColor;
            
            if (timeOfDay >= 6f && timeOfDay < 8f) // Dawn
            {
                float t = (timeOfDay - 6f) / 2f;
                currentColor = Color.Lerp(nightColor, dawnColor, t);
            }
            else if (timeOfDay >= 8f && timeOfDay < 18f) // Day
            {
                float t = (timeOfDay - 8f) / 10f;
                currentColor = Color.Lerp(dawnColor, dayColor, t);
            }
            else if (timeOfDay >= 18f && timeOfDay < 20f) // Dusk
            {
                float t = (timeOfDay - 18f) / 2f;
                currentColor = Color.Lerp(dayColor, duskColor, t);
            }
            else // Night
            {
                currentColor = nightColor;
            }
            
            // Apply color to ambient lighting
            RenderSettings.ambientSkyColor = currentColor;
            RenderSettings.ambientEquatorColor = currentColor;
            RenderSettings.ambientGroundColor = currentColor * 0.5f;
        }
        
        private void Update()
        {
            if (currentThemeData != null && currentThemeData.enableTimeProgression)
            {
                UpdateTimeOfDay();
            }
        }
        
        /// <summary>
        /// Applies weather effects based on theme data
        /// </summary>
        public void ApplyWeatherEffects()
        {
            if (currentThemeData == null) return;
            
            // Apply rain effects
            if (currentThemeData.rainIntensity > 0f)
            {
                ApplyRainEffect(currentThemeData.rainIntensity);
            }
            
            // Apply snow effects
            if (currentThemeData.snowIntensity > 0f)
            {
                ApplySnowEffect(currentThemeData.snowIntensity);
            }
            
            // Apply wind effects
            if (currentThemeData.windIntensity > 0f)
            {
                ApplyWindEffect(currentThemeData.windIntensity);
            }
        }
        
        private void ApplyRainEffect(float intensity)
        {
            // This would require rain particle systems or shader effects
            // For now, we'll just adjust fog density to simulate rain
            RenderSettings.fogDensity = currentThemeData.fogDensity * (1f + intensity);
        }
        
        private void ApplySnowEffect(float intensity)
        {
            // This would require snow particle systems
            // For now, we'll adjust ambient lighting to be cooler
            RenderSettings.ambientSkyColor = Color.Lerp(currentThemeData.ambientLightColor, Color.white, intensity * 0.3f);
        }
        
        private void ApplyWindEffect(float intensity)
        {
            // This would require wind effects on particle systems and vegetation
            foreach (var particleSystem in particleSystems)
            {
                if (particleSystem == null) continue;
                
                var velocityOverLifetime = particleSystem.velocityOverLifetime;
                velocityOverLifetime.enabled = true;
                velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
                velocityOverLifetime.x = intensity * 5f;
                velocityOverLifetime.z = intensity * 3f;
            }
        }
        
        [ContextMenu("Auto-Find Environment Elements")]
        private void AutoFindEnvironmentElementsMenu()
        {
            AutoFindEnvironmentElements();
        }
    }
}
