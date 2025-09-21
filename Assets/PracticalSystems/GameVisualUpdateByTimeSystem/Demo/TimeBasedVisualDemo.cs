using UnityEngine;
using UnityEngine.UI;
using GameVisualUpdateByTimeSystem.Core.Interfaces;
using GameVisualUpdateByTimeSystem.Core;
using GameVisualUpdateByTimeSystem.Configurations;
using GameVisualUpdateByTimeSystem.Visuals.Audio;
using GameVisualUpdateByTimeSystem.Visuals.Lighting;
using GameVisualUpdateByTimeSystem.Visuals.Materials;

namespace GameVisualUpdateByTimeSystem.Demo
{
    /// <summary>
    /// Demo script showing how to use the Time-Based Visual System
    /// Provides UI controls and examples for testing the system
    /// </summary>
    public class TimeBasedVisualDemo : MonoBehaviour
    {
        [Header("Demo Configuration")]
        [SerializeField] private TimeBasedVisualConfiguration _demoConfiguration;
        [SerializeField] private bool _autoSetup = true;
        [SerializeField] private bool _enableDemoUI = true;
        
        [Header("Demo UI Controls")]
        [SerializeField] private Button _playPauseButton;
        [SerializeField] private Button _speedUpButton;
        [SerializeField] private Button _speedDownButton;
        [SerializeField] private Button _resetTimeButton;
        [SerializeField] private Button _setDawnButton;
        [SerializeField] private Button _setNoonButton;
        [SerializeField] private Button _setDuskButton;
        [SerializeField] private Button _setNightButton;
        [SerializeField] private Slider _timeSpeedSlider;
        [SerializeField] private Text _timeDisplayText;
        [SerializeField] private Text _systemStatusText;
        
        [Header("Demo Objects")]
        [SerializeField] private GameObject _demoLight;
        [SerializeField] private GameObject _demoSkybox;
        [SerializeField] private GameObject _demoMaterial;
        [SerializeField] private GameObject _demoAudio;
        [SerializeField] private GameObject _demoUI;
        
        // System references
        private ITimeBasedVisualSystem _visualSystem;
        private ITimeProvider _timeProvider;
        private TimeBasedVisualSystemService _systemService;
        
        // Demo state
        private bool _isInitialized = false;
        private float _lastUpdateTime;
        
        private void Start()
        {
            if (_autoSetup)
            {
                SetupDemo();
            }
        }
        
        private void Update()
        {
            if (_isInitialized && _enableDemoUI)
            {
                UpdateDemoUI();
            }
        }
        
        /// <summary>
        /// Setup the demo with visual system and components
        /// </summary>
        public void SetupDemo()
        {
            LogDemo("Setting up Time-Based Visual System Demo...");
            
            // Create or find the visual system service
            _systemService = FindObjectOfType<TimeBasedVisualSystemService>();
            if (_systemService == null)
            {
                var serviceGO = new GameObject("TimeBasedVisualSystemService");
                _systemService = serviceGO.AddComponent<TimeBasedVisualSystemService>();
            }
            
            // Initialize the service
            _systemService.InitializeService();
            
            // Get system references
            _visualSystem = _systemService.VisualSystem;
            _timeProvider = _systemService.TimeProvider;
            
            // Apply demo configuration if provided
            if (_demoConfiguration != null)
            {
                _demoConfiguration.ApplyToSystem(_visualSystem);
                LogDemo($"Applied configuration: {_demoConfiguration.ConfigurationName}");
            }
            
            // Setup demo objects and components
            SetupDemoObjects();
            SetupDemoUI();
            
            _isInitialized = true;
            LogDemo("Demo setup completed successfully!");
        }
        
