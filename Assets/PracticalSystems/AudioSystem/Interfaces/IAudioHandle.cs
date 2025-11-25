using Cysharp.Threading.Tasks;
using PracticalSystems.AudioSystem.Data;

namespace PracticalSystems.AudioSystem.Interfaces
{
    /// <summary>
    /// Handle to control individual audio playback
    /// Provides fine-grained control over playing audio instances
    /// </summary>
    public interface IAudioHandle
    {
        /// <summary>
        /// Gets whether the audio is currently playing
        /// </summary>
        public bool IsPlaying { get; }
        
        /// <summary>
        /// Gets whether the audio is paused
        /// </summary>
        public bool IsPaused { get; }
        
        /// <summary>
        /// Gets the current playback time
        /// </summary>
        public float CurrentTime { get; }
        
        /// <summary>
        /// Gets the total duration of the audio
        /// </summary>
        public float Duration { get; }
        
        /// <summary>
        /// Gets the audio type
        /// </summary>
        public AudioKind AudioKind { get; }
        
        /// <summary>
        /// Stops the audio playback
        /// </summary>
        public void Stop(float fadeOutDuration = 0f);
        
        /// <summary>
        /// Pauses the audio playback
        /// </summary>
        public void Pause(float fadeOutDuration = 0f);
        
        /// <summary>
        /// Resumes the audio playback
        /// </summary>
        public void Resume(float fadeInDuration = 0f);
        
        /// <summary>
        /// Sets the volume
        /// </summary>
        public void SetVolume(float volume);
        
        /// <summary>
        /// Sets the pitch
        /// </summary>
        public void SetPitch(float pitch);
        
        /// <summary>
        /// Fades the volume over time
        /// </summary>
        public UniTask FadeVolumeAsync(float targetVolume, float duration);
        
        /// <summary>
        /// Waits until the audio finishes playing
        /// </summary>
        public UniTask WaitForCompletionAsync();
    }
}

