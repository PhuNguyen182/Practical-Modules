using UnityEngine;
using UnityEditor;
using System.Linq;
using PracticalSystems.ThemeSystem.Core;
using PracticalSystems.ThemeSystem.Managers;
using PracticalSystems.ThemeSystem.Themes;

namespace PracticalSystems.ThemeSystem.Editor
{
    /// <summary>
    /// Custom editor for theme managers
    /// </summary>
    [CustomEditor(typeof(BaseThemeManager), true)]
    public class ThemeManagerEditor : UnityEditor.Editor
    {
        private BaseThemeManager themeManager;
        private bool showComponents = false;
        private bool showThemes = false;
        
        private void OnEnable()
        {
            themeManager = (BaseThemeManager)target;
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // Draw default inspector
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            
            // Draw custom controls
            DrawCustomControls();
            
            // Draw components list
            if (showComponents)
            {
                DrawComponentsList();
            }
            
            // Draw themes list
            if (showThemes)
            {
                DrawThemesList();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawCustomControls()
        {
            EditorGUILayout.LabelField($"{themeManager.Category} Theme Manager Tools", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Auto-Find Components"))
            {
                if (themeManager is UIThemeManager uiManager)
                    uiManager.AutoFindUIThemeComponents();
                else if (themeManager is EnvironmentThemeManager envManager)
                    envManager.AutoFindEnvironmentThemeComponents();
                else if (themeManager is AudioThemeManager audioManager)
                    audioManager.AutoFindAudioThemeComponents();
                else if (themeManager is CharacterThemeManager charManager)
                    charManager.AutoFindCharacterThemeComponents();
            }
            
            if (GUILayout.Button("Refresh Current Theme"))
            {
                themeManager.RefreshCurrentTheme();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Validate Components"))
            {
                bool isValid = ValidateComponents();
                Debug.Log($"[{themeManager.Category} Theme Manager] Components validation: {(isValid ? "PASSED" : "FAILED")}");
            }
            
            if (GUILayout.Button("Clear Components"))
            {
                if (EditorUtility.DisplayDialog("Clear Components", "Are you sure you want to clear all registered components?", "Yes", "No"))
                {
                    themeManager.ClearRegisteredComponents();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Toggle options
            EditorGUILayout.BeginHorizontal();
            
            showComponents = EditorGUILayout.Toggle("Show Components", showComponents);
            showThemes = EditorGUILayout.Toggle("Show Themes", showThemes);
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawComponentsList()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Registered Components", EditorStyles.boldLabel);
            
            var components = themeManager.GetRegisteredComponents();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (components.Count > 0)
            {
                EditorGUILayout.LabelField($"Total: {components.Count}", EditorStyles.miniLabel);
                
                foreach (var component in components)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    if (component != null)
                    {
                        EditorGUILayout.ObjectField(component as Object, typeof(Object), true);
                        
                        if (GUILayout.Button("Focus", GUILayout.Width(60)))
                        {
                            Selection.activeObject = component as Object;
                            EditorGUIUtility.PingObject(component as Object);
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("NULL", EditorStyles.miniLabel);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField("No components registered", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawThemesList()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Available Themes", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (themeManager is UIThemeManager uiManager)
            {
                DrawUIThemes(uiManager);
            }
            else if (themeManager is EnvironmentThemeManager envManager)
            {
                DrawEnvironmentThemes(envManager);
            }
            else if (themeManager is AudioThemeManager audioManager)
            {
                DrawAudioThemes(audioManager);
            }
            else if (themeManager is CharacterThemeManager charManager)
            {
                DrawCharacterThemes(charManager);
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawUIThemes(UIThemeManager uiManager)
        {
            var themes = uiManager.AvailableThemes;
            if (themes != null && themes.Length > 0)
            {
                EditorGUILayout.LabelField($"Total: {themes.Length}", EditorStyles.miniLabel);
                
                foreach (var theme in themes)
                {
                    if (theme != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        EditorGUILayout.ObjectField(theme, typeof(UITheme), true);
                        
                        if (GUILayout.Button("Apply", GUILayout.Width(60)))
                        {
                            uiManager.ApplyTheme(theme);
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No themes available", EditorStyles.miniLabel);
            }
        }
        
        private void DrawEnvironmentThemes(EnvironmentThemeManager envManager)
        {
            var themes = envManager.AvailableThemes;
            if (themes != null && themes.Length > 0)
            {
                EditorGUILayout.LabelField($"Total: {themes.Length}", EditorStyles.miniLabel);
                
                foreach (var theme in themes)
                {
                    if (theme != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        EditorGUILayout.ObjectField(theme, typeof(EnvironmentTheme), true);
                        
                        if (GUILayout.Button("Apply", GUILayout.Width(60)))
                        {
                            envManager.ApplyTheme(theme);
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No themes available", EditorStyles.miniLabel);
            }
        }
        
        private void DrawAudioThemes(AudioThemeManager audioManager)
        {
            var themes = audioManager.AvailableThemes;
            if (themes != null && themes.Length > 0)
            {
                EditorGUILayout.LabelField($"Total: {themes.Length}", EditorStyles.miniLabel);
                
                foreach (var theme in themes)
                {
                    if (theme != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        EditorGUILayout.ObjectField(theme, typeof(AudioTheme), true);
                        
                        if (GUILayout.Button("Apply", GUILayout.Width(60)))
                        {
                            audioManager.ApplyTheme(theme);
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No themes available", EditorStyles.miniLabel);
            }
        }
        
        private void DrawCharacterThemes(CharacterThemeManager charManager)
        {
            var themes = charManager.AvailableThemes;
            if (themes != null && themes.Length > 0)
            {
                EditorGUILayout.LabelField($"Total: {themes.Length}", EditorStyles.miniLabel);
                
                foreach (var theme in themes)
                {
                    if (theme != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        EditorGUILayout.ObjectField(theme, typeof(CharacterTheme), true);
                        
                        if (GUILayout.Button("Apply", GUILayout.Width(60)))
                        {
                            charManager.ApplyTheme(theme);
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No themes available", EditorStyles.miniLabel);
            }
        }
        
        private bool ValidateComponents()
        {
            if (themeManager is UIThemeManager uiManager)
                return uiManager.ValidateUIComponents();
            else if (themeManager is EnvironmentThemeManager envManager)
                return envManager.ValidateEnvironmentComponents();
            else if (themeManager is AudioThemeManager audioManager)
                return audioManager.ValidateAudioComponents();
            else if (themeManager is CharacterThemeManager charManager)
                return charManager.ValidateCharacterComponents();
                
            return true;
        }
    }
}
