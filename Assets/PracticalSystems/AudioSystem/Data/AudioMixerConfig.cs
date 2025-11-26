using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace PracticalSystems.AudioSystem.Data
{
    /// <summary>
    /// ScriptableObject configuration for AudioMixer groups
    /// Maps audio types to mixer groups and exposed parameters
    /// </summary>
    [CreateAssetMenu(fileName = "AudioMixerConfig", menuName = "Foundations/Audio/Audio Mixer Config")]
    public class AudioMixerConfig : ScriptableObject
    {
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;
        
        [Header("Mixer Groups")]
        [SerializeField] private List<AudioTypeMixerMapping> audioTypeMappings = new List<AudioTypeMixerMapping>();
        
        [Header("Global Parameters")]
        [SerializeField] private string masterVolumeParameter = "MasterVolume";
        [SerializeField] private float defaultMasterVolume = 1f;
        
        public AudioMixer AudioMixer => this.audioMixer;
        public IReadOnlyList<AudioTypeMixerMapping> AudioTypeMappings => this.audioTypeMappings;
        public string MasterVolumeParameter => this.masterVolumeParameter;
        public float DefaultMasterVolume => this.defaultMasterVolume;
        
        /// <summary>
        /// Gets the mixer group for a specific audio type
        /// </summary>
        public AudioMixerGroup GetMixerGroup(AudioKind audioKind)
        {
            foreach (var mapping in this.audioTypeMappings)
            {
                if (mapping.audioKind == audioKind)
                {
                    return mapping.mixerGroup;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Gets the volume parameter name for a specific audio type
        /// </summary>
        public string GetVolumeParameterName(AudioKind audioKind)
        {
            foreach (var mapping in this.audioTypeMappings)
            {
                if (mapping.audioKind == audioKind)
                {
                    return mapping.volumeParameterName;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Gets the default volume for a specific audio type
        /// </summary>
        public float GetDefaultVolume(AudioKind audioKind)
        {
            foreach (var mapping in this.audioTypeMappings)
            {
                if (mapping.audioKind == audioKind)
                {
                    return mapping.defaultVolume;
                }
            }
            
            return 1f;
        }
    }
}

