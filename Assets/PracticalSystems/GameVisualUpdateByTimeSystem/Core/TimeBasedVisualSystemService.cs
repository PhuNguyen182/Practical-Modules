using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;
using GameVisualUpdateByTimeSystem.Core.TimeProvider;
using PracticalModules.Patterns.ServiceLocator;

namespace GameVisualUpdateByTimeSystem.Core
{
    /// <summary>
    /// Service wrapper for TimeBasedVisualSystem to integrate with ServiceLocator
    /// Provides a clean interface for dependency injection and service management
    /// </summary>
    public class TimeBasedVisualSystemService : MonoBehaviour
    {
        [Header("Service Configuration")]
        [SerializeField] private bool _registerAsGlobalService = true;
        [SerializeField] private bool _autoInitialize = true;
        [SerializeField] private bool _enableServiceLogging = true;
        
        // Core components
        private ITimeBasedVisualSystem _visualSystem;
        private ITimeProvider _timeProvider;
        private ServiceLocator _serviceLocator;
        
        // Properties
        public ITimeBasedVisualSystem VisualSystem => _visualSystem;
        public ITimeProvider TimeProvider => _timeProvider;
        
        private void Awake()
        {
            if (_autoInitialize)
            {
                InitializeService();
            }
        }
        
        private void Start()
        {
            if (_registerAsGlobalService)
            {
                RegisterWithServiceLocator();
            }
        }
        
        private void OnDestroy()
        {
            UnregisterFromServiceLocator();
        }
        
        /// <summary>
        /// Initialize the service with default components
        /// </summary>
        public void InitializeService()
        {
            // Create time provider
            _timeProvider = new GameTimeProvider();
            
            // Create visual system
            _visualSystem = GetComponent<TimeBasedVisualSystem>();
            if (_visualSystem == null)
            {
                _visualSystem = gameObject.AddComponent<TimeBasedVisualSystem>();
            }
            
            // Initialize the visual system
            _visualSystem.Initialize();
            _visualSystem.SetTimeProvider(_timeProvider);
            
            LogService("Service initialized");
        }
        
        /// <summary>
        /// Initialize the service with custom time provider
        /// </summary>
        /// <param name="timeProvider">Custom time provider</param>
        public void InitializeService(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
            
            // Create visual system
            _visualSystem = GetComponent<TimeBasedVisualSystem>();
            if (_visualSystem == null)
            {
                _visualSystem = gameObject.AddComponent<TimeBasedVisualSystem>();
            }
            
            // Initialize the visual system
            _visualSystem.Initialize();
            _visualSystem.SetTimeProvider(_timeProvider);
            
            LogService("Service initialized with custom time provider");
        }
        
        /// <summary>
        /// Register this service with the ServiceLocator
        /// </summary>
        public void RegisterWithServiceLocator()
        {
            _serviceLocator = ServiceLocator.Global;
            
            // Register the service itself
            _serviceLocator.Register(this);
            
            // Register the visual system interface
            _serviceLocator.Register<ITimeBasedVisualSystem>(_visualSystem);
            
            // Register the time provider interface
            _serviceLocator.Register<ITimeProvider>(_timeProvider);
            
            LogService("Registered with ServiceLocator");
        }
        
        /// <summary>
        /// Unregister this service from the ServiceLocator
        /// </summary>
        public void UnregisterFromServiceLocator()
        {
            if (_serviceLocator != null)
            {
                _serviceLocator.Register<ITimeBasedVisualSystem>(null);
                _serviceLocator.Register<ITimeProvider>(null);
                
                LogService("Unregistered from ServiceLocator");
            }
        }
        
        /// <summary>
        /// Get the visual system from ServiceLocator
        /// </summary>
        /// <returns>Visual system instance or null</returns>
        public static ITimeBasedVisualSystem GetVisualSystem()
        {
            var serviceLocator = ServiceLocator.Global;
            if (serviceLocator.TryGet<ITimeBasedVisualSystem>(out var visualSystem))
            {
                return visualSystem;
            }
            return null;
        }
        
        /// <summary>
        /// Get the time provider from ServiceLocator
        /// </summary>
        /// <returns>Time provider instance or null</returns>
        public static ITimeProvider GetTimeProvider()
        {
            var serviceLocator = ServiceLocator.Global;
            if (serviceLocator.TryGet<ITimeProvider>(out var timeProvider))
            {
                return timeProvider;
            }
            return null;
        }
        
        /// <summary>
        /// Register a visual component with the system
        /// </summary>
        /// <param name="visual">Visual component to register</param>
        public void RegisterVisual(ITimeBasedVisual visual)
        {
            _visualSystem?.RegisterVisual(visual);
        }
        
        /// <summary>
        /// Unregister a visual component from the system
        /// </summary>
        /// <param name="visual">Visual component to unregister</param>
        public void UnregisterVisual(ITimeBasedVisual visual)
        {
            _visualSystem?.UnregisterVisual(visual);
        }
        
