using PracticalSystems.AudioSystem.Data;
using UnityEngine.Audio;

namespace PracticalSystems.AudioSystem.Interfaces
{
    /// <summary>
    /// Interface for controlling AudioMixer parameters
    /// </summary>
    public interface IAudioMixerController
    {
        /// <summary>
        /// Sets a mixer parameter value
        /// </summary>
        public void SetMixerParameter(string parameterName, float value);
        
        /// <summary>
        /// Gets a mixer parameter value
        /// </summary>
        public float GetMixerParameter(string parameterName);
        
        /// <summary>
        /// Sets the volume for a specific audio type mixer group
        /// </summary>
        public void SetAudioTypeVolume(AudioKind audioType, float volume);
        
        /// <summary>
        /// Gets the volume for a specific audio type mixer group
        /// </summary>
        public float GetAudioTypeVolume(AudioKind audioType);
        
        /// <summary>
        /// Gets the mixer group for a specific audio type
        /// </summary>
        public AudioMixerGroup GetMixerGroup(AudioKind audioType);
    }
}

