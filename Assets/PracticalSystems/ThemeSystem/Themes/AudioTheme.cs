using PracticalSystems.ThemeSystem.Components;
using PracticalSystems.ThemeSystem.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace PracticalSystems.ThemeSystem.Themes
{
    /// <summary>
    /// Audio theme data structure
    /// </summary>
    [CreateAssetMenu(fileName = "Audio Theme", menuName = "Theme System/Audio Theme")]
    public class AudioTheme : BaseTheme
    {
        [Header("Audio Theme Settings")]
        [SerializeField] private AudioThemeData themeData = new AudioThemeData();
        
        public AudioThemeData ThemeData => themeData;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            category = "Audio";
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            category = "Audio";
        }
        
        public override bool ApplyTo(IThemeComponent component)
        {
            if (component is AudioThemeComponent audioComponent)
            {
                audioComponent.ApplyAudioTheme(themeData);
                return base.ApplyTo(component);
            }
            return false;
        }
    }
    
    /// <summary>
    /// Audio theme data container
    /// </summary>
    [System.Serializable]
    public class AudioThemeData
    {
        [Header("Background Music")]
        public AudioClip backgroundMusic;
        public float musicVolume = 0.7f;
        public bool loopMusic = true;
        public float musicFadeInTime = 2f;
        public float musicFadeOutTime = 2f;
        public AudioMixerGroup musicMixerGroup;
        
        [Header("Ambient Sounds")]
        public AudioClip[] ambientSounds;
        public float ambientVolume = 0.5f;
        public bool loopAmbient = true;
        public float ambientFadeInTime = 3f;
        public float ambientFadeOutTime = 3f;
        public AudioMixerGroup ambientMixerGroup;
        
        [Header("Sound Effects")]
        public AudioClip[] uiSoundEffects;
        public AudioClip[] environmentSoundEffects;
        public AudioClip[] characterSoundEffects;
        public float sfxVolume = 0.8f;
        public AudioMixerGroup sfxMixerGroup;
        
        [Header("Audio Settings")]
        public float masterVolume = 1f;
        public bool enableAudio = true;
        public AudioReverbPreset reverbPreset = AudioReverbPreset.Off;
        public float reverbLevel = 0f;
        public float hfReference = 5000f;
        public float room = 0f;
        public float roomHF = 0f;
        public float roomLF = 0f;
        public float decayTime = 1.49f;
        public float decayHFRatio = 0.83f;
        public float reflections = -2602f;
        public float reflectionsDelay = 0.007f;
        public float reverb = 200f;
        public float reverbDelay = 0.011f;
        public float hfReference2 = 5000f;
        public float lfReference = 250f;
        public float diffusion = 100f;
        public float density = 100f;
        
        [Header("Dynamic Range")]
        public bool enableDynamicRange = true;
        public float dynamicRangeMin = 0.1f;
        public float dynamicRangeMax = 1f;
        public float dynamicRangeCurve = 1f;
        
        [Header("Spatial Audio")]
        public bool enableSpatialAudio = true;
        public float spatialBlend = 1f;
        public float dopplerLevel = 1f;
        public float spread = 0f;
        public float minDistance = 1f;
        public float maxDistance = 500f;
        public AnimationCurve rolloffMode = AnimationCurve.Linear(0, 1, 1, 0);
        
        [Header("Audio Filters")]
        public bool enableLowPassFilter = false;
        public float lowPassCutoff = 5000f;
        public float lowPassResonance = 1f;
        public bool enableHighPassFilter = false;
        public float highPassCutoff = 5000f;
        public float highPassResonance = 1f;
        public bool enableEchoFilter = false;
        public float echoDelay = 500f;
        public float echoDecayRatio = 0.5f;
        public bool enableDistortionFilter = false;
        public float distortionLevel = 0.5f;
    }
}
