using PracticalSystems.AudioSystem.Data;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Interfaces
{
    /// <summary>
    /// Interface for individual audio player component
    /// </summary>
    public interface IAudioPlayer
    {
        /// <summary>
        /// Gets the AudioSource component
        /// </summary>
        public AudioSource AudioSource { get; }
        
        /// <summary>
        /// Gets the transform of this audio player
        /// </summary>
        public Transform Transform { get; }
        
        /// <summary>
        /// Gets whether this player is currently in use
        /// </summary>
        public bool IsPlaying { get; }
        
        /// <summary>
        /// Gets whether this player is paused
        /// </summary>
        public bool IsPaused { get; }

        /// <summary>
        /// Initializes the audio player pool service
        /// </summary>
        public void InitializePoolService(IAudioPlayerPool playerPool);
        
        /// <summary>
        /// Plays audio with specified parameters
        /// </summary>
        public void Play(IAudioEntry audioEntry, AudioPlaybackParameters parameters);
        
        /// <summary>
        /// Stops playback
        /// </summary>
        public void Stop();
        
        /// <summary>
        /// Pauses playback
        /// </summary>
        public void Pause();
        
        /// <summary>
        /// Resumes playback
        /// </summary>
        public void Resume();
        
        /// <summary>
        /// Sets the position for 3D audio
        /// </summary>
        public void SetPosition(Vector3 position);
        
        /// <summary>
        /// Attaches to a transform (follows the object)
        /// </summary>
        public void AttachToTransform(Transform target);
        
        /// <summary>
        /// Setup lifetime duration for this audio player
        /// </summary>
        public void SetLifeTimeDuration(float duration);
        
        /// <summary>
        /// Detaches from transform
        /// </summary>
        public void DetachFromTransform();
        
        /// <summary>
        /// Resets the player to initial state
        /// </summary>
        public void Reset();
    }
}

