using UnityEngine;
using PracticalSystems.AudioSystem.Core;
using PracticalSystems.AudioSystem.Data;

namespace Foundations.Audio.Utilities
{
    /// <summary>
    /// Utility class for setting up the audio system in a scene
    /// </summary>
    public static class AudioSystemSetup
    {
        /// <summary>
        /// Creates a complete audio system setup in the current scene
        /// </summary>
        /// <param name="databaseConfig">Audio database configuration</param>
        /// <param name="mixerConfig">Audio mixer configuration</param>
        /// <param name="audioPlayerPrefab">Audio player prefab (optional)</param>
        /// <returns>The created AudioManager GameObject</returns>
        public static GameObject CreateAudioSystem(
            AudioDatabaseConfig databaseConfig,
            AudioMixerConfig mixerConfig,
            GameObject audioPlayerPrefab = null)
        {
            if (databaseConfig == null)
            {
                Debug.LogError("AudioSystemSetup: AudioDatabaseConfig is required");
                return null;
            }
            
            if (mixerConfig == null)
            {
                Debug.LogError("AudioSystemSetup: AudioMixerConfig is required");
                return null;
            }
            
            // Create audio manager GameObject
            var audioManagerObject = new GameObject("AudioManager");
            var audioManager = audioManagerObject.AddComponent<AudioManager>();
            
            // Create audio player prefab if not provided
            if (audioPlayerPrefab == null)
            {
                audioPlayerPrefab = CreateDefaultAudioPlayerPrefab();
            }
            
            // This would be done through serialized fields in the inspector
            // In code, you'd need to use reflection or make the fields public
            Debug.Log("AudioSystemSetup: Audio system created. Please assign configs in the inspector.");
            
            return audioManagerObject;
        }
        
        /// <summary>
        /// Creates a default audio player prefab
        /// </summary>
        private static GameObject CreateDefaultAudioPlayerPrefab()
        {
            var prefab = new GameObject("AudioPlayer");
            var audioSource = prefab.AddComponent<AudioSource>();
            var audioPlayer = prefab.AddComponent<AudioPlayer>();
            
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            
            return prefab;
        }
        
        /// <summary>
        /// Validates audio system setup
        /// </summary>
        public static bool ValidateAudioSystem(GameObject audioManagerObject)
        {
            if (audioManagerObject == null)
            {
                Debug.LogError("AudioSystemSetup: Audio manager object is null");
                return false;
            }
            
            var audioManager = audioManagerObject.GetComponent<AudioManager>();
            if (audioManager == null)
            {
                Debug.LogError("AudioSystemSetup: AudioManager component not found");
                return false;
            }
            
            // Additional validation could be added here
            Debug.Log("AudioSystemSetup: Audio system validation passed");
            return true;
        }
    }
}

