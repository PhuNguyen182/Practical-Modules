using System;
using UnityEngine;
using UnityEngine.Rendering;
using GameVisualUpdateByTimeSystem.Core.Interfaces;

namespace GameVisualUpdateByTimeSystem.Visuals.Lighting
{
    /// <summary>
    /// Manages lighting changes based on time of day, month, and season
    /// Supports multiple light sources and environment lighting
    /// </summary>
    public class TimeBasedLighting : BaseVisualComponent, IInterpolatableVisual<LightingState>
    {
        [Header("Lighting Configuration")]
        [SerializeField] private Light _mainLight;
        [SerializeField] private Light[] _additionalLights = new Light[0];
        [SerializeField] private bool _controlEnvironmentLighting = true;
        [SerializeField] private bool _controlFog = true;
        
        [Header("Lighting States")]
        [SerializeField] private LightingState[] _lightingStates = new LightingState[0];
        
        [Header("Performance")]
        [SerializeField] private float _updateInterval = 0.1f; // Update every 100ms
        
        private LightingState _currentState;
        private LightingState _targetState;
        private float _transitionProgress;
        private float _lastStateUpdateTime;
        
        protected override void OnInitialize()
        {
            // Auto-find main light if not assigned
            if (_mainLight == null)
            {
                _mainLight = FindObjectOfType<Light>();
                if (_mainLight == null)
                {
                    LogWarning("No main light found. Creating directional light.");
                    var lightGO = new GameObject("TimeBasedLight");
                    _mainLight = lightGO.AddComponent<Light>();
                    _mainLight.type = LightType.Directional;
                }
            }
            
            // Validate lighting states
            if (_lightingStates.Length == 0)
            {
                LogWarning("No lighting states configured. Using default states.");
                CreateDefaultLightingStates();
            }
            
            // Initialize with first state
            if (_lightingStates.Length > 0)
            {
                _currentState = _lightingStates[0];
                _targetState = _lightingStates[0];
                ApplyLightingState(_currentState);
            }
            
            LogInfo($"Initialized with {_lightingStates.Length} lighting states");
        }
        
        protected override void OnUpdateVisual(ITimeProvider timeProvider, float deltaTime)
        {
            // Update lighting state based on current time
            UpdateLightingState(timeProvider);
            
            // Interpolate between current and target states
            if (_transitionProgress < 1f)
            {
                _transitionProgress += deltaTime / _transitionDuration;
                _transitionProgress = Mathf.Clamp01(_transitionProgress);
                
                var interpolatedState = InterpolateLightingStates(_currentState, _targetState, _transitionProgress);
                ApplyLightingState(interpolatedState);
                
                if (_transitionProgress >= 1f)
                {
                    _currentState = _targetState;
                }
            }
        }
        
        private void UpdateLightingState(ITimeProvider timeProvider)
        {
            if (Time.time - _lastStateUpdateTime < _updateInterval) return;
            
            var newTargetState = GetLightingStateForTime(timeProvider);
            if (newTargetState.StateId != _targetState.StateId)
            {
                _currentState = GetCurrentLightingState();
                _targetState = newTargetState;
                _transitionProgress = 0f;
                _lastStateUpdateTime = Time.time;
                
                LogInfo($"Transitioning to lighting state: {_targetState.StateId}");
            }
        }
        
        private LightingState GetLightingStateForTime(ITimeProvider timeProvider)
        {
            // Find the most appropriate lighting state for current time
            LightingState bestState = _lightingStates[0];
            float bestScore = float.MaxValue;
            
            foreach (var state in _lightingStates)
            {
                float score = CalculateStateScore(state, timeProvider);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestState = state;
                }
            }
            
            return bestState;
        }
        
