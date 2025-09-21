using System;
using UnityEngine;
using UnityEngine.Rendering;
using GameVisualUpdateByTimeSystem.Core.Interfaces;

namespace GameVisualUpdateByTimeSystem.Visuals.Lighting
{
    /// <summary>
    /// Represents a complete lighting state for time-based lighting transitions
    /// </summary>
    [Serializable]
    public class LightingState : IVisualState
    {
        [Header("State Configuration")]
        public string stateId;
        public TimeCondition timeCondition;
        
        [Header("Main Light")]
        public Color MainLightColor = Color.white;
        public float MainLightIntensity = 1f;
        public Vector3 MainLightRotation = new Vector3(50f, -30f, 0f);
        public LightShadows MainLightShadows = LightShadows.Soft;
        
        [Header("Environment Lighting")]
        public Color AmbientSkyColor = new Color(0.5f, 0.7f, 1f, 1f);
        public Color AmbientEquatorColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        public Color AmbientGroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        public float AmbientIntensity = 1f;
        
        [Header("Fog Settings")]
        public bool EnableFog = false;
        public Color FogColor = Color.white;
        public FogMode FogMode = FogMode.Linear;
        public float FogStartDistance = 10f;
        public float FogEndDistance = 50f;
        public float FogDensity = 0.01f;
        
        [Header("Additional Lights")]
        public LightState[] AdditionalLights = new LightState[0];
        
        public string StateId => StateId;
        public TimeCondition TimeCondition => TimeCondition;
        
        public LightingState()
        {
            stateId = "Default";
            timeCondition = new TimeCondition();
        }
        
        public LightingState(string stateId, TimeCondition timeCondition)
        {
            this.stateId = stateId;
            this.timeCondition = timeCondition;
        }
        
        /// <summary>
        /// Creates a copy of this lighting state
        /// </summary>
        /// <returns>Deep copy of the lighting state</returns>
        public LightingState Clone()
        {
            var clone = new LightingState(StateId, TimeCondition)
            {
                MainLightColor = MainLightColor,
                MainLightIntensity = MainLightIntensity,
                MainLightRotation = MainLightRotation,
                MainLightShadows = MainLightShadows,
                AmbientSkyColor = AmbientSkyColor,
                AmbientEquatorColor = AmbientEquatorColor,
                AmbientGroundColor = AmbientGroundColor,
                AmbientIntensity = AmbientIntensity,
                EnableFog = EnableFog,
                FogColor = FogColor,
                FogMode = FogMode,
                FogStartDistance = FogStartDistance,
                FogEndDistance = FogEndDistance,
                FogDensity = FogDensity
            };
            
            // Clone additional lights
            clone.AdditionalLights = new LightState[AdditionalLights.Length];
            for (int i = 0; i < AdditionalLights.Length; i++)
            {
                clone.AdditionalLights[i] = AdditionalLights[i].Clone();
            }
            
            return clone;
        }
        
        /// <summary>
        /// Validates the lighting state configuration
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(StateId))
            {
                Debug.LogError("LightingState: StateId cannot be null or empty");
                return false;
            }
            
            if (MainLightIntensity < 0f)
            {
                Debug.LogError("LightingState: MainLightIntensity cannot be negative");
                return false;
            }
            
            if (AmbientIntensity < 0f)
            {
                Debug.LogError("LightingState: AmbientIntensity cannot be negative");
                return false;
            }
            
            if (EnableFog)
            {
                if (FogMode == FogMode.Linear && FogStartDistance >= FogEndDistance)
                {
                    Debug.LogError("LightingState: FogStartDistance must be less than FogEndDistance for linear fog");
                    return false;
                }
                
                if (FogDensity < 0f)
                {
                    Debug.LogError("LightingState: FogDensity cannot be negative");
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Gets a human-readable description of this lighting state
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
    }
    
    /// <summary>
    /// Represents a single light configuration
    /// </summary>
    [Serializable]
    public class LightState
    {
        [Header("Light Properties")]
        public Color Color = Color.white;
        public float Intensity = 1f;
        public float Range = 10f;
        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public LightType Type = LightType.Point;
        public LightShadows Shadows = LightShadows.Soft;
        
        [Header("Advanced")]
        public float SpotAngle = 30f;
        public float InnerSpotAngle = 21.8f;
        public bool UseColorTemperature = false;
        public float ColorTemperature = 6500f;
        
        public LightState()
        {
        }
        
        public LightState(Color color, float intensity, float range)
        {
            Color = color;
            Intensity = intensity;
            Range = range;
        }
        
        /// <summary>
        /// Creates a copy of this light state
        /// </summary>
        /// <returns>Deep copy of the light state</returns>
        public LightState Clone()
        {
            return new LightState
            {
                Color = Color,
                Intensity = Intensity,
                Range = Range,
                Position = Position,
                Rotation = Rotation,
                Type = Type,
                Shadows = Shadows,
                SpotAngle = SpotAngle,
                InnerSpotAngle = InnerSpotAngle,
                UseColorTemperature = UseColorTemperature,
                ColorTemperature = ColorTemperature
            };
        }
        
        /// <summary>
        /// Applies this light state to a Unity Light component
        /// </summary>
        /// <param name="light">Light component to apply state to</param>
        public void ApplyToLight(Light light)
        {
            if (light == null) return;
            
            light.color = Color;
            light.intensity = Intensity;
            light.range = Range;
            light.type = Type;
            light.shadows = Shadows;
            
            if (UseColorTemperature)
            {
                light.useColorTemperature = true;
                light.colorTemperature = ColorTemperature;
            }
            
            if (Type == LightType.Spot)
            {
                light.spotAngle = SpotAngle;
                light.innerSpotAngle = InnerSpotAngle;
            }
            
            light.transform.position = Position;
            light.transform.rotation = Quaternion.Euler(Rotation);
        }
        
        /// <summary>
        /// Captures current state from a Unity Light component
        /// </summary>
        /// <param name="light">Light component to capture from</param>
        public void CaptureFromLight(Light light)
        {
            if (light == null) return;
            
            Color = light.color;
            Intensity = light.intensity;
            Range = light.range;
            Type = light.type;
            Shadows = light.shadows;
            Position = light.transform.position;
            Rotation = light.transform.rotation.eulerAngles;
            
            if (light.useColorTemperature)
            {
                UseColorTemperature = true;
                ColorTemperature = light.colorTemperature;
            }
            
            if (Type == LightType.Spot)
            {
                SpotAngle = light.spotAngle;
                InnerSpotAngle = light.innerSpotAngle;
            }
        }
    }
}
