using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PracticalSystems.ThemeSystem.Core;
using PracticalSystems.ThemeSystem.Components;

namespace PracticalSystems.ThemeSystem.Demo
{
    /// <summary>
    /// Demo script that showcases the theme system functionality
    /// </summary>
    public class ThemeSystemDemo : MonoBehaviour
    {
        [Header("Demo Settings")]
        [SerializeField] private bool autoSetup = true;
        [SerializeField] private bool enableDemoControls = true;
        
        [Header("Demo UI")]
        [SerializeField] private Button themeChangeButton;
        [SerializeField] private TMP_Text themeInfoText;
        [SerializeField] private Slider transitionDurationSlider;
        [SerializeField] private Toggle debugModeToggle;
        
        [Header("Demo References")]
        [SerializeField] private ThemeController themeController;
        [SerializeField] private UIThemeComponent demoUIComponent;
        [SerializeField] private EnvironmentThemeComponent demoEnvironmentComponent;
        [SerializeField] private AudioThemeComponent demoAudioComponent;
        [SerializeField] private CharacterThemeComponent demoCharacterComponent;
        
        private string[] availablePresets;
        private int currentPresetIndex = 0;
        
        private void Start()
        {
            if (autoSetup)
            {
                SetupDemo();
            }
            
            if (enableDemoControls)
            {
                SetupDemoControls();
            }
        }
        
        /// <summary>
        /// Sets up the demo scene
        /// </summary>
        private void SetupDemo()
        {
            // Find or create theme controller
            if (themeController == null)
            {
                themeController = FindObjectOfType<ThemeController>();
                if (themeController == null)
                {
                    var controllerObject = new GameObject("Theme Controller");
                    themeController = controllerObject.AddComponent<ThemeController>();
                }
            }
            
            // Find or create demo components
            SetupDemoComponents();
            
            // Initialize theme controller
            themeController.Initialize();
            
            // Get available presets
            availablePresets = themeController.GetAvailablePresetNames();
            
            Debug.Log($"[Theme System Demo] Setup complete with {availablePresets.Length} available presets");
        }
        
        /// <summary>
        /// Sets up demo components
        /// </summary>
        private void SetupDemoComponents()
        {
            // Create demo UI
            CreateDemoUI();
            
            // Create demo environment
            CreateDemoEnvironment();
            
            // Create demo audio
            CreateDemoAudio();
            
            // Create demo character
            CreateDemoCharacter();
        }
        
        /// <summary>
        /// Creates demo UI elements
        /// </summary>
        private void CreateDemoUI()
        {
            if (demoUIComponent == null)
            {
                var canvas = FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    var canvasObject = new GameObject("Demo Canvas");
                    canvas = canvasObject.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasObject.AddComponent<CanvasScaler>();
                    canvasObject.AddComponent<GraphicRaycaster>();
                }
                
                var uiObject = new GameObject("Demo UI");
                uiObject.transform.SetParent(canvas.transform);
                
                demoUIComponent = uiObject.AddComponent<UIThemeComponent>();
                
                // Add some demo UI elements
                var button = uiObject.AddComponent<Button>();
                var buttonImage = uiObject.AddComponent<Image>();
                button.targetGraphic = buttonImage;
                
                var buttonText = new GameObject("Button Text");
                buttonText.transform.SetParent(uiObject.transform);
                var tmpText = buttonText.AddComponent<TMP_Text>();
                tmpText.text = "Demo Button";
                tmpText.fontSize = 14f;
                tmpText.color = Color.white;
                tmpText.alignment = TextAlignmentOptions.Center;
                
                var rectTransform = buttonText.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
        }
        
        /// <summary>
        /// Creates demo environment
        /// </summary>
        private void CreateDemoEnvironment()
        {
            if (demoEnvironmentComponent == null)
            {
                var envObject = new GameObject("Demo Environment");
                demoEnvironmentComponent = envObject.AddComponent<EnvironmentThemeComponent>();
                
                // Add a simple cube for environment demo
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.SetParent(envObject.transform);
                cube.transform.localPosition = Vector3.zero;
                cube.name = "Demo Cube";
            }
        }
        
