using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;
using GameVisualUpdateByTimeSystem.Core.TimeProvider;
using PracticalModules.PlayerLoopServices.Core.Handlers;
using PracticalModules.PlayerLoopServices.UpdateServices;

namespace GameVisualUpdateByTimeSystem.Core
{
    /// <summary>
    /// Main system for managing time-based visual updates
    /// Integrates with Unity's PlayerLoop for efficient updates
    /// </summary>
    public class TimeBasedVisualSystem : MonoBehaviour, ITimeBasedVisualSystem, IUpdateHandler
    {
        [Header("System Configuration")]
        [SerializeField] private bool _autoInitialize = true;
        [SerializeField] private bool _autoUpdate = true;
        [SerializeField] private float _updateFrequency = 1f / 60f; // 60 FPS
        [SerializeField] private bool _enableDebugLogging = false;
        
        [Header("Time Configuration")]
        [SerializeField] private int _startHour = 6;
        [SerializeField] private int _startMinute = 0;
        [SerializeField] private int _startDay = 1;
        [SerializeField] private int _startMonth = 1;
        [SerializeField] private int _startYear = 2024;
        [SerializeField] private float _timeSpeed = 1.0f;
        
        // Core components
        private ITimeProvider _timeProvider;
        private readonly List<ITimeBasedVisual> _visualComponents = new List<ITimeBasedVisual>();
        private readonly Dictionary<string, ITimeBasedVisual> _visualLookup = new Dictionary<string, ITimeBasedVisual>();
        
        // Update tracking
        private float _lastUpdateTime;
        private bool _isInitialized = false;
        
        // Events
        public event Action<ITimeBasedVisual> OnVisualRegistered;
        public event Action<ITimeBasedVisual> OnVisualUnregistered;
        
        public ITimeProvider TimeProvider => _timeProvider;
        public bool IsRunning => _isInitialized && _autoUpdate;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            if (_autoInitialize)
            {
                Initialize();
            }
        }
        
        private void OnEnable()
        {
            if (_isInitialized && _autoUpdate)
            {
                RegisterToUpdateSystem();
            }
        }
        
        private void OnDisable()
        {
            UnregisterFromUpdateSystem();
        }
        
        private void OnDestroy()
        {
            Shutdown();
        }
        
        #endregion
        
        #region ITimeBasedVisualSystem Implementation
        
        public void Initialize()
        {
            if (_isInitialized)
            {
                LogWarning("System already initialized");
                return;
            }
            
            // Create default time provider if none exists
            if (_timeProvider == null)
            {
                _timeProvider = new GameTimeProvider(_startHour, _startMinute, _startDay, _startMonth, _startYear);
                _timeProvider.TimeSpeed = _timeSpeed;
            }
            
            // Register with update system
            if (_autoUpdate)
            {
                RegisterToUpdateSystem();
            }
            
            _isInitialized = true;
            LogInfo("TimeBasedVisualSystem initialized");
        }
        
        public void Shutdown()
        {
            if (!_isInitialized) return;
            
            // Unregister from update system
            UnregisterFromUpdateSystem();
            
            // Cleanup all visual components
            foreach (var visual in _visualComponents.ToList())
            {
                UnregisterVisual(visual);
            }
            
            _visualComponents.Clear();
            _visualLookup.Clear();
            
            _isInitialized = false;
            LogInfo("TimeBasedVisualSystem shutdown");
        }
        
        public void RegisterVisual(ITimeBasedVisual visual)
        {
            if (visual == null)
            {
                LogError("Cannot register null visual component");
                return;
            }
            
            if (_visualLookup.ContainsKey(visual.VisualId))
            {
                LogWarning($"Visual component with ID '{visual.VisualId}' is already registered");
                return;
            }
            
            _visualComponents.Add(visual);
            _visualLookup[visual.VisualId] = visual;
            
            // Initialize the visual component
            visual.Initialize();
            
            // Sort by priority
            _visualComponents.Sort((a, b) => a.UpdatePriority.CompareTo(b.UpdatePriority));
            
            OnVisualRegistered?.Invoke(visual);
            LogInfo($"Registered visual component: {visual.VisualId}");
        }
        
        public void UnregisterVisual(ITimeBasedVisual visual)
        {
            if (visual == null) return;
            
            if (_visualLookup.Remove(visual.VisualId))
            {
                _visualComponents.Remove(visual);
                visual.Cleanup();
                OnVisualUnregistered?.Invoke(visual);
                LogInfo($"Unregistered visual component: {visual.VisualId}");
            }
        }
        
