using System;
using System.Collections.Generic;
using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;

namespace GameVisualUpdateByTimeSystem.Visuals.Materials
{
    /// <summary>
    /// Manages material property changes based on time
    /// Supports multiple materials and property types
    /// </summary>
    public class TimeBasedMaterialProperties : BaseVisualComponent, IInterpolatableVisual<MaterialState>
    {
        [Header("Material Configuration")]
        [SerializeField] private Material[] _materials = new Material[0];
        [SerializeField] private Renderer[] _renderers = new Renderer[0];
        [SerializeField] private bool _useSharedMaterials = false;
        [SerializeField] private bool _createMaterialInstances = true;
        
        [Header("Material States")]
        [SerializeField] private MaterialState[] _materialStates = new MaterialState[0];
        
        [Header("Performance")]
        [SerializeField] private float _updateInterval = 0.1f;
        
        private MaterialState _currentState;
        private MaterialState _targetState;
        private float _transitionProgress;
        private float _lastStateUpdateTime;
        
        // Material instances for non-destructive editing
        private Material[] _materialInstances;
        private Dictionary<Material, Material> _originalToInstanceMap = new Dictionary<Material, Material>();
        
        protected override void OnInitialize()
        {
            // Auto-find renderers if not assigned
            if (_renderers.Length == 0)
            {
                _renderers = GetComponentsInChildren<Renderer>();
                if (_renderers.Length == 0)
                {
                    LogWarning("No renderers found. Component may not function correctly.");
                }
            }
            
            // Collect materials from renderers
            CollectMaterials();
            
            // Create material instances if needed
            if (_createMaterialInstances)
            {
                CreateMaterialInstances();
            }
            
            // Validate material states
            if (_materialStates.Length == 0)
            {
                LogWarning("No material states configured. Using default states.");
                CreateDefaultMaterialStates();
            }
            
            // Initialize with first state
            if (_materialStates.Length > 0)
            {
                _currentState = _materialStates[0];
                _targetState = _materialStates[0];
                ApplyMaterialState(_currentState);
            }
            
            LogInfo($"Initialized with {_materials.Length} materials and {_materialStates.Length} states");
        }
        
        private void CollectMaterials()
        {
            var materialSet = new HashSet<Material>();
            
            foreach (var renderer in _renderers)
            {
                if (renderer == null) continue;
                
                var materials = _useSharedMaterials ? renderer.sharedMaterials : renderer.materials;
                foreach (var material in materials)
                {
                    if (material != null)
                    {
                        materialSet.Add(material);
                    }
                }
            }
            
            _materials = new Material[materialSet.Count];
            materialSet.CopyTo(_materials);
        }
        
        private void CreateMaterialInstances()
        {
            _materialInstances = new Material[_materials.Length];
            _originalToInstanceMap.Clear();
            
            for (int i = 0; i < _materials.Length; i++)
            {
                var original = _materials[i];
                var instance = new Material(original);
                instance.name = original.name + "_Instance";
                
                _materialInstances[i] = instance;
                _originalToInstanceMap[original] = instance;
                
                // Update renderers to use instances
                foreach (var renderer in _renderers)
                {
                    if (renderer == null) continue;
                    
                    var materials = renderer.materials;
                    for (int j = 0; j < materials.Length; j++)
                    {
                        if (materials[j] == original)
                        {
                            materials[j] = instance;
                        }
                    }
                    renderer.materials = materials;
                }
            }
            
            LogInfo($"Created {_materialInstances.Length} material instances");
        }
        
        protected override void OnUpdateVisual(ITimeProvider timeProvider, float deltaTime)
        {
            // Update material state based on current time
            UpdateMaterialState(timeProvider);
            
            // Interpolate between current and target states
            if (_transitionProgress < 1f)
            {
                _transitionProgress += deltaTime / _transitionDuration;
                _transitionProgress = Mathf.Clamp01(_transitionProgress);
                
                var interpolatedState = InterpolateMaterialStates(_currentState, _targetState, _transitionProgress);
                ApplyMaterialState(interpolatedState);
                
                if (_transitionProgress >= 1f)
                {
                    _currentState = _targetState;
                }
            }
        }
        
        private void UpdateMaterialState(ITimeProvider timeProvider)
        {
            if (Time.time - _lastStateUpdateTime < _updateInterval) return;
            
            var newTargetState = GetMaterialStateForTime(timeProvider);
            if (newTargetState.StateId != _targetState.StateId)
            {
                _currentState = GetCurrentMaterialState();
                _targetState = newTargetState;
                _transitionProgress = 0f;
                _lastStateUpdateTime = Time.time;
                
                LogInfo($"Transitioning to material state: {_targetState.StateId}");
            }
        }
        