        private float CalculateStateScore(LightingState state, ITimeProvider timeProvider)
        {
            float score = 0f;
            var condition = state.TimeCondition;
            
            // Hour-based scoring (most important for lighting)
            if (condition.Hour != -1)
            {
                int hourDiff = Mathf.Abs(timeProvider.CurrentHour - condition.Hour);
                score += hourDiff * 10f; // Weight hour differences heavily
            }
            
            // Season-based scoring
            if (condition.UseSeason)
            {
                var currentSeason = GetSeasonFromTime(timeProvider);
                if (currentSeason != condition.Season)
                {
                    score += 50f; // Heavy penalty for wrong season
                }
            }
            
            // Month-based scoring
            if (condition.Month != -1)
            {
                int monthDiff = Mathf.Abs(timeProvider.CurrentMonth - condition.Month);
                score += monthDiff * 5f;
            }
            
            return score;
        }
        
        private Season GetSeasonFromTime(ITimeProvider timeProvider)
        {
            return timeProvider.CurrentMonth switch
            {
                3 or 4 or 5 => Season.Spring,
                6 or 7 or 8 => Season.Summer,
                9 or 10 or 11 => Season.Autumn,
                _ => Season.Winter
            };
        }
        
        private void ApplyLightingState(LightingState state)
        {
            // Apply main light properties
            if (_mainLight != null)
            {
                _mainLight.color = state.MainLightColor;
                _mainLight.intensity = state.MainLightIntensity;
                _mainLight.transform.rotation = Quaternion.Euler(state.MainLightRotation);
                
                if (state.MainLightShadows != LightShadows.None)
                {
                    _mainLight.shadows = state.MainLightShadows;
                }
            }
            
            // Apply additional lights
            for (int i = 0; i < _additionalLights.Length && i < state.AdditionalLights.Length; i++)
            {
                var light = _additionalLights[i];
                var lightState = state.AdditionalLights[i];
                
                if (light != null)
                {
                    light.color = lightState.Color;
                    light.intensity = lightState.Intensity;
                    light.range = lightState.Range;
                    
                    if (lightState.Position != Vector3.zero)
                    {
                        light.transform.position = lightState.Position;
                    }
                    
                    if (lightState.Rotation != Vector3.zero)
                    {
                        light.transform.rotation = Quaternion.Euler(lightState.Rotation);
                    }
                }
            }
            
            // Apply environment lighting
            if (_controlEnvironmentLighting)
            {
                RenderSettings.ambientMode = AmbientMode.Trilight;
                RenderSettings.ambientSkyColor = state.AmbientSkyColor;
                RenderSettings.ambientEquatorColor = state.AmbientEquatorColor;
                RenderSettings.ambientGroundColor = state.AmbientGroundColor;
                RenderSettings.ambientIntensity = state.AmbientIntensity;
            }
            
            // Apply fog settings
            if (_controlFog)
            {
                RenderSettings.fog = state.EnableFog;
                if (state.EnableFog)
                {
                    RenderSettings.fogColor = state.FogColor;
                    RenderSettings.fogMode = state.FogMode;
                    RenderSettings.fogStartDistance = state.FogStartDistance;
                    RenderSettings.fogEndDistance = state.FogEndDistance;
                    RenderSettings.fogDensity = state.FogDensity;
                }
            }
        }
        
