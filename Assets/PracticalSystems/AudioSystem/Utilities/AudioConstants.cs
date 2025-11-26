using UnityEngine;

namespace PracticalSystems.AudioSystem.Utilities
{
    /// <summary>
    /// Constants and default values for the audio system
    /// </summary>
    public static class AudioConstants
    {
        public const float DefaultPowerBase = 10f;
        
        /// <summary>
        /// Default fade duration in seconds
        /// </summary>
        public const float DefaultFadeDuration = 1f;
        
        /// <summary>
        /// Minimum decibel value for volume
        /// </summary>
        public const float MinDecibels = -80f;
        
        /// <summary>
        /// Maximum decibel value for volume
        /// </summary>
        public const float MaxDecibels = 20f;
        
        /// <summary>
        /// Default audio priority (0-256, higher is more important)
        /// </summary>
        public const int DefaultPriority = 128;
        
        /// <summary>
        /// Default min distance for 3D audio
        /// </summary>
        public const float DefaultMinDistance = 1f;
        
        /// <summary>
        /// Default max distance for 3D audio
        /// </summary>
        public const float DefaultMaxDistance = 500f;
        
        /// <summary>
        /// Default initial pool size
        /// </summary>
        public const int DefaultPoolSize = 10;
        
        /// <summary>
        /// Default max concurrent instances per audio
        /// </summary>
        public const int DefaultMaxConcurrentInstances = 5;
        
        /// <summary>
        /// Minimum pitch value
        /// </summary>
        public const float MinPitch = 0.1f;
        
        /// <summary>
        /// Maximum pitch value
        /// </summary>
        public const float MaxPitch = 3f;

        /// <summary>
        /// Converts linear volume (0-1) to decibels
        /// </summary>
        public static float LinearToDecibels(float linearVolume)
        {
            return linearVolume <= 0f ? MinDecibels
                : Mathf.Clamp(Mathf.Log10(linearVolume) * MaxDecibels, MinDecibels, MaxDecibels);
        }

        /// <summary>
        /// Converts decibels to linear volume (0-1)
        /// </summary>
        public static float DecibelsToLinear(float decibels)
        {
            return decibels <= MinDecibels ? 0f : Mathf.Pow(DefaultPowerBase, decibels / MaxDecibels);
        }
    }
}