        private MaterialState GetMaterialStateForTime(ITimeProvider timeProvider)
        {
            // Find the most appropriate material state for current time
            MaterialState bestState = _materialStates[0];
            float bestScore = float.MaxValue;
            
            foreach (var state in _materialStates)
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
        
        private float CalculateStateScore(MaterialState state, ITimeProvider timeProvider)
        {
            float score = 0f;
            var condition = state.TimeCondition;
            
            // Hour-based scoring
            if (condition.Hour != -1)
            {
                int hourDiff = Mathf.Abs(timeProvider.CurrentHour - condition.Hour);
                score += hourDiff * 12f;
            }
            
            // Season-based scoring
            if (condition.UseSeason)
            {
                var currentSeason = GetSeasonFromTime(timeProvider);
                if (currentSeason != condition.Season)
                {
                    score += 60f;
                }
            }
            
            // Month-based scoring
            if (condition.Month != -1)
            {
                int monthDiff = Mathf.Abs(timeProvider.CurrentMonth - condition.Month);
                score += monthDiff * 6f;
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
        
        private void ApplyMaterialState(MaterialState state)
        {
            var materialsToUpdate = _createMaterialInstances ? _materialInstances : _materials;
            
            foreach (var material in materialsToUpdate)
            {
                if (material == null) continue;
                
                ApplyMaterialProperties(material, state);
            }
        }
        
        private void ApplyMaterialProperties(Material material, MaterialState state)
        {
            // Apply color properties
            foreach (var colorProp in state.ColorProperties)
            {
                if (material.HasProperty(colorProp.PropertyName))
                {
                    material.SetColor(colorProp.PropertyName, colorProp.Value);
                }
            }
            
            // Apply float properties
            foreach (var floatProp in state.FloatProperties)
            {
                if (material.HasProperty(floatProp.PropertyName))
                {
                    material.SetFloat(floatProp.PropertyName, floatProp.Value);
                }
            }
            
            // Apply vector properties
            foreach (var vectorProp in state.VectorProperties)
            {
                if (material.HasProperty(vectorProp.PropertyName))
                {
                    material.SetVector(vectorProp.PropertyName, vectorProp.Value);
                }
            }
            
            // Apply texture properties
            foreach (var textureProp in state.TextureProperties)
            {
                if (material.HasProperty(textureProp.PropertyName))
                {
                    material.SetTexture(textureProp.PropertyName, textureProp.Value);
                }
            }
        }
        
        private MaterialState InterpolateMaterialStates(MaterialState from, MaterialState to, float t)
        {
            var result = new MaterialState
            {
                stateId = to.StateId,
                timeCondition = to.TimeCondition
            };
            
            // Interpolate color properties
            result.ColorProperties = InterpolateColorProperties(from.ColorProperties, to.ColorProperties, t);
            
            // Interpolate float properties
            result.FloatProperties = InterpolateFloatProperties(from.FloatProperties, to.FloatProperties, t);
            
            // Interpolate vector properties
            result.VectorProperties = InterpolateVectorProperties(from.VectorProperties, to.VectorProperties, t);
            
            // For texture properties, just use the target (no interpolation)
            result.TextureProperties = to.TextureProperties;
            
            return result;
        }
        
        private MaterialColorProperty[] InterpolateColorProperties(MaterialColorProperty[] from, MaterialColorProperty[] to, float t)
        {
            var result = new MaterialColorProperty[to.Length];
            
            for (int i = 0; i < to.Length; i++)
            {
                result[i] = new MaterialColorProperty
                {
                    PropertyName = to[i].PropertyName,
                    Value = SmoothInterpolate(
                        FindColorPropertyValue(from, to[i].PropertyName, to[i].Value),
                        to[i].Value,
                        t
                    )
                };
            }
            
            return result;
        }
        
        private MaterialFloatProperty[] InterpolateFloatProperties(MaterialFloatProperty[] from, MaterialFloatProperty[] to, float t)
        {
            var result = new MaterialFloatProperty[to.Length];
            
            for (int i = 0; i < to.Length; i++)
            {
                result[i] = new MaterialFloatProperty
                {
                    PropertyName = to[i].PropertyName,
                    Value = SmoothInterpolate(
                        FindFloatPropertyValue(from, to[i].PropertyName, to[i].Value),
                        to[i].Value,
                        t
                    )
                };
            }
            
            return result;
        }
        
        private MaterialVectorProperty[] InterpolateVectorProperties(MaterialVectorProperty[] from, MaterialVectorProperty[] to, float t)
        {
            var result = new MaterialVectorProperty[to.Length];
            
            for (int i = 0; i < to.Length; i++)
            {
                result[i] = new MaterialVectorProperty
                {
                    PropertyName = to[i].PropertyName,
                    Value = SmoothInterpolate(
                        FindVectorPropertyValue(from, to[i].PropertyName, to[i].Value),
                        to[i].Value,
                        t)
                };
            }
            
            return result;
        }
        
        private Color FindColorPropertyValue(MaterialColorProperty[] properties, string name, Color defaultValue)
        {
            foreach (var prop in properties)
            {
                if (prop.PropertyName == name)
                    return prop.Value;
            }
            return defaultValue;
        }
        
        private float FindFloatPropertyValue(MaterialFloatProperty[] properties, string name, float defaultValue)
        {
            foreach (var prop in properties)
            {
                if (prop.PropertyName == name)
                    return prop.Value;
            }
            return defaultValue;
        }
        
        private Vector4 FindVectorPropertyValue(MaterialVectorProperty[] properties, string name, Vector4 defaultValue)
        {
            foreach (var prop in properties)
            {
                if (prop.PropertyName == name)
                    return prop.Value;
            }
            return defaultValue;
        }
        
        private MaterialState GetCurrentMaterialState()
        {
            var state = new MaterialState
            {
                stateId = _currentState.StateId,
                timeCondition = _currentState.TimeCondition
            };
            
            // Capture current material properties
            var materialsToCheck = _createMaterialInstances ? _materialInstances : _materials;
            if (materialsToCheck.Length > 0 && materialsToCheck[0] != null)
            {
                var material = materialsToCheck[0]; // Use first material as reference
                
                // Capture common properties if they exist
                if (material.HasProperty("_Color"))
                {
                    state.ColorProperties = new MaterialColorProperty[]
                    {
                        new MaterialColorProperty { PropertyName = "_Color", Value = material.GetColor("_Color") }
                    };
                }
                
                if (material.HasProperty("_Metallic"))
                {
                    state.FloatProperties = new MaterialFloatProperty[]
                    {
                        new MaterialFloatProperty { PropertyName = "_Metallic", Value = material.GetFloat("_Metallic") },
                        new MaterialFloatProperty { PropertyName = "_Smoothness", Value = material.GetFloat("_Smoothness") }
                    };
                }
            }
            
            return state;
        }
        
        private void CreateDefaultMaterialStates()
        {
            _materialStates = new MaterialState[]
            {
                // Day state
                new MaterialState
                {
                    stateId = "Day",
                    timeCondition = new TimeCondition(12, -1, -1, -1, TimeType.Hour),
                    ColorProperties = new MaterialColorProperty[]
                    {
                        new MaterialColorProperty { PropertyName = "_Color", Value = Color.white },
                        new MaterialColorProperty { PropertyName = "_EmissionColor", Value = Color.black }
                    },
                    FloatProperties = new MaterialFloatProperty[]
                    {
                        new MaterialFloatProperty { PropertyName = "_Metallic", Value = 0.2f },
                        new MaterialFloatProperty { PropertyName = "_Smoothness", Value = 0.8f }
                    }
                },
                
                // Night state
                new MaterialState
                {
                    stateId = "Night",
                    timeCondition = new TimeCondition(0, -1, -1, -1, TimeType.Hour),
                    ColorProperties = new MaterialColorProperty[]
                    {
                        new MaterialColorProperty { PropertyName = "_Color", Value = new Color(0.8f, 0.8f, 1f, 1f) },
                        new MaterialColorProperty { PropertyName = "_EmissionColor", Value = new Color(0.1f, 0.1f, 0.2f, 1f) }
                    },
                    FloatProperties = new MaterialFloatProperty[]
                    {
                        new MaterialFloatProperty { PropertyName = "_Metallic", Value = 0.3f },
                        new MaterialFloatProperty { PropertyName = "_Smoothness", Value = 0.6f }
                    }
                }
            };
        }
        
        #region IInterpolatableVisual Implementation
        
        public void Interpolate(MaterialState from, MaterialState to, float t)
        {
            var interpolatedState = InterpolateMaterialStates(from, to, t);
            ApplyMaterialState(interpolatedState);
        }
        
        public MaterialState GetCurrentState()
        {
            return GetCurrentMaterialState();
        }
        
        public void SetState(MaterialState state)
        {
            _currentState = state;
            _targetState = state;
            _transitionProgress = 1f;
            ApplyMaterialState(state);
        }
        
        #endregion
        
        #region Editor Helpers
        
        [ContextMenu("Create Default Material States")]
        private void EditorCreateDefaultStates()
        {
            CreateDefaultMaterialStates();
            LogInfo("Created default material states");
        }
        
        [ContextMenu("Apply Current State")]
        private void EditorApplyCurrentState()
        {
            if (_currentState.StateId != null)
            {
                ApplyMaterialState(_currentState);
                LogInfo($"Applied material state: {_currentState.StateId}");
            }
        }
        
        [ContextMenu("Capture Current Materials")]
        private void EditorCaptureCurrentMaterials()
        {
            CollectMaterials();
            LogInfo($"Captured {_materials.Length} materials");
        }
        
        #endregion
        
        protected override void OnCleanup()
        {
            // Restore original materials if we created instances
            if (_createMaterialInstances && _originalToInstanceMap.Count > 0)
            {
                foreach (var renderer in _renderers)
                {
                    if (renderer == null) continue;
                    
                    var materials = renderer.materials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        foreach (var kvp in _originalToInstanceMap)
                        {
                            if (materials[i] == kvp.Value)
                            {
                                materials[i] = kvp.Key;
                                break;
                            }
                        }
                    }
                    renderer.materials = materials;
                }
                
                // Destroy material instances
                foreach (var instance in _materialInstances)
                {
                    if (instance != null)
                    {
                        DestroyImmediate(instance);
                    }
                }
            }
        }
    }
}