        private LightingState InterpolateLightingStates(LightingState from, LightingState to, float t)
        {
            var result = new LightingState
            {
                stateId = to.StateId,
                timeCondition = to.TimeCondition,
                MainLightColor = SmoothInterpolate(from.MainLightColor, to.MainLightColor, t),
                MainLightIntensity = SmoothInterpolate(from.MainLightIntensity, to.MainLightIntensity, t),
                MainLightRotation = SmoothInterpolate(from.MainLightRotation, to.MainLightRotation, t),
                AmbientSkyColor = SmoothInterpolate(from.AmbientSkyColor, to.AmbientSkyColor, t),
                AmbientEquatorColor = SmoothInterpolate(from.AmbientEquatorColor, to.AmbientEquatorColor, t),
                AmbientGroundColor = SmoothInterpolate(from.AmbientGroundColor, to.AmbientGroundColor, t),
                AmbientIntensity = SmoothInterpolate(from.AmbientIntensity, to.AmbientIntensity, t),
                EnableFog = to.EnableFog, // Fog enable/disable is not interpolated
                FogColor = SmoothInterpolate(from.FogColor, to.FogColor, t),
                FogStartDistance = SmoothInterpolate(from.FogStartDistance, to.FogStartDistance, t),
                FogEndDistance = SmoothInterpolate(from.FogEndDistance, to.FogEndDistance, t),
                FogDensity = SmoothInterpolate(from.FogDensity, to.FogDensity, t)
            };
            
            // Interpolate additional lights
            int maxLights = Mathf.Max(from.AdditionalLights.Length, to.AdditionalLights.Length);
            result.AdditionalLights = new LightState[maxLights];
            
            for (int i = 0; i < maxLights; i++)
            {
                var fromLight = i < from.AdditionalLights.Length ? from.AdditionalLights[i] : new LightState();
                var toLight = i < to.AdditionalLights.Length ? to.AdditionalLights[i] : new LightState();
                
                result.AdditionalLights[i] = new LightState
                {
                    Color = SmoothInterpolate(fromLight.Color, toLight.Color, t),
                    Intensity = SmoothInterpolate(fromLight.Intensity, toLight.Intensity, t),
                    Range = SmoothInterpolate(fromLight.Range, toLight.Range, t),
                    Position = SmoothInterpolate(fromLight.Position, toLight.Position, t),
                    Rotation = SmoothInterpolate(fromLight.Rotation, toLight.Rotation, t)
                };
            }
            
            return result;
        }
        
        private LightingState GetCurrentLightingState()
        {
            var state = new LightingState
            {
                stateId = _currentState.StateId,
                timeCondition = _currentState.TimeCondition
            };
            
            // Capture current lighting state
            if (_mainLight != null)
            {
                state.MainLightColor = _mainLight.color;
                state.MainLightIntensity = _mainLight.intensity;
                state.MainLightRotation = _mainLight.transform.rotation.eulerAngles;
                state.MainLightShadows = _mainLight.shadows;
            }
            
            // Capture environment lighting
            state.AmbientSkyColor = RenderSettings.ambientSkyColor;
            state.AmbientEquatorColor = RenderSettings.ambientEquatorColor;
            state.AmbientGroundColor = RenderSettings.ambientGroundColor;
            state.AmbientIntensity = RenderSettings.ambientIntensity;
            
            // Capture fog settings
            state.EnableFog = RenderSettings.fog;
            state.FogColor = RenderSettings.fogColor;
            state.FogMode = RenderSettings.fogMode;
            state.FogStartDistance = RenderSettings.fogStartDistance;
            state.FogEndDistance = RenderSettings.fogEndDistance;
            state.FogDensity = RenderSettings.fogDensity;
            
            return state;
        }
        
