using System;
using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;

namespace GameVisualUpdateByTimeSystem.Visuals.Skyboxes
{
    /// <summary>
    /// Manages skybox changes based on time of day, month, and season
    /// Supports both procedural and material-based skyboxes
    /// </summary>
    public class TimeBasedSkybox : BaseVisualComponent, IInterpolatableVisual<SkyboxState>
    {
        [Header("Skybox Configuration")]
        [SerializeField] private Skybox _skyboxComponent;
        [SerializeField] private Material _skyboxMaterial;
        [SerializeField] private bool _useProceduralSkybox = true;
        
        [Header("Skybox States")]
        [SerializeField] private SkyboxState[] _skyboxStates = new SkyboxState[0];
        
        [Header("Performance")]
        [SerializeField] private float _updateInterval = 0.2f; // Update every 200ms
        
        private SkyboxState _currentState;
        private SkyboxState _targetState;
        private float _transitionProgress;
        private float _lastStateUpdateTime;
        
        // Procedural skybox properties
        private Material _proceduralMaterial;
        
        protected override void OnInitialize()
        {
            // Get or create skybox component
            if (_skyboxComponent == null)
            {
                _skyboxComponent = FindObjectOfType<Skybox>();
                if (_skyboxComponent == null)
                {
                    var camera = FindObjectOfType<Camera>();
                    if (camera != null)
                    {
                        _skyboxComponent = camera.gameObject.AddComponent<Skybox>();
                    }
                }
            }
            
            // Setup procedural material if using procedural skybox
            if (_useProceduralSkybox)
            {
                SetupProceduralSkybox();
            }
            else if (_skyboxMaterial != null)
            {
                RenderSettings.skybox = _skyboxMaterial;
            }
            
            // Validate skybox states
            if (_skyboxStates.Length == 0)
            {
                LogWarning("No skybox states configured. Using default states.");
                CreateDefaultSkyboxStates();
            }
            
            // Initialize with first state
            if (_skyboxStates.Length > 0)
            {
                _currentState = _skyboxStates[0];
                _targetState = _skyboxStates[0];
                ApplySkyboxState(_currentState);
            }
            
            LogInfo($"Initialized with {_skyboxStates.Length} skybox states");
        }
        
        private void SetupProceduralSkybox()
        {
            // Create a simple procedural skybox material
            _proceduralMaterial = new Material(Shader.Find("Skybox/Procedural"));
            RenderSettings.skybox = _proceduralMaterial;
            
            LogInfo("Setup procedural skybox material");
        }
        
        protected override void OnUpdateVisual(ITimeProvider timeProvider, float deltaTime)
        {
            // Update skybox state based on current time
            UpdateSkyboxState(timeProvider);
            
            // Interpolate between current and target states
            if (_transitionProgress < 1f)
            {
                _transitionProgress += deltaTime / _transitionDuration;
                _transitionProgress = Mathf.Clamp01(_transitionProgress);
                
                var interpolatedState = InterpolateSkyboxStates(_currentState, _targetState, _transitionProgress);
                ApplySkyboxState(interpolatedState);
                
                if (_transitionProgress >= 1f)
                {
                    _currentState = _targetState;
                }
            }
        }
        
        private void UpdateSkyboxState(ITimeProvider timeProvider)
        {
            if (Time.time - _lastStateUpdateTime < _updateInterval) return;
            
            var newTargetState = GetSkyboxStateForTime(timeProvider);
            if (newTargetState.StateId != _targetState.StateId)
            {
                _currentState = GetCurrentSkyboxState();
                _targetState = newTargetState;
                _transitionProgress = 0f;
                _lastStateUpdateTime = Time.time;
                
                LogInfo($"Transitioning to skybox state: {_targetState.StateId}");
            }
        }
        
