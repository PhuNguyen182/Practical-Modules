using System;
using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;
using UnityEngine.UI;

namespace GameVisualUpdateByTimeSystem.Visuals.UI
{
    /// <summary>
    /// Represents a complete UI state for time-based UI transitions
    /// </summary>
    [Serializable]
    public class UIState : IVisualState
    {
        [Header("State Configuration")]
        public string stateId;
        public TimeCondition timeCondition;
        
        [Header("UI Elements")]
        public UIElementState[] ElementStates = new UIElementState[0];
        
        public string StateId => stateId;
        public TimeCondition TimeCondition => timeCondition;
        
        public UIState()
        {
            stateId = "Default";
            timeCondition = new TimeCondition();
        }
        
        public UIState(string stateId, TimeCondition timeCondition)
        {
            this.stateId = stateId;
            this.timeCondition = timeCondition;
        }
        
        /// <summary>
        /// Creates a copy of this UI state
        /// </summary>
        /// <returns>Deep copy of the UI state</returns>
        public UIState Clone()
        {
            var clone = new UIState(StateId, TimeCondition)
            {
                ElementStates = new UIElementState[ElementStates.Length]
            };
            
            for (int i = 0; i < ElementStates.Length; i++)
            {
                clone.ElementStates[i] = ElementStates[i].Clone();
            }
            
            return clone;
        }
        
        /// <summary>
        /// Validates the UI state configuration
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(StateId))
            {
                Debug.LogError("UIState: StateId cannot be null or empty");
                return false;
            }
            
            if (ElementStates == null)
            {
                ElementStates = new UIElementState[0];
            }
            
            // Validate element states
            foreach (var elementState in ElementStates)
            {
                if (elementState != null && !elementState.IsValid())
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Gets a human-readable description of this UI state
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
            
            return $"{StateId}: {timeDesc}{seasonDesc}{monthDesc} ({ElementStates.Length} elements)";
        }
        
        /// <summary>
        /// Adds an element state to this UI state
        /// </summary>
        /// <param name="elementState">Element state to add</param>
        public void AddElementState(UIElementState elementState)
        {
            var newStates = new UIElementState[ElementStates.Length + 1];
            ElementStates.CopyTo(newStates, 0);
            newStates[ElementStates.Length] = elementState;
            ElementStates = newStates;
        }
        
        /// <summary>
        /// Removes an element state by element ID
        /// </summary>
        /// <param name="elementId">ID of the element to remove</param>
        public void RemoveElementState(string elementId)
        {
            for (int i = 0; i < ElementStates.Length; i++)
            {
                if (ElementStates[i].ElementId == elementId)
                {
                    var newStates = new UIElementState[ElementStates.Length - 1];
                    Array.Copy(ElementStates, 0, newStates, 0, i);
                    Array.Copy(ElementStates, i + 1, newStates, i, ElementStates.Length - i - 1);
                    ElementStates = newStates;
                    return;
                }
            }
        }
        
        /// <summary>
        /// Gets an element state by ID
        /// </summary>
        /// <param name="elementId">ID of the element</param>
        /// <returns>Element state or null if not found</returns>
        public UIElementState GetElementState(string elementId)
        {
            foreach (var elementState in ElementStates)
            {
                if (elementState.ElementId == elementId)
                    return elementState;
            }
            return null;
        }
        
