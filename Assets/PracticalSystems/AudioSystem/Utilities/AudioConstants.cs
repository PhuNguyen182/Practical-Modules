namespace Foundations.Audio.Utilities
{
    /// <summary>
    /// Constants and default values for the audio system
    /// </summary>
    public static class AudioConstants
    {
        /// <summary>
        /// Default fade duration in seconds
        /// </summary>
        public const float DEFAULT_FADE_DURATION = 1f;
        
        /// <summary>
        /// Minimum decibel value for volume
        /// </summary>
        public const float MIN_DECIBELS = -80f;
        
        /// <summary>
        /// Maximum decibel value for volume
        /// </summary>
        public const float MAX_DECIBELS = 20f;
        
        /// <summary>
        /// Default audio priority (0-256, higher is more important)
        /// </summary>
        public const int DEFAULT_PRIORITY = 128;
        
        /// <summary>
        /// Default min distance for 3D audio
        /// </summary>
        public const float DEFAULT_MIN_DISTANCE = 1f;
        
        /// <summary>
        /// Default max distance for 3D audio
        /// </summary>
        public const float DEFAULT_MAX_DISTANCE = 500f;
        
        /// <summary>
        /// Default initial pool size
        /// </summary>
        public const int DEFAULT_POOL_SIZE = 10;
        
        /// <summary>
        /// Default max concurrent instances per audio
        /// </summary>
        public const int DEFAULT_MAX_CONCURRENT_INSTANCES = 5;
        
        /// <summary>
        /// Minimum pitch value
        /// </summary>
        public const float MIN_PITCH = 0.1f;
        
        /// <summary>
        /// Maximum pitch value
        /// </summary>
        public const float MAX_PITCH = 3f;
        
        /// <summary>
        /// Converts linear volume (0-1) to decibels
        /// </summary>
        public static float LinearToDecibels(float linearVolume)
        {
            if (linearVolume <= 0f)
            {
                return MIN_DECIBELS;
            }
            
            return UnityEngine.Mathf.Clamp(
                UnityEngine.Mathf.Log10(linearVolume) * 20f,
                MIN_DECIBELS,
                MAX_DECIBELS
            );
        }
        
        /// <summary>
        /// Converts decibels to linear volume (0-1)
        /// </summary>
        public static float DecibelsToLinear(float decibels)
        {
            if (decibels <= MIN_DECIBELS)
            {
                return 0f;
            }
            
            return UnityEngine.Mathf.Pow(10f, decibels / 20f);
        }
    }
}

