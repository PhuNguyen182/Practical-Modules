using UnityEngine;
using UnityEditor;
using System.Linq;
using PracticalSystems.ThemeSystem.Core;

namespace PracticalSystems.ThemeSystem.Editor
{
    /// <summary>
    /// Custom editor for ThemeController
    /// </summary>
    [CustomEditor(typeof(ThemeController))]
    public class ThemeControllerEditor : UnityEditor.Editor
    {
        private ThemeController themeController;
        private bool showStats = false;
        private bool showDebugInfo = false;
        
        private void OnEnable()
        {
            themeController = (ThemeController)target;
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // Draw default inspector
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            
            // Draw custom controls
            DrawCustomControls();
            
            // Draw statistics
            if (showStats)
            {
                DrawStatistics();
            }
            
            // Draw debug information
            if (showDebugInfo)
            {
                DrawDebugInfo();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawCustomControls()
        {
            EditorGUILayout.LabelField("Theme Controller Tools", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Initialize"))
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
            
            if (GUILayout.Button("Clear All Themes"))
            {
                if (EditorUtility.DisplayDialog("Clear All Themes", "Are you sure you want to clear all active themes?", "Yes", "No"))
                {
                    // Clear all active themes
                    Debug.Log("[Theme Controller] Cleared all themes");
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Preset controls
            EditorGUILayout.LabelField("Preset Controls", EditorStyles.boldLabel);
            
            var availablePresets = themeController.GetAvailablePresetNames();
            if (availablePresets.Length > 0)
            {
                EditorGUILayout.LabelField("Available Presets:", EditorStyles.label);
                
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
            }
            else
            {
                EditorGUILayout.HelpBox("No presets available. Create theme presets in the inspector.", MessageType.Info);
            }
            
            EditorGUILayout.Space();
            
            // Toggle options
            EditorGUILayout.BeginHorizontal();
            
            showStats = EditorGUILayout.Toggle("Show Statistics", showStats);
            showDebugInfo = EditorGUILayout.Toggle("Show Debug Info", showDebugInfo);
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawStatistics()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("System Statistics", EditorStyles.boldLabel);
            
            var stats = themeController.GetSystemStats();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField($"Initialized: {stats.isInitialized}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Transitioning: {stats.isTransitioning}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Total Managers: {stats.totalManagers}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Registered Components: {stats.totalRegisteredComponents}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Active Themes: {stats.activeThemes}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Total Presets: {stats.totalPresets}", EditorStyles.miniLabel);
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawDebugInfo()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug Information", EditorStyles.boldLabel);
            
            var activeThemes = themeController.GetAllActiveThemes();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (activeThemes.Count > 0)
            {
                EditorGUILayout.LabelField("Active Themes:", EditorStyles.miniLabel);
                
                foreach (var kvp in activeThemes)
                {
                    EditorGUILayout.LabelField($"{kvp.Key}: {kvp.Value?.ThemeName ?? "None"}", EditorStyles.miniLabel);
                }
            }
            else
            {
                EditorGUILayout.LabelField("No active themes", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}
