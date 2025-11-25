using PracticalSystems.AudioSystem.Data;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Interfaces
{
    /// <summary>
    /// Represents a single audio entry in the audio database
    /// </summary>
    public interface IAudioEntry
    {
        /// <summary>
        /// Unique identifier for this audio entry
        /// </summary>
        public string AudioId { get; }
        
        /// <summary>
        /// Type of audio (BGM, SFX, Voice, etc.)
        /// </summary>
        public AudioKind AudioKind { get; }
        
        /// <summary>
        /// The audio clip to play
        /// </summary>
        public AudioClip AudioClip { get; }
        
        /// <summary>
        /// Default volume for this audio (0-1)
        /// </summary>
        public float DefaultVolume { get; }
        
        /// <summary>
        /// Default pitch for this audio (0.1-3)
        /// </summary>
        public float DefaultPitch { get; }
        
        /// <summary>
        /// Whether this audio loops
        /// </summary>
        public bool IsLooping { get; }
        
        /// <summary>
        /// Priority for audio playback (0-256, higher is more important)
        /// </summary>
        public int Priority { get; }
        
        /// <summary>
        /// Minimum time between consecutive plays (for high-frequency audio)
        /// </summary>
        public float MinTimeBetweenPlays { get; }
        
        /// <summary>
        /// Maximum concurrent instances of this audio
        /// </summary>
        public int MaxConcurrentInstances { get; }
        
        /// <summary>
        /// Spatial blend (0 = 2D, 1 = 3D)
        /// </summary>
        public float SpatialBlend { get; }
        
        /// <summary>
        /// Minimum distance for 3D audio
        /// </summary>
        public float MinDistance { get; }
        
        /// <summary>
        /// Maximum distance for 3D audio
        /// </summary>
        public float MaxDistance { get; }
    }
}

