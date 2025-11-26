using System;
using UnityEngine.Audio;

namespace PracticalSystems.AudioSystem.Data
{
    [Serializable]
    public class AudioTypeMixerMapping
    {
        public AudioKind audioKind;
        public AudioMixerGroup mixerGroup;
        public string volumeParameterName;
        public float defaultVolume = 1f;
    }
}