        private void SetupDemoObjects()
        {
            // Create demo light if not assigned
            if (_demoLight == null)
            {
                _demoLight = new GameObject("Demo Light");
                var light = _demoLight.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.5f;
                light.color = Color.white;
                
                // Add time-based lighting component
                var timeBasedLighting = _demoLight.AddComponent<TimeBasedLighting>();
                LogDemo("Created demo light with TimeBasedLighting component");
            }
            
            // Create demo material object if not assigned
            if (_demoMaterial == null)
            {
                _demoMaterial = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                _demoMaterial.name = "Demo Material";
                _demoMaterial.transform.position = new Vector3(0, 1, 0);
                
                // Add time-based material properties component
                var timeBasedMaterials = _demoMaterial.AddComponent<TimeBasedMaterialProperties>();
                LogDemo("Created demo material object with TimeBasedMaterialProperties component");
            }
            
            // Create demo audio source if not assigned
            if (_demoAudio == null)
            {
                _demoAudio = new GameObject("Demo Audio");
                var audioSource = _demoAudio.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = true;
                
                // Add time-based audio component
                var timeBasedAudio = _demoAudio.AddComponent<TimeBasedAudio>();
                LogDemo("Created demo audio source with TimeBasedAudio component");
            }
            
            // Create demo UI if not assigned
            if (_demoUI == null && _enableDemoUI)
            {
                CreateDemoUI();
            }
        }
        
        private void CreateDemoUI()
        {
            // Create canvas
            var canvasGO = new GameObject("Demo Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Create time display
            var timeDisplayGO = new GameObject("Time Display");
            timeDisplayGO.transform.SetParent(canvasGO.transform);
            var timeDisplayRect = timeDisplayGO.AddComponent<RectTransform>();
            timeDisplayRect.anchorMin = new Vector2(0, 1);
            timeDisplayRect.anchorMax = new Vector2(0, 1);
            timeDisplayRect.anchoredPosition = new Vector2(150, -30);
            timeDisplayRect.sizeDelta = new Vector2(300, 60);
            
            _timeDisplayText = timeDisplayGO.AddComponent<Text>();
            _timeDisplayText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            _timeDisplayText.fontSize = 24;
            _timeDisplayText.alignment = TextAnchor.MiddleLeft;
            _timeDisplayText.color = Color.white;
            _timeDisplayText.text = "Time: 00:00";
            
            // Create system status display
            var statusDisplayGO = new GameObject("System Status");
            statusDisplayGO.transform.SetParent(canvasGO.transform);
            var statusDisplayRect = statusDisplayGO.AddComponent<RectTransform>();
            statusDisplayRect.anchorMin = new Vector2(0, 1);
            statusDisplayRect.anchorMax = new Vector2(0, 1);
            statusDisplayRect.anchoredPosition = new Vector2(150, -80);
            statusDisplayRect.sizeDelta = new Vector2(300, 40);
            
            _systemStatusText = statusDisplayGO.AddComponent<Text>();
            _systemStatusText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            _systemStatusText.fontSize = 18;
            _systemStatusText.alignment = TextAnchor.MiddleLeft;
            _systemStatusText.color = Color.green;
            _systemStatusText.text = "System: Ready";
            
            // Create control buttons
            CreateControlButtons(canvasGO);
            
            _demoUI = canvasGO;
            LogDemo("Created demo UI with controls");
        }
        
        private void CreateControlButtons(GameObject canvas)
        {
            var buttonHeight = 40f;
            var buttonWidth = 120f;
            var buttonSpacing = 10f;
            var startY = -150f;
            
            // Play/Pause button
            _playPauseButton = CreateButton("Play/Pause", canvas, new Vector2(-200, startY), new Vector2(buttonWidth, buttonHeight));
            _playPauseButton.onClick.AddListener(TogglePlayPause);
            
            // Speed controls
            _speedDownButton = CreateButton("Speed -", canvas, new Vector2(-60, startY), new Vector2(buttonWidth, buttonHeight));
            _speedDownButton.onClick.AddListener(DecreaseTimeSpeed);
            
            _speedUpButton = CreateButton("Speed +", canvas, new Vector2(80, startY), new Vector2(buttonWidth, buttonHeight));
            _speedUpButton.onClick.AddListener(IncreaseTimeSpeed);
            
            // Time speed slider
            var sliderGO = new GameObject("Time Speed Slider");
            sliderGO.transform.SetParent(canvas.transform);
            var sliderRect = sliderGO.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.5f, 1);
            sliderRect.anchorMax = new Vector2(0.5f, 1);
            sliderRect.anchoredPosition = new Vector2(0, startY - 60);
            sliderRect.sizeDelta = new Vector2(300, 30);
            
            _timeSpeedSlider = sliderGO.AddComponent<Slider>();
            _timeSpeedSlider.minValue = 0.1f;
            _timeSpeedSlider.maxValue = 10f;
            _timeSpeedSlider.value = 1f;
            _timeSpeedSlider.onValueChanged.AddListener(OnTimeSpeedSliderChanged);
            
            // Time preset buttons
            var presetY = startY - 120;
            _setDawnButton = CreateButton("Dawn", canvas, new Vector2(-180, presetY), new Vector2(80, buttonHeight));
            _setDawnButton.onClick.AddListener(() => SetTime(6, 0));
            
            _setNoonButton = CreateButton("Noon", canvas, new Vector2(-80, presetY), new Vector2(80, buttonHeight));
            _setNoonButton.onClick.AddListener(() => SetTime(12, 0));
            
            _setDuskButton = CreateButton("Dusk", canvas, new Vector2(20, presetY), new Vector2(80, buttonHeight));
            _setDuskButton.onClick.AddListener(() => SetTime(18, 0));
            
            _setNightButton = CreateButton("Night", canvas, new Vector2(120, presetY), new Vector2(80, buttonHeight));
            _setNightButton.onClick.AddListener(() => SetTime(0, 0));
            
            // Reset button
            _resetTimeButton = CreateButton("Reset", canvas, new Vector2(220, presetY), new Vector2(80, buttonHeight));
            _resetTimeButton.onClick.AddListener(ResetTime);
        }
        