        /// <summary>
        /// Creates a seasonal UI state
        /// </summary>
        /// <param name="season">Season for the UI</param>
        /// <param name="hour">Hour of day</param>
        /// <returns>Seasonal UI state</returns>
        public static UIState CreateSeasonalState(Season season, int hour = 12)
        {
            var state = new UIState($"Season_{season}_{hour:D2}h", new TimeCondition(hour, -1, -1, -1, TimeType.Hour, true, season));
            
            // Adjust UI colors based on season
            Color seasonColor = season switch
            {
                Season.Spring => new Color(0.8f, 1f, 0.8f, 1f), // Light green
                Season.Summer => new Color(1f, 1f, 0.8f, 1f),   // Light yellow
                Season.Autumn => new Color(1f, 0.8f, 0.6f, 1f), // Orange
                Season.Winter => new Color(0.9f, 0.9f, 1f, 1f), // Light blue
                _ => Color.white
            };
            
            // Adjust for time of day
            if (hour < 6 || hour > 20) // Night
            {
                seasonColor *= 0.7f; // Darker for night
            }
            
            // Create a default element state (can be customized)
            state.ElementStates = new UIElementState[]
            {
                new UIElementState
                {
                    ElementId = "Default",
                    IsVisible = true,
                    UseColor = true,
                    Color = seasonColor,
                    UseAlpha = true,
                    Alpha = 1f,
                    UseScale = false,
                    Scale = Vector3.one,
                    UseRotation = false,
                    Rotation = Vector3.zero,
                    UsePosition = false,
                    Position = Vector3.zero
                }
            };
            
            return state;
        }
    }
    
    /// <summary>
    /// Represents the state of a single UI element
    /// </summary>
    [Serializable]
    public class UIElementState
    {
        [Header("Element Configuration")]
        [Tooltip("Unique identifier for this UI element")]
        public string ElementId;
        
        [Header("Visibility")]
        [Tooltip("Whether the element is visible")]
        public bool IsVisible = true;
        
        [Header("Color Properties")]
        [Tooltip("Whether to use color override")]
        public bool UseColor = false;
        
        [Tooltip("Color of the UI element")]
        public Color Color = Color.white;
        
        [Tooltip("Whether to use alpha override")]
        public bool UseAlpha = false;
        
        [Range(0f, 1f)]
        [Tooltip("Alpha (transparency) of the UI element")]
        public float Alpha = 1f;
        
        [Header("Transform Properties")]
        [Tooltip("Whether to use scale override")]
        public bool UseScale = false;
        
        [Tooltip("Scale of the UI element")]
        public Vector3 Scale = Vector3.one;
        
        [Tooltip("Whether to use rotation override")]
        public bool UseRotation = false;
        
        [Tooltip("Rotation of the UI element (Euler angles)")]
        public Vector3 Rotation = Vector3.zero;
        
        [Tooltip("Whether to use position override")]
        public bool UsePosition = false;
        
        [Tooltip("Position of the UI element")]
        public Vector3 Position = Vector3.zero;
        
        [Header("Animation Properties")]
        [Tooltip("Animation duration for transitions")]
        public float AnimationDuration = 0.5f;
        
        [Tooltip("Animation curve for transitions")]
        public AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        public UIElementState()
        {
            ElementId = "Default";
            IsVisible = true;
            UseColor = false;
            Color = Color.white;
            UseAlpha = false;
            Alpha = 1f;
            UseScale = false;
            Scale = Vector3.one;
            UseRotation = false;
            Rotation = Vector3.zero;
            UsePosition = false;
            Position = Vector3.zero;
            AnimationDuration = 0.5f;
            AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
        
        public UIElementState(string elementId)
        {
            ElementId = elementId;
            IsVisible = true;
            UseColor = false;
            Color = Color.white;
            UseAlpha = false;
            Alpha = 1f;
            UseScale = false;
            Scale = Vector3.one;
            UseRotation = false;
            Rotation = Vector3.zero;
            UsePosition = false;
            Position = Vector3.zero;
            AnimationDuration = 0.5f;
            AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
        
        /// <summary>
        /// Creates a copy of this UI element state
        /// </summary>
        /// <returns>Deep copy of the UI element state</returns>
        public UIElementState Clone()
        {
            return new UIElementState
            {
                ElementId = ElementId,
                IsVisible = IsVisible,
                UseColor = UseColor,
                Color = Color,
                UseAlpha = UseAlpha,
                Alpha = Alpha,
                UseScale = UseScale,
                Scale = Scale,
                UseRotation = UseRotation,
                Rotation = Rotation,
                UsePosition = UsePosition,
                Position = Position,
                AnimationDuration = AnimationDuration,
                AnimationCurve = new AnimationCurve(AnimationCurve.keys)
            };
        }
        
        /// <summary>
        /// Validates the UI element state configuration
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(ElementId))
            {
                Debug.LogError("UIElementState: ElementId cannot be null or empty");
                return false;
            }
            
            if (Alpha < 0f || Alpha > 1f)
            {
                Debug.LogError("UIElementState: Alpha must be between 0 and 1");
                return false;
            }
            
            if (AnimationDuration < 0f)
            {
                Debug.LogError("UIElementState: AnimationDuration cannot be negative");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Applies this UI element state to a GameObject with UI components
        /// </summary>
        /// <param name="gameObject">GameObject to apply state to</param>
        public void ApplyToGameObject(GameObject gameObject)
        {
            if (gameObject == null) return;
            
            // Apply visibility
            gameObject.SetActive(IsVisible);
            
            if (!IsVisible) return;
            
            // Apply color to graphic component
            var graphic = gameObject.GetComponent<Graphic>();
            if (graphic != null)
            {
                if (UseColor)
                {
                    graphic.color = Color;
                }
                
                if (UseAlpha)
                {
                    var color = graphic.color;
                    color.a = Alpha;
                    graphic.color = color;
                }
            }
            
            // Apply transform properties
            if (UseScale)
            {
                gameObject.transform.localScale = Scale;
            }
            
            if (UseRotation)
            {
                gameObject.transform.localRotation = Quaternion.Euler(Rotation);
            }
            
            if (UsePosition)
            {
                gameObject.transform.localPosition = Position;
            }
        }
        
        /// <summary>
        /// Captures current state from a GameObject with UI components
        /// </summary>
        /// <param name="gameObject">GameObject to capture from</param>
        public void CaptureFromGameObject(GameObject gameObject)
        {
            if (gameObject == null) return;
            
            IsVisible = gameObject.activeInHierarchy;
            
            var graphic = gameObject.GetComponent<Graphic>();
            if (graphic != null)
            {
                Color = graphic.color;
                Alpha = graphic.color.a;
                UseColor = true;
                UseAlpha = true;
            }
            
            Scale = gameObject.transform.localScale;
            Rotation = gameObject.transform.localRotation.eulerAngles;
            Position = gameObject.transform.localPosition;
            UseScale = true;
            UseRotation = true;
            UsePosition = true;
        }
        
        /// <summary>
        /// Gets a human-readable description of this UI element state
        /// </summary>
        /// <returns>Description string</returns>
        public string GetDescription()
        {
            var props = new System.Text.StringBuilder();
            
            if (UseColor) props.Append($"Color: {Color}, ");
            if (UseAlpha) props.Append($"Alpha: {Alpha:F2}, ");
            if (UseScale) props.Append($"Scale: {Scale}, ");
            if (UseRotation) props.Append($"Rotation: {Rotation}, ");
            if (UsePosition) props.Append($"Position: {Position}, ");
            
            props.Append($"Visible: {IsVisible}");
            
            return $"{ElementId} ({props.ToString().TrimEnd(',', ' ')})";
        }
        
        /// <summary>
        /// Creates a UI element state for a button with seasonal colors
        /// </summary>
        /// <param name="elementId">ID of the element</param>
        /// <param name="season">Season for color selection</param>
        /// <param name="isNight">Whether it's night time</param>
        /// <returns>Seasonal button state</returns>
        public static UIElementState CreateSeasonalButton(string elementId, Season season, bool isNight = false)
        {
            var state = new UIElementState(elementId)
            {
                IsVisible = true,
                UseColor = true,
                UseAlpha = true,
                Alpha = isNight ? 0.8f : 1f
            };
            
            state.Color = season switch
            {
                Season.Spring => new Color(0.8f, 1f, 0.8f, 1f), // Light green
                Season.Summer => new Color(1f, 1f, 0.8f, 1f),   // Light yellow
                Season.Autumn => new Color(1f, 0.8f, 0.6f, 1f), // Orange
                Season.Winter => new Color(0.9f, 0.9f, 1f, 1f), // Light blue
                _ => Color.white
            };
            
            if (isNight)
            {
                state.Color *= 0.7f;
            }
            
            return state;
        }
        
        /// <summary>
        /// Creates a UI element state for a panel with time-based visibility
        /// </summary>
        /// <param name="elementId">ID of the element</param>
        /// <param name="hour">Hour of day</param>
        /// <param name="visibleHours">Hours when the panel should be visible</param>
        /// <returns>Time-based panel state</returns>
        public static UIElementState CreateTimeBasedPanel(string elementId, int hour, int[] visibleHours)
        {
            var state = new UIElementState(elementId);
            
            bool isVisible = Array.IndexOf(visibleHours, hour) >= 0;
            state.IsVisible = isVisible;
            state.UseAlpha = true;
            state.Alpha = isVisible ? 1f : 0f;
            
            return state;
        }
    }
}
