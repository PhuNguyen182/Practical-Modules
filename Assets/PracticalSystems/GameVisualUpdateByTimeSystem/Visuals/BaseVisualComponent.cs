using GameVisualUpdateByTimeSystem.Core;
using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;

namespace GameVisualUpdateByTimeSystem.Visuals
{
    /// <summary>
    /// Base class for all time-based visual components
    /// Provides common functionality and default implementations
    /// </summary>
    public abstract class BaseVisualComponent : MonoBehaviour, ITimeBasedVisual
    {
        [Header("Visual Component Configuration")]
        [SerializeField] protected string _visualId;
        [SerializeField] protected int _updatePriority = 100;
        [SerializeField] protected bool _isActive = true;
        [SerializeField] protected bool _enableSmoothTransitions = true;
        [SerializeField] protected float _transitionDuration = 1.0f;
        
        [Header("Debug")]
        [SerializeField] protected bool _enableDebugLogging = false;
        
        // Protected fields for derived classes
        protected ITimeProvider _timeProvider;
        protected float _lastUpdateTime;
        protected bool _isInitialized = false;
        
        // Properties
        public string VisualId => string.IsNullOrEmpty(_visualId) ? GetType().Name : _visualId;
        public int UpdatePriority => _updatePriority;
        
        public virtual bool IsActive
        {
            get => _isActive && enabled && gameObject.activeInHierarchy;
            set => _isActive = value;
        }
        
        #region ITimeBasedVisual Implementation
        
        public virtual void Initialize()
        {
            if (_isInitialized) return;
            
            OnInitialize();
            _isInitialized = true;
            
            if (_enableDebugLogging)
            {
                Debug.Log($"[{GetType().Name}] Initialized: {VisualId}", this);
            }
        }
        
        public virtual void UpdateVisual(ITimeProvider timeProvider, float deltaTime)
        {
            if (!IsActive || timeProvider == null) return;
            
            _timeProvider = timeProvider;
            
            // Throttle updates to avoid excessive computation
            if (ShouldUpdate(deltaTime))
            {
                OnUpdateVisual(timeProvider, deltaTime);
                _lastUpdateTime = Time.time;
            }
        }
        
        public virtual void Cleanup()
        {
            if (!_isInitialized) return;
            
            OnCleanup();
            _isInitialized = false;
            
            if (_enableDebugLogging)
            {
                Debug.Log($"[{GetType().Name}] Cleaned up: {VisualId}", this);
            }
        }
        
        #endregion
        
        #region Abstract Methods
        
        /// <summary>
        /// Called during initialization. Override to implement component-specific setup.
        /// </summary>
        protected abstract void OnInitialize();
        
        /// <summary>
        /// Called during visual update. Override to implement component-specific update logic.
        /// </summary>
        /// <param name="timeProvider">Current time provider</param>
        /// <param name="deltaTime">Time since last update</param>
        protected abstract void OnUpdateVisual(ITimeProvider timeProvider, float deltaTime);
        
        /// <summary>
        /// Called during cleanup. Override to implement component-specific cleanup.
        /// </summary>
        protected virtual void OnCleanup() { }
        
        #endregion
        
        #region Virtual Methods
        
        /// <summary>
        /// Determines if the visual should update this frame
        /// Override to implement custom update throttling
        /// </summary>
        /// <param name="deltaTime">Time since last update</param>
        /// <returns>True if should update, false otherwise</returns>
        protected virtual bool ShouldUpdate(float deltaTime)
        {
            return true; // Default: update every frame
        }
        
        /// <summary>
        /// Smooth interpolation helper for float values
        /// </summary>
        /// <param name="from">Start value</param>
        /// <param name="to">Target value</param>
        /// <param name="t">Interpolation factor (0-1)</param>
        /// <returns>Interpolated value</returns>
        protected virtual float SmoothInterpolate(float from, float to, float t)
        {
            if (!_enableSmoothTransitions) return to;
            
            return Mathf.Lerp(from, to, t);
        }
        
        /// <summary>
        /// Smooth interpolation helper for Color values
        /// </summary>
        /// <param name="from">Start color</param>
        /// <param name="to">Target color</param>
        /// <param name="t">Interpolation factor (0-1)</param>
        /// <returns>Interpolated color</returns>
        protected virtual Color SmoothInterpolate(Color from, Color to, float t)
        {
            if (!_enableSmoothTransitions) return to;
            
            return Color.Lerp(from, to, t);
        }
        
        /// <summary>
        /// Smooth interpolation helper for Vector3 values
        /// </summary>
        /// <param name="from">Start vector</param>
        /// <param name="to">Target vector</param>
        /// <param name="t">Interpolation factor (0-1)</param>
        /// <returns>Interpolated vector</returns>
        protected virtual Vector3 SmoothInterpolate(Vector3 from, Vector3 to, float t)
        {
            if (!_enableSmoothTransitions) return to;
            
            return Vector3.Lerp(from, to, t);
        }
        
        protected virtual Vector4 SmoothInterpolate(Vector4 from, Vector4 to, float t)
        {
            if (!_enableSmoothTransitions) return to;
            
            return Vector4.Lerp(from, to, t);
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Get normalized time value for interpolation
        /// </summary>
        /// <param name="timeType">Type of time normalization</param>
        /// <returns>Normalized time value (0-1)</returns>
        protected virtual float GetNormalizedTime(TimeType timeType)
        {
            return _timeProvider?.GetNormalizedTime(timeType) ?? 0f;
        }
        
        /// <summary>
        /// Check if current time matches a condition
        /// </summary>
        /// <param name="condition">Time condition to check</param>
        /// <returns>True if condition is met</returns>
        protected virtual bool IsTimeConditionMet(TimeCondition condition)
        {
            if (_timeProvider == null) return false;
            
            bool hourMatch = condition.Hour == -1 || _timeProvider.CurrentHour == condition.Hour;
            bool dayMatch = condition.Day == -1 || _timeProvider.CurrentDay == condition.Day;
            bool monthMatch = condition.Month == -1 || _timeProvider.CurrentMonth == condition.Month;
            bool yearMatch = condition.Year == -1 || _timeProvider.CurrentYear == condition.Year;
            
            return hourMatch && dayMatch && monthMatch && yearMatch;
        }
        
        #endregion
        
        #region Unity Lifecycle
        
        protected virtual void Awake()
        {
            // Generate unique ID if not set
            if (string.IsNullOrEmpty(_visualId))
            {
                _visualId = $"{GetType().Name}_{GetInstanceID()}";
            }
        }
        
        protected virtual void Start()
        {
            // Auto-register with system if not already done
            var system = FindObjectOfType<TimeBasedVisualSystem>();
            if (system != null)
            {
                system.RegisterVisual(this);
            }
        }
        
        protected virtual void OnDestroy()
        {
            // Auto-unregister from system
            var system = FindObjectOfType<TimeBasedVisualSystem>();
            if (system != null)
            {
                system.UnregisterVisual(this);
            }
            
            Cleanup();
        }
        
        #endregion
        
        #region Debug Methods
        
        protected virtual void LogInfo(string message)
        {
            if (_enableDebugLogging)
            {
                Debug.Log($"[{GetType().Name}] {message}", this);
            }
        }
        
        protected virtual void LogWarning(string message)
        {
            if (_enableDebugLogging)
            {
                Debug.LogWarning($"[{GetType().Name}] {message}", this);
            }
        }
        
        protected virtual void LogError(string message)
        {
            Debug.LogError($"[{GetType().Name}] {message}", this);
        }
        
        #endregion
    }
}
