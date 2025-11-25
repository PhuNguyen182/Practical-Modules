using System.Threading;
using Cysharp.Threading.Tasks;
using PracticalSystems.AudioSystem.Data;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Interfaces
{
    /// <summary>
    /// Main interface for audio management system
    /// Provides comprehensive control over audio playback, mixing, and lifecycle
    /// </summary>
    public interface IAudioManager
    {
        /// <summary>
        /// Plays an audio clip with specified parameters
        /// </summary>
        /// <param name="audioId">Unique identifier for the audio clip</param>
        /// <param name="parameters">Optional parameters for audio playback</param>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>Handle to control the playing audio</returns>
        public UniTask<IAudioHandle> PlayAudioAsync(
            string audioId, 
            AudioPlaybackParameters parameters = default,
            CancellationToken cancellationToken = default
        );
        
        /// <summary>
        /// Plays audio at a specific world position
        /// </summary>
        public UniTask<IAudioHandle> PlayAudioAtPositionAsync(
            string audioId,
            Vector3 position,
            AudioPlaybackParameters parameters = default,
            CancellationToken cancellationToken = default
        );
        
        /// <summary>
        /// Plays audio attached to a transform (follows the object)
        /// </summary>
        public UniTask<IAudioHandle> PlayAudioAttachedAsync(
            string audioId,
            Transform attachTarget,
            AudioPlaybackParameters parameters = default,
            CancellationToken cancellationToken = default
        );
        
        /// <summary>
        /// Stops all audio of a specific type
        /// </summary>
        public void StopAudioType(AudioKind audioType, float fadeOutDuration = 0f);
        
        /// <summary>
        /// Stops all currently playing audio
        /// </summary>
        public void StopAllAudio(float fadeOutDuration = 0f);
        
        /// <summary>
        /// Pauses all audio of a specific type
        /// </summary>
        public void PauseAudioType(AudioKind audioType, float fadeOutDuration = 0f);
        
        /// <summary>
        /// Pauses all currently playing audio
        /// </summary>
        public void PauseAllAudio(float fadeOutDuration = 0f);
        
        /// <summary>
        /// Resumes all paused audio of a specific type
        /// </summary>
        public void ResumeAudioType(AudioKind audioType, float fadeInDuration = 0f);
        
        /// <summary>
        /// Resumes all paused audio
        /// </summary>
        public void ResumeAllAudio(float fadeInDuration = 0f);
        
        /// <summary>
        /// Sets the volume for a specific audio type
        /// </summary>
        public void SetAudioTypeVolume(AudioKind audioType, float volume);
        
        /// <summary>
        /// Gets the volume for a specific audio type
        /// </summary>
        public float GetAudioTypeVolume(AudioKind audioType);
        
        /// <summary>
        /// Sets the master volume
        /// </summary>
        public void SetMasterVolume(float volume);
        
        /// <summary>
        /// Gets the master volume
        /// </summary>
        public float GetMasterVolume();
    }
}

