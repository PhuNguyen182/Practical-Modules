using PracticalSystems.AudioSystem.Data;
using PracticalSystems.AudioSystem.Interfaces;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Core
{
    /// <summary>
    /// Component that plays audio using Unity AudioSource
    /// Poolable audio player with full playback control
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour, IAudioPlayer
    {
        private AudioSource _audioSource;
        private Transform _attachedTransform;
        private bool _isPaused;
        
        public AudioSource AudioSource => this._audioSource;
        public Transform Transform => this.transform;
        public bool IsPlaying => this._audioSource != null && this._audioSource.isPlaying && !this._isPaused;
        public bool IsPaused => this._isPaused;
        
        private void Awake()
        {
            this._audioSource = this.GetComponent<AudioSource>();
            if (this._audioSource == null)
            {
                this._audioSource = this.gameObject.AddComponent<AudioSource>();
            }
            
            this._audioSource.playOnAwake = false;
        }
        
        public void Play(IAudioEntry audioEntry, AudioPlaybackParameters parameters)
        {
            if (audioEntry == null)
            {
                Debug.LogWarning("AudioPlayer: Cannot play null audio entry");
                return;
            }
            
            if (audioEntry.AudioClip == null)
            {
                Debug.LogWarning($"AudioPlayer: Audio entry '{audioEntry.AudioId}' has null AudioClip");
                return;
            }
            
            
            this._isPaused = false;
            this.ConfigureAudioSource(audioEntry, parameters);
            
            if (parameters.delay > 0f)
            {
                this._audioSource.PlayDelayed(parameters.delay);
            }
            else
            {
                this._audioSource.Play();
            }
        }
        
        public void Stop()
        {
            if (this._audioSource != null)
            {
                this._audioSource.Stop();
            }
            
            this._isPaused = false;
            this.DetachFromTransform();
        }
        
        public void Pause()
        {
            if (this._audioSource != null && this._audioSource.isPlaying)
            {
                this._audioSource.Pause();
                this._isPaused = true;
            }
        }
        
        public void Resume()
        {
            if (this._audioSource != null && this._isPaused)
            {
                this._audioSource.UnPause();
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
            
            if (this._audioSource != null)
            {
                this._audioSource.clip = null;
                this._audioSource.volume = 1f;
                this._audioSource.pitch = 1f;
                this._audioSource.loop = false;
                this._audioSource.spatialBlend = 0f;
            }
        }
        
        /// <summary>
        /// Configures the AudioSource with entry and parameter settings
        /// </summary>
        private void ConfigureAudioSource(IAudioEntry audioEntry, AudioPlaybackParameters parameters)
        {
            this._audioSource.clip = audioEntry.AudioClip;
            this._audioSource.priority = audioEntry.Priority;
            
            // Volume with random variation
            var finalVolume = audioEntry.DefaultVolume * parameters.volumeMultiplier;
            if (parameters.useRandomVolume)
            {
                finalVolume += Random.Range(-parameters.randomVolumeRange, parameters.randomVolumeRange);
            }
            this._audioSource.volume = Mathf.Clamp01(finalVolume);
            
            // Pitch with random variation
            var finalPitch = audioEntry.DefaultPitch * parameters.pitchMultiplier;
            if (parameters.useRandomPitch)
            {
                finalPitch += Random.Range(-parameters.randomPitchRange, parameters.randomPitchRange);
            }
            this._audioSource.pitch = Mathf.Clamp(finalPitch, 0.1f, 3f);
            
            // Loop setting
            if (parameters.overrideLoop)
            {
                this._audioSource.loop = parameters.loop;
            }
            else
            {
                this._audioSource.loop = audioEntry.IsLooping;
            }
            
            // Spatial blend
            if (parameters.overrideSpatialBlend)
            {
                this._audioSource.spatialBlend = Mathf.Clamp01(parameters.spatialBlend);
            }
            else
            {
                this._audioSource.spatialBlend = Mathf.Clamp01(audioEntry.SpatialBlend);
            }
            
            // 3D audio settings
            this._audioSource.minDistance = audioEntry.MinDistance;
            this._audioSource.maxDistance = audioEntry.MaxDistance;
            this._audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
    }
}

