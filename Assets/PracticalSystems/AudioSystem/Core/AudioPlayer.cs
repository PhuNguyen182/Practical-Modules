using PracticalSystems.AudioSystem.Data;
using PracticalSystems.AudioSystem.Interfaces;
using PracticalSystems.AudioSystem.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PracticalSystems.AudioSystem.Core
{
    /// <summary>
    /// Component that plays audio using Unity AudioSource
    /// Poolable audio player with full playback control
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour, IAudioPlayer
    {
        [SerializeField] private AudioSource audioSource;
        private Transform _attachedTransform;
        private bool _isPaused;
        
        public AudioSource AudioSource => this.audioSource;
        public Transform Transform => this.transform;
        public bool IsPlaying => this.audioSource != null && this.audioSource.isPlaying && !this._isPaused;
        public bool IsPaused => this._isPaused;
        
        private void Awake()
        {
            if (!this.TryGetComponent(out this.audioSource))
            {
                if (!this.audioSource)
                {
                    this.audioSource = this.gameObject.AddComponent<AudioSource>();
                }
            }

            this.audioSource.playOnAwake = false;
        }
        
        public void Play(IAudioEntry audioEntry, AudioPlaybackParameters parameters)
        {
            if (audioEntry == null)
            {
                Debug.LogWarning("AudioPlayer: Cannot play null audio entry");
                return;
            }
            
            if (!audioEntry.AudioClip)
            {
                Debug.LogWarning($"AudioPlayer: Audio entry '{audioEntry.AudioId}' has null AudioClip");
                return;
            }
            
            
            this._isPaused = false;
            this.ConfigureAudioSource(audioEntry, parameters);
            
            if (parameters.delay > 0f)
            {
                this.audioSource.PlayDelayed(parameters.delay);
            }
            else
            {
                this.audioSource.Play();
            }
        }
        
        public void Stop()
        {
            if (this.audioSource != null)
            {
                this.audioSource.Stop();
            }
            
            this._isPaused = false;
            this.DetachFromTransform();
        }
        
        public void Pause()
        {
            if (this.audioSource != null && this.audioSource.isPlaying)
            {
                this.audioSource.Pause();
                this._isPaused = true;
            }
        }
        
        public void Resume()
        {
            if (this.audioSource != null && this._isPaused)
            {
                this.audioSource.UnPause();
                this._isPaused = false;
            }
        }
        
        public void SetPosition(Vector3 position)
        {
            this.transform.position = position;
        }
        
        public void AttachToTransform(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("AudioPlayer: Cannot attach to null transform");
                return;
            }
            
            this._attachedTransform = target;
            this.transform.position = target.position;
        }
        
        public void DetachFromTransform()
        {
            this._attachedTransform = null;
        }
        
        public void Reset()
        {
            this.Stop();
            this._attachedTransform = null;
            this._isPaused = false;
            
            if (this.audioSource != null)
            {
                this.audioSource.clip = null;
                this.audioSource.volume = 1f;
                this.audioSource.pitch = 1f;
                this.audioSource.loop = false;
                this.audioSource.spatialBlend = 0f;
            }
        }

        /// <summary>
        /// Configures the AudioSource with entry and parameter settings
        /// </summary>
        private void ConfigureAudioSource(IAudioEntry audioEntry, AudioPlaybackParameters parameters)
        {
            this.audioSource.clip = audioEntry.AudioClip;
            this.audioSource.priority = audioEntry.Priority;

            // Volume with random variation
            var finalVolume = audioEntry.DefaultVolume * parameters.volumeMultiplier;
            if (parameters.useRandomVolume)
            {
                finalVolume += Random.Range(-parameters.randomVolumeRange, parameters.randomVolumeRange);
            }

            this.audioSource.volume = Mathf.Clamp01(finalVolume);

            // Pitch with random variation
            var finalPitch = audioEntry.DefaultPitch * parameters.pitchMultiplier;
            if (parameters.useRandomPitch)
            {
                finalPitch += Random.Range(-parameters.randomPitchRange, parameters.randomPitchRange);
            }

            this.audioSource.pitch = Mathf.Clamp(finalPitch, AudioConstants.MinPitch, AudioConstants.MaxPitch);

            // Loop setting
            this.audioSource.loop = parameters.overrideLoop ? parameters.loop : audioEntry.IsLooping;

            // Spatial blend
            this.audioSource.spatialBlend =
                Mathf.Clamp01(parameters.overrideSpatialBlend ? parameters.spatialBlend : audioEntry.SpatialBlend);

            // 3D audio settings
            this.audioSource.minDistance = audioEntry.MinDistance;
            this.audioSource.maxDistance = audioEntry.MaxDistance;
            this.audioSource.rolloffMode = AudioRolloffMode.Linear;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!this.audioSource)
                this.audioSource = this.GetComponent<AudioSource>();
        }
#endif
    }
}

