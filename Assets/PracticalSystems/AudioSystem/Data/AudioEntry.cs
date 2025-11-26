using System;
using PracticalSystems.AudioSystem.Interfaces;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Data
{
    /// <summary>
    /// Represents a single audio entry with all configuration data
    /// Serializable for Unity Inspector and ScriptableObject
    /// </summary>
    [Serializable]
    public class AudioEntry : IAudioEntry
    {
        [SerializeField] private string audioId;
        [SerializeField] private AudioKind audioKind;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private float defaultVolume = 1f;
        [SerializeField] private float defaultPitch = 1f;
        [SerializeField] private bool isLooping;
        [SerializeField] private int priority = 128;
        [SerializeField] private float minTimeBetweenPlays;
        [SerializeField] private int maxConcurrentInstances = 5;
        [SerializeField] private float spatialBlend;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 500f;
        
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
    }
}

