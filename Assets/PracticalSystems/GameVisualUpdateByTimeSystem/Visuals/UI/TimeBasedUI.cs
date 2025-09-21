using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameVisualUpdateByTimeSystem.Core.Interfaces;

namespace GameVisualUpdateByTimeSystem.Visuals.UI
{
    /// <summary>
    /// Manages UI changes based on time of day, month, and season
    /// Supports UI element visibility, colors, and animations
    /// </summary>
    public class TimeBasedUI : BaseVisualComponent, IInterpolatableVisual<UIState>
    {
        [Header("UI Configuration")]
        [SerializeField] private Canvas[] _canvases = new Canvas[0];
        [SerializeField] private UIElement[] _uiElements = new UIElement[0];
        [SerializeField] private bool _autoFindUIElements = true;
        
        [Header("UI States")]
        [SerializeField] private UIState[] _uiStates = new UIState[0];
        
        [Header("Performance")]
        [SerializeField] private float _updateInterval = 0.2f;
        
        private UIState _currentState;
        private UIState _targetState;
        private float _transitionProgress;
        private float _lastStateUpdateTime;
        
        // UI element tracking
        private Dictionary<string, UIElement> _elementLookup = new Dictionary<string, UIElement>();
        
        protected override void OnInitialize()
        {
            // Auto-find UI elements if not assigned
            if (_autoFindUIElements && _uiElements.Length == 0)
            {
                FindUIElements();
            }
            
            // Build element lookup
            BuildElementLookup();
            
            // Validate UI states
            if (_uiStates.Length == 0)
            {
                LogWarning("No UI states configured. Using default states.");
                CreateDefaultUIStates();
            }
            
            // Initialize with first state
            if (_uiStates.Length > 0)
            {
                _currentState = _uiStates[0];
                _targetState = _uiStates[0];
                ApplyUIState(_currentState);
            }
            
            LogInfo($"Initialized with {_uiElements.Length} UI elements and {_uiStates.Length} states");
        }
        
        private void FindUIElements()
        {
            var elements = new List<UIElement>();
            
            // Find UI elements in canvases
            foreach (var canvas in _canvases)
            {
                if (canvas == null) continue;
                
                var graphicElements = canvas.GetComponentsInChildren<Graphic>();
                foreach (var graphic in graphicElements)
                {
                    var element = new UIElement
                    {
                        GameObject = graphic.gameObject,
                        Graphic = graphic,
                        ElementId = graphic.name
                    };
                    elements.Add(element);
                }
            }
            
            // Find UI elements in scene if no canvases specified
            if (_canvases.Length == 0)
            {
                var allGraphics = FindObjectsOfType<Graphic>();
                foreach (var graphic in allGraphics)
                {
                    var element = new UIElement
                    {
                        GameObject = graphic.gameObject,
                        Graphic = graphic,
                        ElementId = graphic.name
                    };
                    elements.Add(element);
                }
            }
            
            _uiElements = elements.ToArray();
        }
        
        private void BuildElementLookup()
        {
            _elementLookup.Clear();
            
            foreach (var element in _uiElements)
            {
                if (!string.IsNullOrEmpty(element.ElementId))
                {
                    _elementLookup[element.ElementId] = element;
                }
            }
        }
        
        protected override void OnUpdateVisual(ITimeProvider timeProvider, float deltaTime)
        {
            // Update UI state based on current time
            UpdateUIState(timeProvider);
            
            // Interpolate between current and target states
            if (_transitionProgress < 1f)
            {
                _transitionProgress += deltaTime / _transitionDuration;
                _transitionProgress = Mathf.Clamp01(_transitionProgress);
                
                var interpolatedState = InterpolateUIStates(_currentState, _targetState, _transitionProgress);
                ApplyUIState(interpolatedState);
                
                if (_transitionProgress >= 1f)
                {
                    _currentState = _targetState;
                }
            }
        }
        
        private void UpdateUIState(ITimeProvider timeProvider)
        {
            if (Time.time - _lastStateUpdateTime < _updateInterval) return;
            
            var newTargetState = GetUIStateForTime(timeProvider);
            if (newTargetState.StateId != _targetState.StateId)
            {
                _currentState = GetCurrentUIState();
                _targetState = newTargetState;
                _transitionProgress = 0f;
                _lastStateUpdateTime = Time.time;
                
                LogInfo($"Transitioning to UI state: {_targetState.StateId}");
            }
        }
        