        private SkyboxState GetSkyboxStateForTime(ITimeProvider timeProvider)
        {
            // Find the most appropriate skybox state for current time
            SkyboxState bestState = _skyboxStates[0];
            float bestScore = float.MaxValue;
            
            foreach (var state in _skyboxStates)
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
        
        private float CalculateStateScore(SkyboxState state, ITimeProvider timeProvider)
        {
            float score = 0f;
            var condition = state.TimeCondition;
            
            // Hour-based scoring (most important for skybox)
            if (condition.Hour != -1)
            {
                int hourDiff = Mathf.Abs(timeProvider.CurrentHour - condition.Hour);
                score += hourDiff * 15f; // Weight hour differences heavily
            }
            
            // Season-based scoring
            if (condition.UseSeason)
            {
                var currentSeason = GetSeasonFromTime(timeProvider);
                if (currentSeason != condition.Season)
                {
                    score += 75f; // Heavy penalty for wrong season
                }
            }
            
            // Month-based scoring
            if (condition.Month != -1)
            {
                int monthDiff = Mathf.Abs(timeProvider.CurrentMonth - condition.Month);
                score += monthDiff * 8f;
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
        
        private void ApplySkyboxState(SkyboxState state)
        {
            if (_useProceduralSkybox && _proceduralMaterial != null)
            {
                ApplyProceduralSkyboxState(state);
            }
            else if (_skyboxMaterial != null)
            {
                ApplyMaterialSkyboxState(state);
            }
        }
        
        private void ApplyProceduralSkyboxState(SkyboxState state)
        {
            // Apply procedural skybox properties
            _proceduralMaterial.SetColor("_SkyTint", state.SkyTint);
            _proceduralMaterial.SetColor("_GroundColor", state.GroundColor);
            _proceduralMaterial.SetFloat("_Exposure", state.Exposure);
            _proceduralMaterial.SetFloat("_SunSize", state.SunSize);
            _proceduralMaterial.SetFloat("_SunSizeConvergence", state.SunSizeConvergence);
            _proceduralMaterial.SetFloat("_AtmosphereThickness", state.AtmosphereThickness);
            
            // Set sun direction if available
            if (state.SunDirection != Vector3.zero)
            {
                _proceduralMaterial.SetVector("_SunDirection", state.SunDirection.normalized);
            }
        }
        
        private void ApplyMaterialSkyboxState(SkyboxState state)
        {
            // For material-based skyboxes, we can interpolate between different materials
            // or modify material properties if the shader supports it
            
            if (state.SkyboxMaterial != null)
            {
                RenderSettings.skybox = state.SkyboxMaterial;
            }
            
            // Apply common material properties if they exist
            if (RenderSettings.skybox.HasProperty("_Tint"))
            {
                RenderSettings.skybox.SetColor("_Tint", state.SkyTint);
            }
            
            if (RenderSettings.skybox.HasProperty("_Exposure"))
            {
                RenderSettings.skybox.SetFloat("_Exposure", state.Exposure);
            }
        }
        
        private SkyboxState InterpolateSkyboxStates(SkyboxState from, SkyboxState to, float t)
        {
            var result = new SkyboxState
            {
                stateId = to.StateId,
                timeCondition = to.TimeCondition,
                SkyTint = SmoothInterpolate(from.SkyTint, to.SkyTint, t),
                GroundColor = SmoothInterpolate(from.GroundColor, to.GroundColor, t),
                Exposure = SmoothInterpolate(from.Exposure, to.Exposure, t),
                SunSize = SmoothInterpolate(from.SunSize, to.SunSize, t),
                SunSizeConvergence = SmoothInterpolate(from.SunSizeConvergence, to.SunSizeConvergence, t),
                AtmosphereThickness = SmoothInterpolate(from.AtmosphereThickness, to.AtmosphereThickness, t),
                SunDirection = SmoothInterpolate(from.SunDirection, to.SunDirection, t)
            };
            
            // For material-based skyboxes, we might want to blend between materials
            // This is more complex and would require custom shader support
            result.SkyboxMaterial = t > 0.5f ? to.SkyboxMaterial : from.SkyboxMaterial;
            
            return result;
        }
        
        private SkyboxState GetCurrentSkyboxState()
        {
            var state = new SkyboxState
            {
                stateId = _currentState.StateId,
                timeCondition = _currentState.TimeCondition
            };
            
            // Capture current skybox state
            if (_useProceduralSkybox && _proceduralMaterial != null)
            {
                state.SkyTint = _proceduralMaterial.GetColor("_SkyTint");
                state.GroundColor = _proceduralMaterial.GetColor("_GroundColor");
                state.Exposure = _proceduralMaterial.GetFloat("_Exposure");
                state.SunSize = _proceduralMaterial.GetFloat("_SunSize");
                state.SunSizeConvergence = _proceduralMaterial.GetFloat("_SunSizeConvergence");
                state.AtmosphereThickness = _proceduralMaterial.GetFloat("_AtmosphereThickness");
                state.SunDirection = _proceduralMaterial.GetVector("_SunDirection");
            }
            else
            {
                state.SkyboxMaterial = RenderSettings.skybox;
            }
            
            return state;
        }
        
        private void CreateDefaultSkyboxStates()
        {
            _skyboxStates = new SkyboxState[]
            {
                // Dawn skybox
                new SkyboxState
                {
                    stateId = "Dawn",
                    timeCondition = new TimeCondition(6, -1, -1, -1, TimeType.Hour),
                    SkyTint = new Color(1f, 0.8f, 0.6f, 1f),
                    GroundColor = new Color(0.4f, 0.3f, 0.2f, 1f),
                    Exposure = 0.8f,
                    SunSize = 0.04f,
                    SunSizeConvergence = 5f,
                    AtmosphereThickness = 1f,
                    SunDirection = new Vector3(0.5f, 0.3f, -0.8f).normalized
                },
                
                // Day skybox
                new SkyboxState
                {
                    stateId = "Day",
                    timeCondition = new TimeCondition(12, -1, -1, -1, TimeType.Hour),
                    SkyTint = new Color(0.5f, 0.7f, 1f, 1f),
                    GroundColor = new Color(0.4f, 0.4f, 0.4f, 1f),
                    Exposure = 1.3f,
                    SunSize = 0.04f,
                    SunSizeConvergence = 5f,
                    AtmosphereThickness = 1f,
                    SunDirection = new Vector3(0.3f, 0.9f, -0.3f).normalized
                },
                
                // Dusk skybox
                new SkyboxState
                {
                    stateId = "Dusk",
                    timeCondition = new TimeCondition(19, -1, -1, -1, TimeType.Hour),
                    SkyTint = new Color(1f, 0.5f, 0.3f, 1f),
                    GroundColor = new Color(0.3f, 0.2f, 0.1f, 1f),
                    Exposure = 1.0f,
                    SunSize = 0.06f,
                    SunSizeConvergence = 3f,
                    AtmosphereThickness = 1.2f,
                    SunDirection = new Vector3(0.5f, 0.2f, -0.8f).normalized
                },
                
                // Night skybox
                new SkyboxState
                {
                    stateId = "Night",
                    timeCondition = new TimeCondition(0, -1, -1, -1, TimeType.Hour),
                    SkyTint = new Color(0.2f, 0.2f, 0.4f, 1f),
                    GroundColor = new Color(0.1f, 0.1f, 0.15f, 1f),
                    Exposure = 0.5f,
                    SunSize = 0.0f,
                    SunSizeConvergence = 1f,
                    AtmosphereThickness = 0.8f,
                    SunDirection = new Vector3(-0.5f, -0.3f, -0.8f).normalized
                }
            };
        }
        
        #region IInterpolatableVisual Implementation
        
        public void Interpolate(SkyboxState from, SkyboxState to, float t)
        {
            var interpolatedState = InterpolateSkyboxStates(from, to, t);
            ApplySkyboxState(interpolatedState);
        }
        
        public SkyboxState GetCurrentState()
        {
            return GetCurrentSkyboxState();
        }
        
        public void SetState(SkyboxState state)
        {
            _currentState = state;
            _targetState = state;
            _transitionProgress = 1f;
            ApplySkyboxState(state);
        }
        
        #endregion
        
        #region Editor Helpers
        
        [ContextMenu("Create Default Skybox States")]
        private void EditorCreateDefaultStates()
        {
            CreateDefaultSkyboxStates();
            LogInfo("Created default skybox states");
        }
        
        [ContextMenu("Apply Current State")]
        private void EditorApplyCurrentState()
        {
            if (_currentState.StateId != null)
            {
                ApplySkyboxState(_currentState);
                LogInfo($"Applied skybox state: {_currentState.StateId}");
            }
        }
        
        [ContextMenu("Capture Current Skybox")]
        private void EditorCaptureCurrentSkybox()
        {
            var capturedState = GetCurrentSkyboxState();
            capturedState.stateId = "Captured_" + DateTime.Now.ToString("HHmm");
            LogInfo($"Captured current skybox as: {capturedState.StateId}");
        }
        
        #endregion
    }
}
