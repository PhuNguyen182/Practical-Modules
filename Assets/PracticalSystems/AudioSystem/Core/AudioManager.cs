using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PracticalSystems.AudioSystem.Data;
using PracticalSystems.AudioSystem.Interfaces;
using PracticalSystems.AudioSystem.Utilities;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Core
{
    /// <summary>
    /// Main audio manager implementation
    /// Provides comprehensive audio management with pooling, frequency control, and fade support
    /// </summary>
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        [SerializeField] private AudioDatabaseConfig audioDatabaseConfig;
        [SerializeField] private AudioMixerConfig audioMixerConfig;
        [SerializeField] private GameObject audioPlayerPrefab;
        
        private IAudioDatabase _audioDatabase;
        private IAudioMixerController _mixerController;
        private IAudioPlayerPool _playerPool;
        private AudioFrequencyController _frequencyController;
        private readonly List<IAudioHandle> _activeHandles = new();
        private readonly Dictionary<AudioKind, List<IAudioHandle>> _handlesByType = new();
        private Transform _poolParent;
        
        private void Awake()
        {
            this.InitializeAudioSystem();
        }
        
        /// <summary>
        /// Initializes all audio system components
        /// </summary>
        private void InitializeAudioSystem()
        {
            // Validate configuration
            if (this.audioDatabaseConfig == null)
            {
                Debug.LogError("AudioManager: AudioDatabaseConfig is not assigned");
                return;
            }
            
            if (this.audioMixerConfig == null)
            {
                Debug.LogError("AudioManager: AudioMixerConfig is not assigned");
                return;
            }
            
            // Create pool parent
            this._poolParent = new GameObject("AudioPlayerPool").transform;
            this._poolParent.SetParent(this.transform);
            
            // Initialize components
            this._audioDatabase = new AudioDatabase(this.audioDatabaseConfig);
            this._mixerController = new AudioMixerController(this.audioMixerConfig);
            this._playerPool = new AudioPlayerPool(
                this.audioPlayerPrefab,
                this._poolParent,
                this._mixerController,
                this.audioDatabaseConfig.InitialPoolSize
            );
            
            this._frequencyController = new AudioFrequencyController();
            foreach (AudioKind audioType in Enum.GetValues(typeof(AudioKind)))
            {
                this._handlesByType[audioType] = new List<IAudioHandle>();
            }
            
            Debug.Log("AudioManager: Initialized successfully");
        }
        
        public async UniTask<IAudioHandle> PlayAudioAsync(
            string audioId, 
            AudioPlaybackParameters parameters = default,
            CancellationToken cancellationToken = default)
        {
            // Load audio entry
            var audioEntry = await this._audioDatabase.LoadAudioEntryAsync(audioId, cancellationToken);
            if (audioEntry == null)
            {
                Debug.LogWarning($"AudioManager: Audio entry '{audioId}' not found");
                return null;
            }
            
            // Check frequency limits
            if (!this._frequencyController.CanPlayAudio(audioEntry))
            {
                return null;
            }
            
            // Get player from pool
            var audioPlayer = this._playerPool.GetAudioPlayer();
            if (audioPlayer == null)
            {
                Debug.LogWarning("AudioManager: Failed to get audio player from pool");
                return null;
            }
            
            // Configure mixer group
            if (audioPlayer.AudioSource != null)
            {
                var mixerGroup = this._mixerController.GetMixerGroup(audioEntry.AudioKind);
                audioPlayer.AudioSource.outputAudioMixerGroup = mixerGroup;
            }
            
            // Apply fade in if specified
            if (parameters.fadeInDuration > 0f)
            {
                var originalVolume = audioEntry.DefaultVolume * parameters.volumeMultiplier;
                parameters.volumeMultiplier = 0f; // Start at 0 for fade in
                audioPlayer.SetLifeTimeDuration(audioEntry.AudioClip.length);
                audioPlayer.Play(audioEntry, parameters);
                
                var handle = new AudioHandle(audioPlayer, this._playerPool, audioEntry);
                this.RegisterHandle(handle);
                this._frequencyController.RegisterPlayback(audioEntry, handle);
                
                // Fade in asynchronously
                await handle.FadeVolumeAsync(originalVolume, parameters.fadeInDuration);
                
                return handle;
            }
            else
            {
                audioPlayer.SetLifeTimeDuration(audioEntry.AudioClip.length);
                audioPlayer.Play(audioEntry, parameters);
                
                var handle = new AudioHandle(audioPlayer, this._playerPool, audioEntry);
                this.RegisterHandle(handle);
                this._frequencyController.RegisterPlayback(audioEntry, handle);
                
                // Auto-cleanup non-looping audio
                if (!audioEntry.IsLooping)
                {
                    this.AutoCleanupHandleAsync(handle, audioEntry.AudioClip.length).Forget();
                }
                
                return handle;
            }
        }
        
        public async UniTask<IAudioHandle> PlayAudioAtPositionAsync(
            string audioId,
            Vector3 position,
            AudioPlaybackParameters parameters = default,
            CancellationToken cancellationToken = default)
        {
            // Override spatial blend to 3D
            parameters.overrideSpatialBlend = true;
            parameters.spatialBlend = 1f;
            
            var handle = await this.PlayAudioAsync(audioId, parameters, cancellationToken);
            
            if (handle != null)
            {
                // Find the player and set position
                var player = this.FindPlayerForHandle(handle);
                player?.SetPosition(position);
            }
            
            return handle;
        }
        
        public async UniTask<IAudioHandle> PlayAudioAttachedAsync(
            string audioId,
            Transform attachTarget,
            AudioPlaybackParameters parameters = default,
            CancellationToken cancellationToken = default)
        {
            if (attachTarget == null)
            {
                Debug.LogWarning("AudioManager: Cannot attach audio to null transform");
                return await this.PlayAudioAsync(audioId, parameters, cancellationToken);
            }
            
            // Override spatial blend to 3D
            parameters.overrideSpatialBlend = true;
            parameters.spatialBlend = 1f;
            
            var handle = await this.PlayAudioAsync(audioId, parameters, cancellationToken);

            if (handle == null) 
                return null;

            // Find the player and attach to transform
            var player = this.FindPlayerForHandle(handle);
            player?.AttachToTransform(attachTarget);
            return handle;
        }
        
        public void StopAudioType(AudioKind audioKind, float fadeOutDuration = 0f)
        {
            if (!this._handlesByType.TryGetValue(audioKind, out var handles))
            {
                return;
            }
            
            foreach (var handle in handles.ToArray())
            {
                if (handle is { IsPlaying: true })
                {
                    handle.Stop(fadeOutDuration);
                }
            }
            
            handles.Clear();
        }
        
        public void StopAllAudio(float fadeOutDuration = 0f)
        {
            foreach (var handle in this._activeHandles.ToArray())
            {
                if (handle is { IsPlaying: true })
                {
                    handle.Stop(fadeOutDuration);
                }
            }
            
            this._activeHandles.Clear();
            
            foreach (var handles in this._handlesByType.Values)
            {
                handles.Clear();
            }
        }
        
        public void PauseAudioType(AudioKind audioKind, float fadeOutDuration = 0f)
        {
            if (!this._handlesByType.TryGetValue(audioKind, out var handles))
            {
                return;
            }
            
            foreach (var handle in handles)
            {
                if (handle is { IsPlaying: true })
                {
                    handle.Pause(fadeOutDuration);
                }
            }
        }
        
        public void PauseAllAudio(float fadeOutDuration = 0f)
        {
            foreach (var handle in this._activeHandles)
            {
                if (handle is { IsPlaying: true })
                {
                    handle.Pause(fadeOutDuration);
                }
            }
        }
        
        public void ResumeAudioType(AudioKind audioKind, float fadeInDuration = 0f)
        {
            if (!this._handlesByType.TryGetValue(audioKind, out var handles))
            {
                return;
            }
            
            foreach (var handle in handles)
            {
                if (handle is { IsPaused: true })
                {
                    handle.Resume(fadeInDuration);
                }
            }
        }
        
        public void ResumeAllAudio(float fadeInDuration = 0f)
        {
            foreach (var handle in this._activeHandles)
            {
                if (handle is { IsPaused: true })
                {
                    handle.Resume(fadeInDuration);
                }
            }
        }
        
        public void SetAudioTypeVolume(AudioKind audioKind, float volume)
        {
            this._mixerController.SetAudioTypeVolume(audioKind, volume);
        }
        
        public float GetAudioTypeVolume(AudioKind audioKind)
        {
            return this._mixerController.GetAudioTypeVolume(audioKind);
        }
        
        public void SetMasterVolume(float volume)
        {
            var masterParam = this.audioMixerConfig.MasterVolumeParameter;
            var volumeDb = this.ConvertToDecibels(volume);
            this._mixerController.SetMixerParameter(masterParam, volumeDb);
        }
        
        public float GetMasterVolume()
        {
            var masterParam = this.audioMixerConfig.MasterVolumeParameter;
            var volumeDb = this._mixerController.GetMixerParameter(masterParam);
            return this.ConvertFromDecibels(volumeDb);
        }
        
        /// <summary>
        /// Registers a handle for tracking
        /// </summary>
        private void RegisterHandle(IAudioHandle handle)
        {
            if (handle == null)
            {
                return;
            }
            
            this._activeHandles.Add(handle);
            
            if (this._handlesByType.TryGetValue(handle.AudioKind, out var handles))
            {
                handles.Add(handle);
            }
        }
        
        /// <summary>
        /// Finds the audio player associated with a handle
        /// </summary>
        private IAudioPlayer FindPlayerForHandle(IAudioHandle handle)
        {
            // This is a simplified approach - in production, you might want to maintain a mapping
            // For now, we rely on the handle containing the player reference
            Debug.Log(handle);
            return null;
        }
        
        /// <summary>
        /// Automatically cleans up handle after audio finishes
        /// </summary>
        private async UniTaskVoid AutoCleanupHandleAsync(IAudioHandle handle, float duration)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration + 0.5f));
            
            if (handle is { IsPlaying: false })
            {
                this._activeHandles.Remove(handle);
                
                if (this._handlesByType.TryGetValue(handle.AudioKind, out var handles))
                {
                    handles.Remove(handle);
                }
            }
        }
        
        /// <summary>
        /// Converts linear volume to decibels
        /// </summary>
        private float ConvertToDecibels(float linearVolume)
        {
            if (linearVolume <= 0f)
            {
                return AudioConstants.MinDecibels;
            }
            
            return Mathf.Log10(linearVolume) * AudioConstants.MaxDecibels;
        }
        
        /// <summary>
        /// Converts decibels to linear volume
        /// </summary>
        private float ConvertFromDecibels(float decibels)
        {
            return decibels <= AudioConstants.MinDecibels
                ? 0f
                : Mathf.Pow(AudioConstants.DefaultPowerBase, decibels / AudioConstants.MaxDecibels);
        }
        
        private void OnDestroy()
        {
            this.StopAllAudio();
        }
    }
}

