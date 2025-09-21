using System;
using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;

namespace GameVisualUpdateByTimeSystem.Visuals.Audio
{
    /// <summary>
    /// Represents a complete audio state for time-based audio transitions
    /// </summary>
    [Serializable]
    public class AudioState : IVisualState
    {
        [Header("State Configuration")]
        public string stateId;
        public TimeCondition timeCondition;
        
        [Header("Audio Sources")]
        public AudioSourceState[] AudioSources = new AudioSourceState[0];
        
        public string StateId => stateId;
        public TimeCondition TimeCondition => timeCondition;
        
        public AudioState()
        {
            stateId = "Default";
            timeCondition = new TimeCondition();
        }
        
        public AudioState(string stateId, TimeCondition timeCondition)
        {
            this.stateId = stateId;
            this.timeCondition = timeCondition;
        }
        
        /// <summary>
        /// Creates a copy of this audio state
        /// </summary>
        /// <returns>Deep copy of the audio state</returns>
        public AudioState Clone()
        {
            var clone = new AudioState(StateId, TimeCondition)
            {
                AudioSources = new AudioSourceState[AudioSources.Length]
            };
            
            for (int i = 0; i < AudioSources.Length; i++)
            {
                clone.AudioSources[i] = AudioSources[i].Clone();
            }
            
            return clone;
        }
        
        /// <summary>
        /// Validates the audio state configuration
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(StateId))
            {
                Debug.LogError("AudioState: StateId cannot be null or empty");
                return false;
            }
            
            if (AudioSources == null)
            {
                AudioSources = new AudioSourceState[0];
            }
            
            // Validate audio source states
            foreach (var audioSource in AudioSources)
            {
                if (audioSource != null && !audioSource.IsValid())
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Gets a human-readable description of this audio state
        /// </summary>
        /// <returns>Description string</returns>
        public string GetDescription()
        {
            var condition = TimeCondition;
            var timeDesc = condition.Hour switch
            {
                -1 => "Any time",
                0 => "Midnight",
                6 => "Dawn",
                12 => "Noon",
                18 => "Dusk",
                23 => "Late night",
                _ => $"{condition.Hour:00}:00"
            };
            
            var seasonDesc = condition.UseSeason ? $" ({condition.Season})" : "";
            var monthDesc = condition.Month != -1 ? $" Month {condition.Month}" : "";
            
            return $"{StateId}: {timeDesc}{seasonDesc}{monthDesc} ({AudioSources.Length} audio sources)";
        }
        
        /// <summary>
        /// Adds an audio source state to this audio state
        /// </summary>
        /// <param name="audioSourceState">Audio source state to add</param>
        public void AddAudioSource(AudioSourceState audioSourceState)
        {
            var newSources = new AudioSourceState[AudioSources.Length + 1];
            AudioSources.CopyTo(newSources, 0);
            newSources[AudioSources.Length] = audioSourceState;
            AudioSources = newSources;
        }
        
        /// <summary>
        /// Removes an audio source state by index
        /// </summary>
        /// <param name="index">Index of the audio source to remove</param>
        public void RemoveAudioSource(int index)
        {
            if (index >= 0 && index < AudioSources.Length)
            {
                var newSources = new AudioSourceState[AudioSources.Length - 1];
                Array.Copy(AudioSources, 0, newSources, 0, index);
                Array.Copy(AudioSources, index + 1, newSources, index, AudioSources.Length - index - 1);
                AudioSources = newSources;
            }
        }
        
        /// <summary>
        /// Creates a seasonal audio state
        /// </summary>
        /// <param name="season">Season for the audio</param>
        /// <param name="hour">Hour of day</param>
        /// <returns>Seasonal audio state</returns>
        public static AudioState CreateSeasonalState(Season season, int hour = 12)
        {
            var state = new AudioState($"Season_{season}_{hour:D2}h", new TimeCondition(hour, -1, -1, -1, TimeType.Hour, true, season));
            
            // Adjust audio properties based on season
            switch (season)
            {
                case Season.Spring:
                    state.AudioSources = new AudioSourceState[]
                    {
                        new AudioSourceState
                        {
                            Volume = 0.7f,
                            Pitch = 1.1f,
                            Pan = 0f,
                            Loop = true,
                            Mute = false
                        }
                    };
                    break;
                    
                case Season.Summer:
                    state.AudioSources = new AudioSourceState[]
                    {
                        new AudioSourceState
                        {
                            Volume = 0.8f,
                            Pitch = 1.0f,
                            Pan = 0f,
                            Loop = true,
                            Mute = false
                        }
                    };
                    break;
                    
                case Season.Autumn:
                    state.AudioSources = new AudioSourceState[]
                    {
                        new AudioSourceState
                        {
                            Volume = 0.6f,
                            Pitch = 0.9f,
                            Pan = 0f,
                            Loop = true,
                            Mute = false
                        }
                    };
                    break;
                    
                case Season.Winter:
                    state.AudioSources = new AudioSourceState[]
                    {
                        new AudioSourceState
                        {
                            Volume = 0.4f,
                            Pitch = 0.8f,
                            Pan = 0f,
                            Loop = true,
                            Mute = false
                        }
                    };
                    break;
            }
            
            // Adjust for time of day
            if (hour < 6 || hour > 20) // Night
            {
                foreach (var audioSource in state.AudioSources)
                {
                    audioSource.Volume *= 0.5f;
                    audioSource.Pitch *= 0.8f;
                }
            }
            
            return state;
        }
    }
    