        /// <summary>
        /// Set the time speed for the time provider
        /// </summary>
        /// <param name="speed">Time speed multiplier</param>
        public void SetTimeSpeed(float speed)
        {
            if (_timeProvider != null)
            {
                _timeProvider.TimeSpeed = speed;
            }
        }
        
        /// <summary>
        /// Pause or resume time progression
        /// </summary>
        /// <param name="paused">Whether to pause time</param>
        public void SetTimePaused(bool paused)
        {
            if (_timeProvider != null)
            {
                _timeProvider.IsPaused = paused;
            }
        }
        
        /// <summary>
        /// Set specific time values
        /// </summary>
        /// <param name="hour">Hour (0-23)</param>
        /// <param name="minute">Minute (0-59)</param>
        /// <param name="day">Day (1-31)</param>
        /// <param name="month">Month (1-12)</param>
        /// <param name="year">Year</param>
        public void SetTime(int hour, int minute, int day, int month, int year)
        {
            if (_timeProvider != null)
            {
                _timeProvider.SetTime(hour, minute, day, month, year);
            }
        }
        
        /// <summary>
        /// Advance time by specified amount
        /// </summary>
        /// <param name="seconds">Seconds to advance</param>
        public void AdvanceTime(float seconds)
        {
            if (_timeProvider != null)
            {
                _timeProvider.AdvanceTime(seconds);
            }
        }
        
        /// <summary>
        /// Get current time information as a formatted string
        /// </summary>
        /// <returns>Formatted time string</returns>
        public string GetCurrentTimeString()
        {
            if (_timeProvider == null) return "No time provider";
            
            return $"{_timeProvider.CurrentHour:D2}:{_timeProvider.CurrentMinute:D2} " +
                   $"{_timeProvider.CurrentDay:D2}/{_timeProvider.CurrentMonth:D2}/{_timeProvider.CurrentYear}";
        }
        
        /// <summary>
        /// Enable or disable the visual system
        /// </summary>
        /// <param name="enabled">Whether to enable the system</param>
        public void SetSystemEnabled(bool enabled)
        {
            _visualSystem?.SetAutoUpdate(enabled);
        }
        
        /// <summary>
        /// Shutdown the service and cleanup resources
        /// </summary>
        public void ShutdownService()
        {
            UnregisterFromServiceLocator();
            
            if (_visualSystem != null)
            {
                _visualSystem.Shutdown();
            }
            
            LogService("Service shutdown");
        }
        
        #region Debug Methods
        
        private void LogService(string message)
        {
            if (_enableServiceLogging)
            {
                Debug.Log($"[TimeBasedVisualSystemService] {message}", this);
            }
        }
        
        #endregion
        
        #region Editor Helpers
        
        [ContextMenu("Initialize Service")]
        private void EditorInitializeService()
        {
            if (!Application.isPlaying) return;
            InitializeService();
        }
        
        [ContextMenu("Register with ServiceLocator")]
        private void EditorRegisterWithServiceLocator()
        {
            if (!Application.isPlaying) return;
            RegisterWithServiceLocator();
        }
        
        [ContextMenu("Print Service Status")]
        private void EditorPrintServiceStatus()
        {
            if (!Application.isPlaying) return;
            
            Debug.Log($"[TimeBasedVisualSystemService] Status: {(VisualSystem?.IsRunning == true ? "Running" : "Stopped")}");
            Debug.Log($"[TimeBasedVisualSystemService] Time: {GetCurrentTimeString()}");
            Debug.Log($"[TimeBasedVisualSystemService] Registered Visuals: {VisualSystem?.GetAllVisuals().Count ?? 0}");
            
            if (VisualSystem != null)
            {
                foreach (var visual in VisualSystem.GetAllVisuals())
                {
                    Debug.Log($"  - {visual.VisualId} (Priority: {visual.UpdatePriority}, Active: {visual.IsActive})");
                }
            }
        }
        
        [ContextMenu("Test Time Progression")]
        private void EditorTestTimeProgression()
        {
            if (!Application.isPlaying) return;
            
            LogService("Starting time progression test...");
            SetTimeSpeed(60f); // 60x speed
            
            // Advance time by 1 hour every second
            InvokeRepeating(nameof(AdvanceTimeByHour), 0f, 1f);
            
            // Stop after 24 hours
            Invoke(nameof(StopTimeProgressionTest), 24f);
        }
        
        private void AdvanceTimeByHour()
        {
            AdvanceTime(3600f); // Advance by 1 hour
        }
        
        private void StopTimeProgressionTest()
        {
            CancelInvoke(nameof(AdvanceTimeByHour));
            SetTimeSpeed(1f); // Reset to normal speed
            LogService("Time progression test completed");
        }
        
        #endregion
    }
}
