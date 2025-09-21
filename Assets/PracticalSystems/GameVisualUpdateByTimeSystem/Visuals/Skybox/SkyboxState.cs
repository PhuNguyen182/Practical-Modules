using System;
using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;

namespace GameVisualUpdateByTimeSystem.Visuals.Skyboxes
{
    /// <summary>
    /// Represents a complete skybox state for time-based skybox transitions
    /// </summary>
    [Serializable]
    public class SkyboxState : IVisualState
    {
        [Header("State Configuration")]
        public string stateId;
        public TimeCondition timeCondition;
        
        [Header("Procedural Skybox Properties")]
        public Color SkyTint = new Color(0.5f, 0.7f, 1f, 1f);
        public Color GroundColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        public float Exposure = 1.3f;
        public float SunSize = 0.04f;
        public float SunSizeConvergence = 5f;
        public float AtmosphereThickness = 1f;
        public Vector3 SunDirection = new Vector3(0.3f, 0.9f, -0.3f);
        
        [Header("Material Skybox")]
        public Material SkyboxMaterial;
        
        [Header("Advanced Properties")]
        public float FogHeight = 0f;
        public float FogHeightDensity = 0f;
        public float FogStart = 0f;
        public float FogEnd = 300f;
        
        public string StateId => stateId;
        public TimeCondition TimeCondition => TimeCondition;
        
        public SkyboxState()
        {
            stateId = "Default";
            timeCondition = new TimeCondition();
        }
        
        public SkyboxState(string stateId, TimeCondition timeCondition)
        {
            this.stateId = stateId;
            this.timeCondition = timeCondition;
        }
        
        /// <summary>
        /// Creates a copy of this skybox state
        /// </summary>
        /// <returns>Deep copy of the skybox state</returns>
        public SkyboxState Clone()
        {
            return new SkyboxState(StateId, TimeCondition)
            {
                SkyTint = SkyTint,
                GroundColor = GroundColor,
                Exposure = Exposure,
                SunSize = SunSize,
                SunSizeConvergence = SunSizeConvergence,
                AtmosphereThickness = AtmosphereThickness,
                SunDirection = SunDirection,
                SkyboxMaterial = SkyboxMaterial,
                FogHeight = FogHeight,
                FogHeightDensity = FogHeightDensity,
                FogStart = FogStart,
                FogEnd = FogEnd
            };
        }
        
        /// <summary>
        /// Validates the skybox state configuration
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(StateId))
            {
                Debug.LogError("SkyboxState: StateId cannot be null or empty");
                return false;
            }
            
            if (Exposure < 0f)
            {
                Debug.LogError("SkyboxState: Exposure cannot be negative");
                return false;
            }
            
            if (SunSize < 0f || SunSize > 1f)
            {
                Debug.LogError("SkyboxState: SunSize must be between 0 and 1");
                return false;
            }
            
            if (SunSizeConvergence < 1f || SunSizeConvergence > 10f)
            {
                Debug.LogError("SkyboxState: SunSizeConvergence must be between 1 and 10");
                return false;
            }
            
