using System.Collections.Generic;
using PracticalSystems.AudioSystem.Data;
using PracticalSystems.AudioSystem.Interfaces;
using UnityEngine;
using UnityEngine.Audio;

namespace PracticalSystems.AudioSystem.Core
{
    /// <summary>
    /// Controls AudioMixer parameters and volume settings
    /// Provides centralized mixer management with type-safe access
    /// </summary>
    public class AudioMixerController : IAudioMixerController
    {
        private readonly AudioMixerConfig _audioMixerConfig;
        private readonly Dictionary<AudioKind, AudioMixerGroup> _mixerGroupLookup;
        private readonly Dictionary<AudioKind, string> _volumeParameterLookup;
        
        public AudioMixerController(AudioMixerConfig audioMixerConfig)
        {
            this._audioMixerConfig = audioMixerConfig;
            this._mixerGroupLookup = new Dictionary<AudioKind, AudioMixerGroup>();
            this._volumeParameterLookup = new Dictionary<AudioKind, string>();
            
            this.InitializeMixerMappings();
            this.InitializeDefaultVolumes();
        }
        
        /// <summary>
        /// Initializes mixer group mappings
        /// </summary>
        private void InitializeMixerMappings()
        {
            foreach (var mapping in this._audioMixerConfig.AudioTypeMappings)
            {
                if (mapping.mixerGroup == null)
                {
                    Debug.LogWarning($"AudioMixerController: Missing mixer group for {mapping.audioKind}");
                    continue;
                }
                
                this._mixerGroupLookup[mapping.audioKind] = mapping.mixerGroup;
                this._volumeParameterLookup[mapping.audioKind] = mapping.volumeParameterName;
            }
        }
        
        /// <summary>
        /// Initializes default volume values
        /// </summary>
        private void InitializeDefaultVolumes()
        {
            // Set master volume
            this.SetMixerParameter(this._audioMixerConfig.MasterVolumeParameter, this.ConvertToDecibels(this._audioMixerConfig.DefaultMasterVolume));
            
            // Set audio type volumes
            foreach (var mapping in this._audioMixerConfig.AudioTypeMappings)
            {
                this.SetAudioTypeVolume(mapping.audioKind, mapping.defaultVolume);
            }
        }
        
        public void SetMixerParameter(string parameterName, float value)
        {
            if (this._audioMixerConfig.AudioMixer == null)
            {
                Debug.LogWarning("AudioMixerController: AudioMixer is null");
                return;
            }
            
            if (string.IsNullOrEmpty(parameterName))
            {
                Debug.LogWarning("AudioMixerController: Parameter name is null or empty");
                return;
            }
            
            this._audioMixerConfig.AudioMixer.SetFloat(parameterName, value);
        }
        
        public float GetMixerParameter(string parameterName)
        {
            if (this._audioMixerConfig.AudioMixer == null)
            {
                Debug.LogWarning("AudioMixerController: AudioMixer is null");
                return 0f;
            }
            
            if (string.IsNullOrEmpty(parameterName))
            {
                Debug.LogWarning("AudioMixerController: Parameter name is null or empty");
                return 0f;
            }
            
            this._audioMixerConfig.AudioMixer.GetFloat(parameterName, out var value);
            return value;
        }
        
        public void SetAudioTypeVolume(AudioKind audioKind, float volume)
        {
            if (!this._volumeParameterLookup.TryGetValue(audioKind, out var parameterName))
            {
                Debug.LogWarning($"AudioMixerController: No volume parameter found for {audioKind}");
                return;
            }
            
            var volumeDb = this.ConvertToDecibels(volume);
            this.SetMixerParameter(parameterName, volumeDb);
        }
        
        public float GetAudioTypeVolume(AudioKind audioKind)
        {
            if (!this._volumeParameterLookup.TryGetValue(audioKind, out var parameterName))
            {
                Debug.LogWarning($"AudioMixerController: No volume parameter found for {audioKind}");
                return 1f;
            }
            
            var volumeDb = this.GetMixerParameter(parameterName);
            return this.ConvertFromDecibels(volumeDb);
        }
        
        public AudioMixerGroup GetMixerGroup(AudioKind audioKind)
        {
            if (this._mixerGroupLookup.TryGetValue(audioKind, out var mixerGroup))
            {
                return mixerGroup;
            }
            
            Debug.LogWarning($"AudioMixerController: No mixer group found for {audioKind}");
            return null;
        }
        
        /// <summary>
        /// Converts linear volume (0-1) to decibels
        /// </summary>
        private float ConvertToDecibels(float linearVolume)
        {
            if (linearVolume <= 0f)
            {
                return -80f; // Minimum decibel value
            }
            
            return Mathf.Log10(linearVolume) * 20f;
        }
        
        /// <summary>
        /// Converts decibels to linear volume (0-1)
        /// </summary>
        private float ConvertFromDecibels(float decibels)
        {
            if (decibels <= -80f)
            {
                return 0f;
            }
            
            return Mathf.Pow(10f, decibels / 20f);
        }
    }
}