        /// <summary>
        /// Creates demo audio
        /// </summary>
        private void CreateDemoAudio()
        {
            if (demoAudioComponent == null)
            {
                var audioObject = new GameObject("Demo Audio");
                demoAudioComponent = audioObject.AddComponent<AudioThemeComponent>();
                
                // Add audio source for demo
                var audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }
        
        /// <summary>
        /// Creates demo character
        /// </summary>
        private void CreateDemoCharacter()
        {
            if (demoCharacterComponent == null)
            {
                var charObject = new GameObject("Demo Character");
                demoCharacterComponent = charObject.AddComponent<CharacterThemeComponent>();
                
                // Add a simple capsule for character demo
                var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                capsule.transform.SetParent(charObject.transform);
                capsule.transform.localPosition = Vector3.zero;
                capsule.name = "Demo Capsule";
            }
        }
        
        /// <summary>
        /// Sets up demo controls
        /// </summary>
        private void SetupDemoControls()
        {
            if (themeChangeButton != null)
            {
                themeChangeButton.onClick.AddListener(ChangeTheme);
            }
            
            if (transitionDurationSlider != null)
            {
                transitionDurationSlider.value = themeController.TransitionDuration;
                transitionDurationSlider.onValueChanged.AddListener(OnTransitionDurationChanged);
            }
            
            if (debugModeToggle != null)
            {
                debugModeToggle.isOn = themeController.DebugMode;
                debugModeToggle.onValueChanged.AddListener(OnDebugModeChanged);
            }
            
            UpdateThemeInfo();
        }
        
        /// <summary>
        /// Changes to the next theme preset
        /// </summary>
        public void ChangeTheme()
        {
            if (availablePresets.Length == 0)
            {
                Debug.LogWarning("[Theme System Demo] No presets available");
                return;
            }
            
            currentPresetIndex = (currentPresetIndex + 1) % availablePresets.Length;
            var presetName = availablePresets[currentPresetIndex];
            var preset = themeController.GetPresetByName(presetName);
            
            if (preset != null)
            {
                themeController.ApplyPreset(preset);
                UpdateThemeInfo();
                Debug.Log($"[Theme System Demo] Applied preset: {presetName}");
            }
        }
        
        /// <summary>
        /// Updates theme information display
        /// </summary>
        private void UpdateThemeInfo()
        {
            if (themeInfoText != null)
            {
                var activeThemes = themeController.GetAllActiveThemes();
                var info = $"Active Themes: {activeThemes.Count}\n";
                
                foreach (var kvp in activeThemes)
                {
                    info += $"{kvp.Key}: {kvp.Value?.ThemeName ?? "None"}\n";
                }
                
                if (availablePresets.Length > 0)
                {
                    info += $"\nCurrent Preset: {availablePresets[currentPresetIndex]}";
                }
                
                themeInfoText.text = info;
            }
        }
        
        /// <summary>
        /// Called when transition duration slider value changes
        /// </summary>
        /// <param name="value">New transition duration</param>
        private void OnTransitionDurationChanged(float value)
        {
            // This would require exposing the transition duration property
            Debug.Log($"[Theme System Demo] Transition duration changed to: {value}");
        }
        
        /// <summary>
        /// Called when debug mode toggle value changes
        /// </summary>
        /// <param name="value">New debug mode state</param>
        private void OnDebugModeChanged(bool value)
        {
            // This would require exposing the debug mode property
            Debug.Log($"[Theme System Demo] Debug mode changed to: {value}");
        }
        
        /// <summary>
        /// Applies a specific theme preset by name
        /// </summary>
        /// <param name="presetName">Name of the preset to apply</param>
        public void ApplyPresetByName(string presetName)
        {
            var preset = themeController.GetPresetByName(presetName);
            if (preset != null)
            {
                themeController.ApplyPreset(preset);
                UpdateThemeInfo();
            }
            else
            {
                Debug.LogWarning($"[Theme System Demo] Preset '{presetName}' not found");
            }
        }
        
        /// <summary>
        /// Refreshes all themes
        /// </summary>
        public void RefreshAllThemes()
        {
            themeController.RefreshAllThemes();
            Debug.Log("[Theme System Demo] Refreshed all themes");
        }
        
        /// <summary>
        /// Validates the theme system
        /// </summary>
        public void ValidateSystem()
        {
            bool isValid = themeController.ValidateSystem();
            Debug.Log($"[Theme System Demo] System validation: {(isValid ? "PASSED" : "FAILED")}");
        }
        
        private void Update()
        {
            // Update theme info periodically
            if (Time.frameCount % 60 == 0) // Update every 60 frames
            {
                UpdateThemeInfo();
            }
        }
        
        [ContextMenu("Setup Demo")]
        private void SetupDemoMenu()
        {
            SetupDemo();
        }
        
        [ContextMenu("Change Theme")]
        private void ChangeThemeMenu()
        {
            ChangeTheme();
        }
        
        [ContextMenu("Refresh All Themes")]
        private void RefreshAllThemesMenu()
        {
            RefreshAllThemes();
        }
        
        [ContextMenu("Validate System")]
        private void ValidateSystemMenu()
        {
            ValidateSystem();
        }
    }
}