        private UIState GetUIStateForTime(ITimeProvider timeProvider)
        {
            // Find the most appropriate UI state for current time
            UIState bestState = _uiStates[0];
            float bestScore = float.MaxValue;
            
            foreach (var state in _uiStates)
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
        
        private float CalculateStateScore(UIState state, ITimeProvider timeProvider)
        {
            float score = 0f;
            var condition = state.TimeCondition;
            
            // Hour-based scoring
            if (condition.Hour != -1)
            {
                int hourDiff = Mathf.Abs(timeProvider.CurrentHour - condition.Hour);
                score += hourDiff * 8f;
            }
            
            // Season-based scoring
            if (condition.UseSeason)
            {
                var currentSeason = GetSeasonFromTime(timeProvider);
                if (currentSeason != condition.Season)
                {
                    score += 40f;
                }
            }
            
            // Month-based scoring
            if (condition.Month != -1)
            {
                int monthDiff = Mathf.Abs(timeProvider.CurrentMonth - condition.Month);
                score += monthDiff * 4f;
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
        
        private void ApplyUIState(UIState state)
        {
            foreach (var elementState in state.ElementStates)
            {
                if (_elementLookup.TryGetValue(elementState.ElementId, out var element))
                {
                    ApplyElementState(element, elementState);
                }
            }
        }
        
        private void ApplyElementState(UIElement element, UIElementState elementState)
        {
            if (element.GameObject == null || element.Graphic == null) return;
            
            // Apply visibility
            element.GameObject.SetActive(elementState.IsVisible);
            
            if (!elementState.IsVisible) return;
            
            // Apply color
            if (elementState.UseColor)
            {
                element.Graphic.color = elementState.Color;
            }
            
            // Apply alpha
            if (elementState.UseAlpha)
            {
                var color = element.Graphic.color;
                color.a = elementState.Alpha;
                element.Graphic.color = color;
            }
            
            // Apply scale
            if (elementState.UseScale)
            {
                element.GameObject.transform.localScale = elementState.Scale;
            }
            
            // Apply rotation
            if (elementState.UseRotation)
            {
                element.GameObject.transform.localRotation = Quaternion.Euler(elementState.Rotation);
            }
            
            // Apply position
            if (elementState.UsePosition)
            {
                element.GameObject.transform.localPosition = elementState.Position;
            }
        }
        
        private UIState InterpolateUIStates(UIState from, UIState to, float t)
        {
            var result = new UIState
            {
                stateId = to.StateId,
                timeCondition = to.TimeCondition,
                ElementStates = new UIElementState[to.ElementStates.Length]
            };
            
            for (int i = 0; i < to.ElementStates.Length; i++)
            {
                var toElement = to.ElementStates[i];
                var fromElement = FindElementState(from.ElementStates, toElement.ElementId);
                
                result.ElementStates[i] = new UIElementState
                {
                    ElementId = toElement.ElementId,
                    IsVisible = t > 0.5f ? toElement.IsVisible : (fromElement?.IsVisible ?? true),
                    UseColor = toElement.UseColor,
                    Color = SmoothInterpolate(fromElement?.Color ?? Color.white, toElement.Color, t),
                    UseAlpha = toElement.UseAlpha,
                    Alpha = SmoothInterpolate(fromElement?.Alpha ?? 1f, toElement.Alpha, t),
                    UseScale = toElement.UseScale,
                    Scale = SmoothInterpolate(fromElement?.Scale ?? Vector3.one, toElement.Scale, t),
                    UseRotation = toElement.UseRotation,
                    Rotation = SmoothInterpolate(fromElement?.Rotation ?? Vector3.zero, toElement.Rotation, t),
                    UsePosition = toElement.UsePosition,
                    Position = SmoothInterpolate(fromElement?.Position ?? Vector3.zero, toElement.Position, t)
                };
            }
            
            return result;
        }
        
        private UIElementState FindElementState(UIElementState[] elementStates, string elementId)
        {
            foreach (var element in elementStates)
            {
                if (element.ElementId == elementId)
                    return element;
            }
            return null;
        }
        
        private UIState GetCurrentUIState()
        {
            var state = new UIState
            {
                stateId = _currentState.StateId,
                timeCondition = _currentState.TimeCondition,
                ElementStates = new UIElementState[_uiElements.Length]
            };
            
            // Capture current UI state
            for (int i = 0; i < _uiElements.Length; i++)
            {
                var element = _uiElements[i];
                state.ElementStates[i] = new UIElementState
                {
                    ElementId = element.ElementId,
                    IsVisible = element.GameObject != null ? element.GameObject.activeInHierarchy : true,
                    UseColor = true,
                    Color = element.Graphic != null ? element.Graphic.color : Color.white,
                    UseAlpha = true,
                    Alpha = element.Graphic != null ? element.Graphic.color.a : 1f,
                    UseScale = true,
                    Scale = element.GameObject != null ? element.GameObject.transform.localScale : Vector3.one,
                    UseRotation = true,
                    Rotation = element.GameObject != null ? element.GameObject.transform.localRotation.eulerAngles : Vector3.zero,
                    UsePosition = true,
                    Position = element.GameObject != null ? element.GameObject.transform.localPosition : Vector3.zero
                };
            }
            
            return state;
        }
        
        private void CreateDefaultUIStates()
        {
            _uiStates = new UIState[]
            {
                // Day UI
                new UIState
                {
                    stateId = "Day",
                    timeCondition = new TimeCondition(12, -1, -1, -1, TimeType.Hour),
                    ElementStates = new UIElementState[0]
                },
                
                // Night UI
                new UIState
                {
                    stateId = "Night",
                    timeCondition = new TimeCondition(0, -1, -1, -1, TimeType.Hour),
                    ElementStates = new UIElementState[0]
                }
            };
        }
        
        #region IInterpolatableVisual Implementation
        
        public void Interpolate(UIState from, UIState to, float t)
        {
            var interpolatedState = InterpolateUIStates(from, to, t);
            ApplyUIState(interpolatedState);
        }
        
        public UIState GetCurrentState()
        {
            return GetCurrentUIState();
        }
        
        public void SetState(UIState state)
        {
            _currentState = state;
            _targetState = state;
            _transitionProgress = 1f;
            ApplyUIState(state);
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Adds a UI element to be controlled by this component
        /// </summary>
        /// <param name="gameObject">GameObject with UI component</param>
        /// <param name="elementId">Unique ID for the element</param>
        public void AddUIElement(GameObject gameObject, string elementId)
        {
            var graphic = gameObject.GetComponent<Graphic>();
            if (graphic == null)
            {
                LogWarning($"GameObject '{gameObject.name}' does not have a Graphic component");
                return;
            }
            
            var element = new UIElement
            {
                GameObject = gameObject,
                Graphic = graphic,
                ElementId = elementId
            };
            
            var newElements = new UIElement[_uiElements.Length + 1];
            _uiElements.CopyTo(newElements, 0);
            newElements[_uiElements.Length] = element;
            _uiElements = newElements;
            
            _elementLookup[elementId] = element;
            
            LogInfo($"Added UI element: {elementId}");
        }
        
        /// <summary>
        /// Removes a UI element from control
        /// </summary>
        /// <param name="elementId">ID of the element to remove</param>
        public void RemoveUIElement(string elementId)
        {
            if (_elementLookup.Remove(elementId))
            {
                var newElements = new UIElement[_uiElements.Length - 1];
                int index = 0;
                
                foreach (var element in _uiElements)
                {
                    if (element.ElementId != elementId)
                    {
                        newElements[index++] = element;
                    }
                }
                
                _uiElements = newElements;
                LogInfo($"Removed UI element: {elementId}");
            }
        }
        
        #endregion
        
        #region Editor Helpers
        
        [ContextMenu("Create Default UI States")]
        private void EditorCreateDefaultStates()
        {
            CreateDefaultUIStates();
            LogInfo("Created default UI states");
        }
        
        [ContextMenu("Apply Current State")]
        private void EditorApplyCurrentState()
        {
            if (_currentState.StateId != null)
            {
                ApplyUIState(_currentState);
                LogInfo($"Applied UI state: {_currentState.StateId}");
            }
        }
        
        [ContextMenu("Find UI Elements")]
        private void EditorFindUIElements()
        {
            FindUIElements();
            BuildElementLookup();
            LogInfo($"Found {_uiElements.Length} UI elements");
        }
        
        [ContextMenu("Capture Current UI")]
        private void EditorCaptureCurrentUI()
        {
            var capturedState = GetCurrentUIState();
            capturedState.stateId = "Captured_" + DateTime.Now.ToString("HHmm");
            LogInfo($"Captured current UI as: {capturedState.StateId}");
        }
        
        #endregion
    }
    
    /// <summary>
    /// Represents a UI element that can be controlled
    /// </summary>
    [Serializable]
    public class UIElement
    {
        [Tooltip("GameObject containing the UI element")]
        public GameObject GameObject;
        
        [Tooltip("Graphic component of the UI element")]
        public Graphic Graphic;
        
        [Tooltip("Unique identifier for this UI element")]
        public string ElementId;
    }
}
