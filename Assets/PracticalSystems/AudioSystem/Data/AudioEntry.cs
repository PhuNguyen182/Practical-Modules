using PracticalSystems.AudioSystem.Interfaces;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Data
{
    /// <summary>
    /// Represents a single audio entry with all configuration data
    /// Serializable for Unity Inspector and ScriptableObject
    /// </summary>
    [System.Serializable]
    public class AudioEntry : IAudioEntry
    {
        public string audioId;
        public AudioKind audioKind;
        public AudioClip audioClip;
        public float defaultVolume = 1f;
        public float defaultPitch = 1f;
        public bool isLooping = false;
        public int priority = 128;
        public float minTimeBetweenPlays = 0f;
        public int maxConcurrentInstances = 5;
        public float spatialBlend = 0f;
        public float minDistance = 1f;
        public float maxDistance = 500f;
        
        public string AudioId => this.audioId;
        public AudioKind AudioKind => this.audioKind;
        public AudioClip AudioClip => this.audioClip;
        public float DefaultVolume => this.defaultVolume;
        public float DefaultPitch => this.defaultPitch;
        public bool IsLooping => this.isLooping;
        public int Priority => this.priority;
        public float MinTimeBetweenPlays => this.minTimeBetweenPlays;
        public int MaxConcurrentInstances => this.maxConcurrentInstances;
        public float SpatialBlend => this.spatialBlend;
        public float MinDistance => this.minDistance;
        public float MaxDistance => this.maxDistance;
        
        /// <summary>
        /// Creates a new audio entry with specified parameters
        /// </summary>
        public AudioEntry(
            string audioId,
            AudioKind audioKind,
            AudioClip audioClip,
            float defaultVolume = 1f,
            float defaultPitch = 1f,
            bool isLooping = false,
            int priority = 128,
            float minTimeBetweenPlays = 0f,
            int maxConcurrentInstances = 5,
            float spatialBlend = 0f,
            float minDistance = 1f,
            float maxDistance = 500f)
        {
            this.audioId = audioId;
            this.audioKind = audioKind;
            this.audioClip = audioClip;
            this.defaultVolume = defaultVolume;
            this.defaultPitch = defaultPitch;
            this.isLooping = isLooping;
            this.priority = priority;
            this.minTimeBetweenPlays = minTimeBetweenPlays;
            this.maxConcurrentInstances = maxConcurrentInstances;
            this.spatialBlend = spatialBlend;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
        }
    }
}