    /// <summary>
    /// Represents the state of a single audio source
    /// </summary>
    [Serializable]
    public class AudioSourceState
    {
        [Header("Audio Clip")]
        [Tooltip("Audio clip to play")]
        public AudioClip AudioClip;
        
        [Header("Audio Properties")]
        [Range(0f, 1f)]
        [Tooltip("Volume of the audio source")]
        public float Volume = 1f;
        
        [Range(0.1f, 3f)]
        [Tooltip("Pitch of the audio source")]
        public float Pitch = 1f;
        
        [Range(-1f, 1f)]
        [Tooltip("Pan (stereo balance) of the audio source")]
        public float Pan = 0f;
        
        [Header("Playback Settings")]
        [Tooltip("Whether the audio should loop")]
        public bool Loop = true;
        
        [Tooltip("Whether the audio source is muted")]
        public bool Mute = false;
        
        [Tooltip("Whether to play the audio when transitioning to this state")]
        public bool PlayOnTransition = true;
        
        [Header("Advanced Settings")]
        [Tooltip("Priority of the audio source (0 = highest priority)")]
        [Range(0, 256)]
        public int Priority = 128;
        
        [Tooltip("Spatial blend (0 = 2D, 1 = 3D)")]
        [Range(0f, 1f)]
        public float SpatialBlend = 0f;
        
        [Tooltip("Reverb zone mix")]
        [Range(0f, 1.1f)]
        public float ReverbZoneMix = 1f;
        
        public AudioSourceState()
        {
            Volume = 1f;
            Pitch = 1f;
            Pan = 0f;
            Loop = true;
            Mute = false;
            PlayOnTransition = true;
            Priority = 128;
            SpatialBlend = 0f;
            ReverbZoneMix = 1f;
        }
        
        public AudioSourceState(AudioClip audioClip, float volume = 1f, float pitch = 1f, bool loop = true)
        {
            AudioClip = audioClip;
            Volume = volume;
            Pitch = pitch;
            Loop = loop;
            Mute = false;
            PlayOnTransition = true;
            Priority = 128;
            SpatialBlend = 0f;
            ReverbZoneMix = 1f;
        }
        
        /// <summary>
        /// Creates a copy of this audio source state
        /// </summary>
        /// <returns>Deep copy of the audio source state</returns>
        public AudioSourceState Clone()
        {
            return new AudioSourceState
            {
                AudioClip = AudioClip,
                Volume = Volume,
                Pitch = Pitch,
                Pan = Pan,
                Loop = Loop,
                Mute = Mute,
                PlayOnTransition = PlayOnTransition,
                Priority = Priority,
                SpatialBlend = SpatialBlend,
                ReverbZoneMix = ReverbZoneMix
            };
        }
        
        /// <summary>
        /// Validates the audio source state configuration
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            if (Volume < 0f || Volume > 1f)
            {
                Debug.LogError("AudioSourceState: Volume must be between 0 and 1");
                return false;
            }
            
            if (Pitch < 0.1f || Pitch > 3f)
            {
                Debug.LogError("AudioSourceState: Pitch must be between 0.1 and 3");
                return false;
            }
            
            if (Pan < -1f || Pan > 1f)
            {
                Debug.LogError("AudioSourceState: Pan must be between -1 and 1");
                return false;
            }
            
            if (Priority < 0 || Priority > 256)
            {
                Debug.LogError("AudioSourceState: Priority must be between 0 and 256");
                return false;
            }
            
            if (SpatialBlend < 0f || SpatialBlend > 1f)
            {
                Debug.LogError("AudioSourceState: SpatialBlend must be between 0 and 1");
                return false;
            }
            
            if (ReverbZoneMix < 0f || ReverbZoneMix > 1.1f)
            {
                Debug.LogError("AudioSourceState: ReverbZoneMix must be between 0 and 1.1");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Applies this audio source state to a Unity AudioSource component
        /// </summary>
        /// <param name="audioSource">AudioSource component to apply state to</param>
        public void ApplyToAudioSource(AudioSource audioSource)
        {
            if (audioSource == null) return;
            
            audioSource.clip = AudioClip;
            audioSource.volume = Volume;
            audioSource.pitch = Pitch;
            audioSource.panStereo = Pan;
            audioSource.loop = Loop;
            audioSource.mute = Mute;
            audioSource.priority = Priority;
            audioSource.spatialBlend = SpatialBlend;
            audioSource.reverbZoneMix = ReverbZoneMix;
            
            if (PlayOnTransition && AudioClip != null)
            {
                audioSource.Play();
            }
        }
        
        /// <summary>
        /// Captures current state from a Unity AudioSource component
        /// </summary>
        /// <param name="audioSource">AudioSource component to capture from</param>
        public void CaptureFromAudioSource(AudioSource audioSource)
        {
            if (audioSource == null) return;
            
            AudioClip = audioSource.clip;
            Volume = audioSource.volume;
            Pitch = audioSource.pitch;
            Pan = audioSource.panStereo;
            Loop = audioSource.loop;
            Mute = audioSource.mute;
            Priority = audioSource.priority;
            SpatialBlend = audioSource.spatialBlend;
            ReverbZoneMix = audioSource.reverbZoneMix;
            PlayOnTransition = false;
        }
        
        /// <summary>
        /// Gets a human-readable description of this audio source state
        /// </summary>
        /// <returns>Description string</returns>
        public string GetDescription()
        {
            var clipName = AudioClip != null ? AudioClip.name : "None";
            return $"{clipName} (Vol: {Volume:F2}, Pitch: {Pitch:F2}, Pan: {Pan:F2})";
        }
    }
}