        public ITimeBasedVisual GetVisual(string visualId)
        {
            return _visualLookup.TryGetValue(visualId, out var visual) ? visual : null;
        }
        
        public IReadOnlyList<ITimeBasedVisual> GetAllVisuals()
        {
            return _visualComponents.AsReadOnly();
        }
        
        public IReadOnlyList<T> GetVisualsByType<T>() where T : class, ITimeBasedVisual
        {
            return _visualComponents.OfType<T>().ToList();
        }
        
        public void UpdateSystem(float deltaTime)
        {
            if (!_isInitialized || _timeProvider == null) return;
            
            // Update time provider
            if (_timeProvider is GameTimeProvider gameTimeProvider)
            {
                gameTimeProvider.UpdateTime(deltaTime);
            }
            
            // Update visual components (throttled by update frequency)
            if (Time.time - _lastUpdateTime >= _updateFrequency)
            {
                UpdateVisualComponents(deltaTime);
                _lastUpdateTime = Time.time;
            }
        }
        
        public void SetTimeProvider(ITimeProvider timeProvider)
        {
            if (timeProvider == null)
            {
                LogError("Cannot set null time provider");
                return;
            }
            
            _timeProvider = timeProvider;
            LogInfo("Time provider updated");
        }
        
        public void SetAutoUpdate(bool enabled)
        {
            if (_autoUpdate == enabled) return;
            
            _autoUpdate = enabled;
            
            if (enabled && _isInitialized)
            {
                RegisterToUpdateSystem();
            }
            else
            {
                UnregisterFromUpdateSystem();
            }
            
            LogInfo($"Auto update {(enabled ? "enabled" : "disabled")}");
        }
        
        #endregion
        
        #region IUpdateHandler Implementation
        
        public void Tick(float deltaTime)
        {
            UpdateSystem(deltaTime);
        }
        
        #endregion
        
        #region Private Methods
        
        private void RegisterToUpdateSystem()
        {
            if (_isInitialized)
            {
                UpdateServiceManager.RegisterUpdateHandler(this);
                LogInfo("Registered to update system");
            }
        }
        
        private void UnregisterFromUpdateSystem()
        {
            UpdateServiceManager.DeregisterUpdateHandler(this);
            LogInfo("Unregistered from update system");
        }
        
        private void UpdateVisualComponents(float deltaTime)
        {
            foreach (var visual in _visualComponents)
            {
                if (visual.IsActive)
                {
                    try
                    {
                        visual.UpdateVisual(_timeProvider, deltaTime);
                    }
                    catch (Exception e)
                    {
                        LogError($"Error updating visual component '{visual.VisualId}': {e.Message}");
                    }
                }
            }
        }
        
        #endregion
        
        #region Debug Methods
        
        private void LogInfo(string message)
        {
            if (_enableDebugLogging)
            {
                Debug.Log($"[TimeBasedVisualSystem] {message}", this);
            }
        }
        
        private void LogWarning(string message)
        {
            if (_enableDebugLogging)
            {
                Debug.LogWarning($"[TimeBasedVisualSystem] {message}", this);
            }
        }
        
        private void LogError(string message)
        {
            Debug.LogError($"[TimeBasedVisualSystem] {message}", this);
        }
        
        #endregion
        
        #region Editor Helpers
        
        [ContextMenu("Initialize System")]
        private void EditorInitialize()
        {
            if (!Application.isPlaying) return;
            Initialize();
        }
        
        [ContextMenu("Shutdown System")]
        private void EditorShutdown()
        {
            if (!Application.isPlaying) return;
            Shutdown();
        }
        
        [ContextMenu("Print System Status")]
        private void PrintSystemStatus()
        {
            if (!Application.isPlaying) return;
            
            Debug.Log($"[TimeBasedVisualSystem] Status: {(IsRunning ? "Running" : "Stopped")}");
            Debug.Log($"[TimeBasedVisualSystem] Time: {_timeProvider?.CurrentHour:D2}:{_timeProvider?.CurrentMinute:D2} {_timeProvider?.CurrentDay:D2}/{_timeProvider?.CurrentMonth:D2}/{_timeProvider?.CurrentYear}");
            Debug.Log($"[TimeBasedVisualSystem] Registered Visuals: {_visualComponents.Count}");
            
            foreach (var visual in _visualComponents)
            {
                Debug.Log($"  - {visual.VisualId} (Priority: {visual.UpdatePriority}, Active: {visual.IsActive})");
            }
        }
        
        #endregion
    }
}
