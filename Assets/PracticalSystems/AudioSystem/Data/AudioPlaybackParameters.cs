namespace PracticalSystems.AudioSystem.Data
{
    /// <summary>
    /// Parameters for audio playback customization
    /// Provides full control over audio playback behavior
    /// </summary>
    [System.Serializable]
    public struct AudioPlaybackParameters
    {
        /// <summary>
        /// Volume multiplier (0-1, multiplied with default volume)
        /// </summary>
        public float volumeMultiplier;
        
        /// <summary>
        /// Pitch multiplier (0.1-3, multiplied with default pitch)
        /// </summary>
        public float pitchMultiplier;
        
        /// <summary>
        /// Fade in duration in seconds
        /// </summary>
        public float fadeInDuration;
        
        /// <summary>
        /// Delay before starting playback in seconds
        /// </summary>
        public float delay;
        
        /// <summary>
        /// Whether to override the default loop setting
        /// </summary>
        public bool overrideLoop;
        
        /// <summary>
        /// Loop value when overriding
        /// </summary>
        public bool loop;
        
        /// <summary>
        /// Whether to override spatial blend
        /// </summary>
        public bool overrideSpatialBlend;
        
        /// <summary>
        /// Spatial blend value when overriding (0 = 2D, 1 = 3D)
        /// </summary>
        public float spatialBlend;
        
        /// <summary>
        /// Whether to use random pitch variation
        /// </summary>
        public bool useRandomPitch;
        
        /// <summary>
        /// Random pitch variation range (-range to +range)
        /// </summary>
        public float randomPitchRange;
        
        /// <summary>
        /// Whether to use random volume variation
        /// </summary>
        public bool useRandomVolume;
        
        /// <summary>
        /// Random volume variation range (-range to +range)
        /// </summary>
        public float randomVolumeRange;
        
        /// <summary>
        /// Creates default playback parameters
        /// </summary>
        public static AudioPlaybackParameters Default => new()
        {
            volumeMultiplier = 1f,
            pitchMultiplier = 1f,
            fadeInDuration = 0f,
            delay = 0f,
            overrideLoop = false,
            loop = false,
            overrideSpatialBlend = false,
            spatialBlend = 0f,
            useRandomPitch = false,
            randomPitchRange = 0f,
            useRandomVolume = false,
            randomVolumeRange = 0f
        };
        
        /// <summary>
        /// Creates parameters with fade in
        /// </summary>
        public static AudioPlaybackParameters WithFadeIn(float fadeInDuration)
        {
            var parameters = Default;
            parameters.fadeInDuration = fadeInDuration;
            return parameters;
        }
        
        /// <summary>
        /// Creates parameters with looping
        /// </summary>
        public static AudioPlaybackParameters WithLoop(bool loop)
        {
            var parameters = Default;
            parameters.overrideLoop = true;
            parameters.loop = loop;
            return parameters;
        }
        
        /// <summary>
        /// Creates parameters with random pitch variation
        /// </summary>
        public static AudioPlaybackParameters WithRandomPitch(float range)
        {
            var parameters = Default;
            parameters.useRandomPitch = true;
            parameters.randomPitchRange = range;
            return parameters;
        }
    }
}

