#if UNITY_EDITOR
using PracticalSystems.AudioSystem.Core;
using PracticalSystems.AudioSystem.Data;
using UnityEditor;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Editor
{
    /// <summary>
    /// Custom editor utilities for the audio system
    /// Provides helpful inspector tools and validation
    /// </summary>
    [CustomEditor(typeof(AudioDatabaseConfig))]
    public class AudioDatabaseConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty _audioEntriesProperty;
        private SerializedProperty _useAddressablesProperty;
        private SerializedProperty _preloadOnStartProperty;
        private SerializedProperty _initialPoolSizeProperty;
        
        private void OnEnable()
        {
            this._audioEntriesProperty = this.serializedObject.FindProperty("audioEntries");
            this._useAddressablesProperty = this.serializedObject.FindProperty("useAddressables");
            this._preloadOnStartProperty = this.serializedObject.FindProperty("preloadOnStart");
            this._initialPoolSizeProperty = this.serializedObject.FindProperty("initialPoolSize");
        }
        
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Audio Database Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Database settings
            EditorGUILayout.PropertyField(this._useAddressablesProperty);
            EditorGUILayout.PropertyField(this._preloadOnStartProperty);
            EditorGUILayout.PropertyField(this._initialPoolSizeProperty);
            
            EditorGUILayout.Space();
            
            // Audio entries
            EditorGUILayout.PropertyField(this._audioEntriesProperty);
            
            // Statistics
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);
            var config = this.target as AudioDatabaseConfig;
            if (config != null)
            {
                EditorGUILayout.LabelField($"Total Entries: {config.AudioEntries.Count}");
                
                // Count by type
                var typeCounts = new System.Collections.Generic.Dictionary<AudioKind, int>();
                foreach (var entry in config.AudioEntries)
                {
                    if (entry != null)
                    {
                        if (!typeCounts.ContainsKey(entry.AudioKind))
                        {
                            typeCounts[entry.AudioKind] = 0;
                        }
                        typeCounts[entry.AudioKind]++;
                    }
                }
                
                EditorGUILayout.Space();
                foreach (var kvp in typeCounts)
                {
                    EditorGUILayout.LabelField($"{kvp.Key}: {kvp.Value} entries");
                }
            }
            
            EditorGUILayout.Space();
            
            // Validation button
            if (GUILayout.Button("Validate Database"))
            {
                this.ValidateDatabase();
            }
            
            this.serializedObject.ApplyModifiedProperties();
        }
        
        private void ValidateDatabase()
        {
            var config = this.target as AudioDatabaseConfig;
            if (config == null)
            {
                return;
            }
            
            var issues = new System.Collections.Generic.List<string>();
            var ids = new System.Collections.Generic.HashSet<string>();
            
            foreach (var entry in config.AudioEntries)
            {
                if (entry == null)
                {
                    issues.Add("Found null entry in database");
                    continue;
                }
                
                if (string.IsNullOrEmpty(entry.AudioId))
                {
                    issues.Add("Found entry with empty Audio ID");
                    continue;
                }
                
                if (!ids.Add(entry.AudioId))
                {
                    issues.Add($"Duplicate Audio ID: {entry.AudioId}");
                }
                
                if (entry.AudioClip == null)
                {
                    issues.Add($"Entry '{entry.AudioId}' has no AudioClip assigned");
                }
                
                if (entry.DefaultVolume < 0f || entry.DefaultVolume > 1f)
                {
                    issues.Add($"Entry '{entry.AudioId}' has invalid volume: {entry.DefaultVolume}");
                }
                
                if (entry.DefaultPitch < 0.1f || entry.DefaultPitch > 3f)
                {
                    issues.Add($"Entry '{entry.AudioId}' has invalid pitch: {entry.DefaultPitch}");
                }
            }
            
            if (issues.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    "Validation Passed",
                    "Audio database validation completed successfully!",
                    "OK"
                );
            }
            else
            {
                var message = "Found the following issues:\n\n" + string.Join("\n", issues);
                EditorUtility.DisplayDialog(
                    "Validation Failed",
                    message,
                    "OK"
                );
            }
        }
    }
    
    /// <summary>
    /// Custom editor for AudioMixerConfig
    /// </summary>
    [CustomEditor(typeof(AudioMixerConfig))]
    public class AudioMixerConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Audio Mixer Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Help box
            EditorGUILayout.HelpBox(
                "Configure mixer groups and exposed parameters for each audio type. " +
                "Make sure to expose volume parameters in your AudioMixer asset.",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            DrawDefaultInspector();
            
            // Validation button
            EditorGUILayout.Space();
            if (GUILayout.Button("Validate Mixer Setup"))
            {
                this.ValidateMixerSetup();
            }
            
            this.serializedObject.ApplyModifiedProperties();
        }
        
        private void ValidateMixerSetup()
        {
            var config = this.target as AudioMixerConfig;
            if (config == null)
            {
                return;
            }
            
            var issues = new System.Collections.Generic.List<string>();
            
            if (config.AudioMixer == null)
            {
                issues.Add("AudioMixer is not assigned");
            }
            
            if (string.IsNullOrEmpty(config.MasterVolumeParameter))
            {
                issues.Add("Master volume parameter name is empty");
            }
            
            foreach (var mapping in config.AudioTypeMappings)
            {
                if (mapping.mixerGroup == null)
                {
                    issues.Add($"Mixer group not assigned for {mapping.audioKind}");
                }
                
                if (string.IsNullOrEmpty(mapping.volumeParameterName))
                {
                    issues.Add($"Volume parameter name empty for {mapping.audioKind}");
                }
            }
            
            if (issues.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    "Validation Passed",
                    "Audio mixer configuration is valid!",
                    "OK"
                );
            }
            else
            {
                var message = "Found the following issues:\n\n" + string.Join("\n", issues);
                EditorUtility.DisplayDialog(
                    "Validation Failed",
                    message,
                    "OK"
                );
            }
        }
    }
    
    /// <summary>
    /// Menu items for creating audio system assets
    /// </summary>
    public static class AudioSystemMenuItems
    {
        [MenuItem("GameObject/Foundations/Audio System/Create Audio Manager", false, 10)]
        private static void CreateAudioManager(MenuCommand menuCommand)
        {
            var audioManager = new GameObject("AudioManager");
            audioManager.AddComponent<AudioManager>();
            
            GameObjectUtility.SetParentAndAlign(audioManager, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(audioManager, "Create Audio Manager");
            Selection.activeObject = audioManager;
            
            EditorUtility.DisplayDialog(
                "Audio Manager Created",
                "Audio Manager created successfully!\n\n" +
                "Please assign AudioDatabaseConfig and AudioMixerConfig in the inspector.",
                "OK"
            );
        }
        
        [MenuItem("GameObject/Foundations/Audio System/Create Audio Player Prefab", false, 11)]
        private static void CreateAudioPlayerPrefab(MenuCommand menuCommand)
        {
            var audioPlayer = new GameObject("AudioPlayer");
            audioPlayer.AddComponent<AudioSource>();
            audioPlayer.AddComponent<AudioPlayer>();
            
            var audioSource = audioPlayer.GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            
            GameObjectUtility.SetParentAndAlign(audioPlayer, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(audioPlayer, "Create Audio Player");
            Selection.activeObject = audioPlayer;
            
            EditorUtility.DisplayDialog(
                "Audio Player Created",
                "Audio Player prefab created successfully!\n\n" +
                "You can now save this as a prefab and assign it to the Audio Manager.",
                "OK"
            );
        }
    }
}
#endif

