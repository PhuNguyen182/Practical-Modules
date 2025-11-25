using PracticalSystems.AudioSystem.Data;
using PracticalSystems.AudioSystem.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PracticalSystems.AudioSystem.Addressables
{
    /// <summary>
    /// Audio entry that loads clips via Addressables
    /// Optimized for memory efficiency with large audio libraries
    /// </summary>
    [System.Serializable]
    public class AddressableAudioEntry : IAudioEntry
    {
        [SerializeField] private string audioId;
        [SerializeField] private AudioKind audioKind;
        [SerializeField] private AssetReferenceT<AudioClip> audioClipReference;
        [SerializeField] private float defaultVolume = 1f;
        [SerializeField] private float defaultPitch = 1f;
        [SerializeField] private bool isLooping;
        [SerializeField] private int priority = 128;
        [SerializeField] private float minTimeBetweenPlays;
        [SerializeField] private int maxConcurrentInstances = 5;
        [SerializeField] private float spatialBlend;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 500f;
        
        private AudioClip _loadedClip;
        
        public string AudioId => this.audioId;
        public AudioKind AudioKind => this.audioKind;
        public AudioClip AudioClip => this._loadedClip;
        public float DefaultVolume => this.defaultVolume;
        public float DefaultPitch => this.defaultPitch;
        public bool IsLooping => this.isLooping;
        public int Priority => this.priority;
        public float MinTimeBetweenPlays => this.minTimeBetweenPlays;
        public int MaxConcurrentInstances => this.maxConcurrentInstances;
        public float SpatialBlend => this.spatialBlend;
        public float MinDistance => this.minDistance;
        public float MaxDistance => this.maxDistance;
        
        public AssetReferenceT<AudioClip> AudioClipReference => this.audioClipReference;
        public bool IsLoaded => this._loadedClip != null;
        
        /// <summary>
        /// Sets the loaded audio clip (called by AddressableAudioDatabase)
        /// </summary>
        public void SetLoadedClip(AudioClip clip)
        {
            this._loadedClip = clip;
        }
    }
}