        private Button CreateButton(string text, GameObject parent, Vector2 position, Vector2 size)
        {
            var buttonGO = new GameObject($"{text} Button");
            buttonGO.transform.SetParent(parent.transform);
            
            var buttonRect = buttonGO.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 1);
            buttonRect.anchorMax = new Vector2(0.5f, 1);
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = size;
            
            var button = buttonGO.AddComponent<Button>();
            var image = buttonGO.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Button text
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform);
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            var buttonText = textGO.AddComponent<Text>();
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.fontSize = 16;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.white;
            buttonText.text = text;
            
            return button;
        }
        
        private void SetupDemoUI()
        {
            if (_enableDemoUI && _timeSpeedSlider != null)
            {
                _timeSpeedSlider.value = _timeProvider.TimeSpeed;
            }
        }
        
        private void UpdateDemoUI()
        {
            if (Time.time - _lastUpdateTime < 0.1f) return; // Update UI 10 times per second
            
            // Update time display
            if (_timeDisplayText != null && _timeProvider != null)
            {
                _timeDisplayText.text = $"Time: {_timeProvider.CurrentHour:D2}:{_timeProvider.CurrentMinute:D2} " +
                                      $"{_timeProvider.CurrentDay:D2}/{_timeProvider.CurrentMonth:D2}/{_timeProvider.CurrentYear}";
            }
            
            // Update system status
            if (_systemStatusText != null && _visualSystem != null)
            {
                var status = _visualSystem.IsRunning ? "Running" : "Stopped";
                var visualCount = _visualSystem.GetAllVisuals().Count;
                _systemStatusText.text = $"System: {status} | Visuals: {visualCount}";
                _systemStatusText.color = _visualSystem.IsRunning ? Color.green : Color.red;
            }
            
            _lastUpdateTime = Time.time;
        }
        
        #region UI Control Methods
        
        public void TogglePlayPause()
        {
            if (_timeProvider != null)
            {
                _timeProvider.IsPaused = !_timeProvider.IsPaused;
                LogDemo($"Time {_timeProvider.IsPaused} ? paused : resumed");
            }
        }
        
        public void IncreaseTimeSpeed()
        {
            if (_timeProvider != null)
            {
                _timeProvider.TimeSpeed = Mathf.Min(_timeProvider.TimeSpeed * 1.5f, 100f);
                if (_timeSpeedSlider != null)
                {
                    _timeSpeedSlider.value = _timeProvider.TimeSpeed;
                }
                LogDemo($"Time speed increased to {_timeProvider.TimeSpeed:F1}x");
            }
        }
        
        public void DecreaseTimeSpeed()
        {
            if (_timeProvider != null)
            {
                _timeProvider.TimeSpeed = Mathf.Max(_timeProvider.TimeSpeed / 1.5f, 0.1f);
                if (_timeSpeedSlider != null)
                {
                    _timeSpeedSlider.value = _timeProvider.TimeSpeed;
                }
                LogDemo($"Time speed decreased to {_timeProvider.TimeSpeed:F1}x");
            }
        }
        
        public void OnTimeSpeedSliderChanged(float value)
        {
            if (_timeProvider != null)
            {
                _timeProvider.TimeSpeed = value;
            }
        }
        
        public void SetTime(int hour, int minute)
        {
            if (_timeProvider != null)
            {
                _timeProvider.SetTime(hour, minute, _timeProvider.CurrentDay, _timeProvider.CurrentMonth, _timeProvider.CurrentYear);
                LogDemo($"Time set to {hour:D2}:{minute:D2}");
            }
        }
        
        public void ResetTime()
        {
            if (_timeProvider != null)
            {
                _timeProvider.SetTime(6, 0, 1, 1, 2024);
                _timeProvider.TimeSpeed = 1f;
                if (_timeSpeedSlider != null)
                {
                    _timeSpeedSlider.value = 1f;
                }
                LogDemo("Time reset to 06:00, Day 1, Month 1, Year 2024");
            }
        }
        
        #endregion
        
        #region Demo Utility Methods
        
        /// <summary>
        /// Run a time progression demo
        /// </summary>
        /// <param name="duration">Duration in seconds</param>
        /// <param name="speed">Time speed multiplier</param>
        public void RunTimeProgressionDemo(float duration = 60f, float speed = 10f)
        {
            if (!_isInitialized)
            {
                LogDemo("Demo not initialized. Call SetupDemo() first.");
                return;
            }
            
            LogDemo($"Starting time progression demo for {duration} seconds at {speed}x speed");
            
            if (_timeProvider != null)
            {
                _timeProvider.TimeSpeed = speed;
                _timeProvider.IsPaused = false;
            }
            
            // Stop the demo after the specified duration
            Invoke(nameof(StopTimeProgressionDemo), duration);
        }
        
        private void StopTimeProgressionDemo()
        {
            if (_timeProvider != null)
            {
                _timeProvider.TimeSpeed = 1f;
                _timeProvider.IsPaused = true;
            }
            
            LogDemo("Time progression demo completed");
        }
        
        /// <summary>
        /// Demonstrate seasonal changes
        /// </summary>
        public void DemonstrateSeasonalChanges()
        {
            if (!_isInitialized)
            {
                LogDemo("Demo not initialized. Call SetupDemo() first.");
                return;
            }
            
            LogDemo("Demonstrating seasonal changes...");
            
            if (_timeProvider != null)
            {
                // Set to spring
                _timeProvider.SetTime(12, 0, 1, 3, 2024);
                LogDemo("Set to Spring (March)");
                
                // Cycle through seasons every 10 seconds
                Invoke(nameof(SetToSummer), 10f);
                Invoke(nameof(SetToAutumn), 20f);
                Invoke(nameof(SetToWinter), 30f);
                Invoke(nameof(SetToSpring), 40f);
            }
        }
        
        private void SetToSummer()
        {
            if (_timeProvider != null)
            {
                _timeProvider.SetTime(12, 0, 1, 6, 2024);
                LogDemo("Set to Summer (June)");
            }
        }
        
        private void SetToAutumn()
        {
            if (_timeProvider != null)
            {
                _timeProvider.SetTime(12, 0, 1, 9, 2024);
                LogDemo("Set to Autumn (September)");
            }
        }
        
        private void SetToWinter()
        {
            if (_timeProvider != null)
            {
                _timeProvider.SetTime(12, 0, 1, 12, 2024);
                LogDemo("Set to Winter (December)");
            }
        }
        
        private void SetToSpring()
        {
            if (_timeProvider != null)
            {
                _timeProvider.SetTime(12, 0, 1, 3, 2024);
                LogDemo("Set to Spring (March)");
            }
        }
        
        #endregion
        
        #region Debug Methods
        
        private void LogDemo(string message)
        {
            Debug.Log($"[TimeBasedVisualDemo] {message}");
        }
        
        #endregion
        
        #region Editor Helpers
        
        [ContextMenu("Setup Demo")]
        private void EditorSetupDemo()
        {
            if (!Application.isPlaying) return;
            SetupDemo();
        }
        
        [ContextMenu("Run Time Progression Demo")]
        private void EditorRunTimeProgressionDemo()
        {
            if (!Application.isPlaying) return;
            RunTimeProgressionDemo();
        }
        
        [ContextMenu("Demonstrate Seasonal Changes")]
        private void EditorDemonstrateSeasonalChanges()
        {
            if (!Application.isPlaying) return;
            DemonstrateSeasonalChanges();
        }
        
        #endregion
    }
}
