using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using PracticalSystems.ThemeSystem.Core;
using PracticalSystems.ThemeSystem.Managers;

namespace PracticalSystems.ThemeSystem.Editor
{
    /// <summary>
    /// Custom window for managing the theme system
    /// </summary>
    public class ThemeSystemWindow : EditorWindow
    {
        private ThemeController themeController;
        private Vector2 scrollPosition;
        private int selectedTab = 0;
        private readonly string[] tabNames = { "Overview", "Themes", "Components", "Presets", "Settings" };
        
        [MenuItem("Window/Theme System/Theme System Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<ThemeSystemWindow>("Theme System Manager");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }
        
        private void OnEnable()
        {
            FindThemeController();
        }
        
        private void OnGUI()
        {
            if (themeController == null)
            {
                EditorGUILayout.HelpBox("Theme Controller not found in the scene. Please add a ThemeController to your scene.", MessageType.Warning);
                
                if (GUILayout.Button("Find Theme Controller"))
                {
                    FindThemeController();
                }
                
                return;
            }
            
            // Draw tab bar
            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            switch (selectedTab)
            {
                case 0:
                    DrawOverviewTab();
                    break;
                case 1:
                    DrawThemesTab();
                    break;
                case 2:
                    DrawComponentsTab();
                    break;
                case 3:
                    DrawPresetsTab();
                    break;
                case 4:
                    DrawSettingsTab();
                    break;
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void FindThemeController()
        {
            themeController = FindObjectOfType<ThemeController>();
        }
        
        private void DrawOverviewTab()
        {
            EditorGUILayout.LabelField("Theme System Overview", EditorStyles.boldLabel);
            
            var stats = themeController.GetSystemStats();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField($"Initialized: {stats.isInitialized}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Transitioning: {stats.isTransitioning}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Total Managers: {stats.totalManagers}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Registered Components: {stats.totalRegisteredComponents}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Active Themes: {stats.activeThemes}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Total Presets: {stats.totalPresets}", EditorStyles.miniLabel);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("System Controls", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Initialize System"))
            {
                themeController.Initialize();
            }
            
            if (GUILayout.Button("Refresh All Themes"))
            {
                themeController.RefreshAllThemes();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Validate System"))
            {
                bool isValid = themeController.ValidateSystem();
                Debug.Log($"[Theme Controller] System validation: {(isValid ? "PASSED" : "FAILED")}");
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawThemesTab()
        {
            EditorGUILayout.LabelField("Active Themes", EditorStyles.boldLabel);
            
            var activeThemes = themeController.GetAllActiveThemes();
            
            if (activeThemes.Count > 0)
            {
                foreach (var kvp in activeThemes)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    
                    EditorGUILayout.LabelField(kvp.Key, EditorStyles.miniLabel, GUILayout.Width(100));
                    EditorGUILayout.ObjectField(kvp.Value as Object, typeof(Object), true);
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No active themes", MessageType.Info);
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Theme Managers", EditorStyles.boldLabel);
            
            var uiManager = FindObjectOfType<UIThemeManager>();
            var envManager = FindObjectOfType<EnvironmentThemeManager>();
            var audioManager = FindObjectOfType<AudioThemeManager>();
            var charManager = FindObjectOfType<CharacterThemeManager>();
            
            DrawManagerInfo("UI Manager", uiManager);
            DrawManagerInfo("Environment Manager", envManager);
            DrawManagerInfo("Audio Manager", audioManager);
            DrawManagerInfo("Character Manager", charManager);
        }
        
        private void DrawManagerInfo(string name, BaseThemeManager manager)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField(name, EditorStyles.miniLabel, GUILayout.Width(150));
            
            if (manager != null)
            {
                EditorGUILayout.ObjectField(manager, typeof(BaseThemeManager), true);
                
                var componentCount = manager.GetRegisteredComponents().Count;
                EditorGUILayout.LabelField($"Components: {componentCount}", EditorStyles.miniLabel, GUILayout.Width(100));
            }
            else
            {
                EditorGUILayout.LabelField("Not Found", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawComponentsTab()
        {
            EditorGUILayout.LabelField("Registered Components", EditorStyles.boldLabel);
            
            var uiManager = FindObjectOfType<UIThemeManager>();
            var envManager = FindObjectOfType<EnvironmentThemeManager>();
            var audioManager = FindObjectOfType<AudioThemeManager>();
            var charManager = FindObjectOfType<CharacterThemeManager>();
            
            DrawComponentList("UI Components", uiManager);
            DrawComponentList("Environment Components", envManager);
            DrawComponentList("Audio Components", audioManager);
            DrawComponentList("Character Components", charManager);
        }
        
        private void DrawComponentList(string title, BaseThemeManager manager)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            
            if (manager != null)
            {
                var components = manager.GetRegisteredComponents();
                
                if (components.Count > 0)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    foreach (var component in components)
                    {
                        if (component != null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            
                            EditorGUILayout.ObjectField(component as Object, typeof(Object), true);
                            
                            if (GUILayout.Button("Focus", GUILayout.Width(60)))
                            {
                                Selection.activeObject = component as Object;
                                EditorGUIUtility.PingObject(component as Object);
                            }
                            
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.HelpBox("No components registered", MessageType.Info);
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"{title} manager not found", MessageType.Warning);
            }
            
            EditorGUILayout.Space();
        }
        
        private void DrawPresetsTab()
        {
            EditorGUILayout.LabelField("Theme Presets", EditorStyles.boldLabel);
            
            var availablePresets = themeController.GetAvailablePresetNames();
            
            if (availablePresets.Length > 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                foreach (var presetName in availablePresets)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUILayout.LabelField(presetName, EditorStyles.miniLabel);
                    
                    if (GUILayout.Button("Apply", GUILayout.Width(60)))
                    {
                        var preset = themeController.GetPresetByName(presetName);
                        if (preset != null)
                        {
                            themeController.ApplyPreset(preset);
                        }
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("No presets available. Create theme presets in the ThemeController inspector.", MessageType.Info);
            }
        }
        
        private void DrawSettingsTab()
        {
            EditorGUILayout.LabelField("Theme System Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("Debug Mode", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Enabled: {themeController.DebugMode}", EditorStyles.miniLabel);
            
            EditorGUILayout.LabelField("Theme Transitions", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Enabled: {themeController.EnableThemeTransitions}", EditorStyles.miniLabel);
            
            EditorGUILayout.LabelField("Transition Duration", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Duration: {themeController.TransitionDuration}s", EditorStyles.miniLabel);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("System Information", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("Version: 1.0.0", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("Unity Version: " + Application.unityVersion, EditorStyles.miniLabel);
            EditorGUILayout.LabelField("Platform: " + Application.platform, EditorStyles.miniLabel);
            
            EditorGUILayout.EndVertical();
        }
    }
}