        private void CreateDefaultLightingStates()
        {
            _lightingStates = new LightingState[]
            {
                // Dawn (5-7 AM)
                new LightingState
                {
                    stateId = "Dawn",
                    timeCondition = new TimeCondition(6, -1, -1, -1, TimeType.Hour),
                    MainLightColor = new Color(1f, 0.8f, 0.6f, 1f),
                    MainLightIntensity = 0.5f,
                    MainLightRotation = new Vector3(10f, -30f, 0f),
                    AmbientSkyColor = new Color(0.5f, 0.4f, 0.3f, 1f),
                    AmbientEquatorColor = new Color(0.3f, 0.25f, 0.2f, 1f),
                    AmbientGroundColor = new Color(0.2f, 0.15f, 0.1f, 1f),
                    AmbientIntensity = 0.3f,
                    EnableFog = true,
                    FogColor = new Color(0.8f, 0.7f, 0.6f, 1f),
                    FogMode = FogMode.Linear,
                    FogStartDistance = 10f,
                    FogEndDistance = 50f,
                    FogDensity = 0.01f,
                    AdditionalLights = new LightState[0]
                },
                
                // Day (7 AM - 6 PM)
                new LightingState
                {
                    stateId = "Day",
                    timeCondition = new TimeCondition(12, -1, -1, -1, TimeType.Hour),
                    MainLightColor = new Color(1f, 1f, 0.95f, 1f),
                    MainLightIntensity = 1.2f,
                    MainLightRotation = new Vector3(50f, -30f, 0f),
                    AmbientSkyColor = new Color(0.5f, 0.7f, 1f, 1f),
                    AmbientEquatorColor = new Color(0.4f, 0.4f, 0.4f, 1f),
                    AmbientGroundColor = new Color(0.2f, 0.2f, 0.2f, 1f),
                    AmbientIntensity = 0.6f,
                    EnableFog = false,
                    AdditionalLights = new LightState[0]
                },
                
                // Dusk (6-8 PM)
                new LightingState
                {
                    stateId = "Dusk",
                    timeCondition = new TimeCondition(19, -1, -1, -1, TimeType.Hour),
                    MainLightColor = new Color(1f, 0.6f, 0.3f, 1f),
                    MainLightIntensity = 0.8f,
                    MainLightRotation = new Vector3(10f, -30f, 0f),
                    AmbientSkyColor = new Color(0.8f, 0.4f, 0.2f, 1f),
                    AmbientEquatorColor = new Color(0.4f, 0.2f, 0.1f, 1f),
                    AmbientGroundColor = new Color(0.2f, 0.1f, 0.05f, 1f),
                    AmbientIntensity = 0.4f,
                    EnableFog = true,
                    FogColor = new Color(0.8f, 0.5f, 0.3f, 1f),
                    FogMode = FogMode.Linear,
                    FogStartDistance = 15f,
                    FogEndDistance = 60f,
                    FogDensity = 0.015f,
                    AdditionalLights = new LightState[0]
                },
                
                // Night (8 PM - 5 AM)
                new LightingState
                {
                    stateId = "Night",
                    timeCondition = new TimeCondition(0, -1, -1, -1, TimeType.Hour),
                    MainLightColor = new Color(0.4f, 0.5f, 0.8f, 1f),
                    MainLightIntensity = 0.1f,
                    MainLightRotation = new Vector3(-30f, -30f, 0f),
                    AmbientSkyColor = new Color(0.1f, 0.1f, 0.2f, 1f),
                    AmbientEquatorColor = new Color(0.05f, 0.05f, 0.1f, 1f),
                    AmbientGroundColor = new Color(0.02f, 0.02f, 0.05f, 1f),
                    AmbientIntensity = 0.1f,
                    EnableFog = true,
                    FogColor = new Color(0.1f, 0.1f, 0.2f, 1f),
                    FogMode = FogMode.ExponentialSquared,
                    FogStartDistance = 0f,
                    FogEndDistance = 0f,
                    FogDensity = 0.02f,
                    AdditionalLights = new LightState[0]
                }
            };
        }
        
        #region IInterpolatableVisual Implementation
        
        public void Interpolate(LightingState from, LightingState to, float t)
        {
            var interpolatedState = InterpolateLightingStates(from, to, t);
            ApplyLightingState(interpolatedState);
        }
        
        public LightingState GetCurrentState()
        {
            return GetCurrentLightingState();
        }
        
        public void SetState(LightingState state)
        {
            _currentState = state;
            _targetState = state;
            _transitionProgress = 1f;
            ApplyLightingState(state);
        }
        
        #endregion
        
        #region Editor Helpers
        
        [ContextMenu("Create Default Lighting States")]
        private void EditorCreateDefaultStates()
        {
            CreateDefaultLightingStates();
            LogInfo("Created default lighting states");
        }
        
        [ContextMenu("Apply Current State")]
        private void EditorApplyCurrentState()
        {
            if (_currentState.StateId != null)
            {
                ApplyLightingState(_currentState);
                LogInfo($"Applied lighting state: {_currentState.StateId}");
            }
        }
        
        #endregion
    }
}