            if (AtmosphereThickness < 0f || AtmosphereThickness > 5f)
            {
                Debug.LogError("SkyboxState: AtmosphereThickness must be between 0 and 5");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Gets a human-readable description of this skybox state
        /// </summary>
        /// <returns>Description string</returns>
        public string GetDescription()
        {
            var condition = TimeCondition;
            var timeDesc = condition.Hour switch
            {
                -1 => "Any time",
                0 => "Midnight",
                6 => "Dawn",
                12 => "Noon",
                18 => "Dusk",
                23 => "Late night",
                _ => $"{condition.Hour:00}:00"
            };
            
            var seasonDesc = condition.UseSeason ? $" ({condition.Season})" : "";
            var monthDesc = condition.Month != -1 ? $" Month {condition.Month}" : "";
            
            return $"{StateId}: {timeDesc}{seasonDesc}{monthDesc}";
        }
        
        /// <summary>
        /// Applies this skybox state to the render settings
        /// </summary>
        public void ApplyToRenderSettings()
        {
            if (SkyboxMaterial != null)
            {
                RenderSettings.skybox = SkyboxMaterial;
            }
            
            // Apply procedural skybox properties if using procedural shader
            var skybox = RenderSettings.skybox;
            if (skybox != null && skybox.shader.name.Contains("Procedural"))
            {
                skybox.SetColor("_SkyTint", SkyTint);
                skybox.SetColor("_GroundColor", GroundColor);
                skybox.SetFloat("_Exposure", Exposure);
                skybox.SetFloat("_SunSize", SunSize);
                skybox.SetFloat("_SunSizeConvergence", SunSizeConvergence);
                skybox.SetFloat("_AtmosphereThickness", AtmosphereThickness);
                skybox.SetVector("_SunDirection", SunDirection.normalized);
            }
        }
        
        /// <summary>
        /// Captures current skybox state from render settings
        /// </summary>
        public void CaptureFromRenderSettings()
        {
            SkyboxMaterial = RenderSettings.skybox;
            
            var skybox = RenderSettings.skybox;
            if (skybox != null && skybox.shader.name.Contains("Procedural"))
            {
                SkyTint = skybox.GetColor("_SkyTint");
                GroundColor = skybox.GetColor("_GroundColor");
                Exposure = skybox.GetFloat("_Exposure");
                SunSize = skybox.GetFloat("_SunSize");
                SunSizeConvergence = skybox.GetFloat("_SunSizeConvergence");
                AtmosphereThickness = skybox.GetFloat("_AtmosphereThickness");
                SunDirection = skybox.GetVector("_SunDirection");
            }
        }
        
        /// <summary>
        /// Creates a seasonal skybox state
        /// </summary>
        /// <param name="season">Season for the skybox</param>
        /// <param name="hour">Hour of day</param>
        /// <returns>Seasonal skybox state</returns>
        public static SkyboxState CreateSeasonalState(Season season, int hour = 12)
        {
            var state = new SkyboxState($"Season_{season}_{hour:D2}h", new TimeCondition(hour, -1, -1, -1, TimeType.Hour, true, season));
            
            // Adjust colors and properties based on season
            switch (season)
            {
                case Season.Spring:
                    state.SkyTint = new Color(0.6f, 0.8f, 1f, 1f);
                    state.GroundColor = new Color(0.5f, 0.6f, 0.4f, 1f);
                    state.Exposure = 1.2f;
                    break;
                    
                case Season.Summer:
                    state.SkyTint = new Color(0.4f, 0.7f, 1f, 1f);
                    state.GroundColor = new Color(0.6f, 0.5f, 0.3f, 1f);
                    state.Exposure = 1.4f;
                    break;
                    
                case Season.Autumn:
                    state.SkyTint = new Color(0.7f, 0.6f, 0.5f, 1f);
                    state.GroundColor = new Color(0.4f, 0.3f, 0.2f, 1f);
                    state.Exposure = 1.1f;
                    break;
                    
                case Season.Winter:
                    state.SkyTint = new Color(0.8f, 0.9f, 1f, 1f);
                    state.GroundColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                    state.Exposure = 1.0f;
                    state.AtmosphereThickness = 1.1f;
                    break;
            }
            
            // Adjust for time of day
            if (hour < 6 || hour > 20) // Night
            {
                state.SkyTint = new Color(0.2f, 0.2f, 0.4f, 1f);
                state.GroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
                state.Exposure = 0.5f;
                state.SunSize = 0f;
            }
            else if (hour < 8) // Dawn
            {
                state.SkyTint = new Color(state.SkyTint.r * 1.2f, state.SkyTint.g * 0.8f, state.SkyTint.b * 0.6f, 1f);
                state.Exposure = state.Exposure * 0.8f;
            }
            else if (hour > 18) // Dusk
            {
                state.SkyTint = new Color(state.SkyTint.r * 1.3f, state.SkyTint.g * 0.6f, state.SkyTint.b * 0.4f, 1f);
                state.Exposure = state.Exposure * 0.9f;
            }
            
            return state;
        }
    }
}
