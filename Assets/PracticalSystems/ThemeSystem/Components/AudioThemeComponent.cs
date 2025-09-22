using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using PracticalSystems.ThemeSystem.Core;
using PracticalSystems.ThemeSystem.Themes;

namespace PracticalSystems.ThemeSystem.Components
{
    /// <summary>
    /// Component that applies audio themes to audio sources and mixer groups
    /// </summary>
    public class AudioThemeComponent : BaseThemeComponent
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource[] audioSources;
        [SerializeField] private AudioSource backgroundMusicSource;
        [SerializeField] private AudioSource ambientSoundSource;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Audio Mixer Groups")]
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup ambientMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        
        [Header("Audio Settings")]
        [SerializeField] private bool applyMusic = true;
        [SerializeField] private bool applyAmbient = true;
        [SerializeField] private bool applySFX = true;
        [SerializeField] private bool applyAudioSettings = true;
        
        private AudioThemeData currentThemeData;
        private Dictionary<AudioSource, float> originalVolumes = new Dictionary<AudioSource, float>();
        private Coroutine musicFadeCoroutine;
        private Coroutine ambientFadeCoroutine;
        
        protected override void Start()
        {
            base.Start();
            CacheOriginalVolumes();
            AutoFindAudioElements();
        }
        
        /// <summary>
        /// Caches original volumes for fade effects
        /// </summary>
        private void CacheOriginalVolumes()
        {
            originalVolumes.Clear();
            
            foreach (var audioSource in audioSources)
            {
                if (audioSource != null)
                    originalVolumes[audioSource] = audioSource.volume;
            }
        }
        
        /// <summary>
        /// Automatically finds audio elements in the scene
        /// </summary>
        private void AutoFindAudioElements()
        {
            if (audioSources == null || audioSources.Length == 0)
                audioSources = GetComponentsInChildren<AudioSource>();
                
            // Find or create dedicated audio sources
            if (backgroundMusicSource == null)
            {
                backgroundMusicSource = gameObject.AddComponent<AudioSource>();
                backgroundMusicSource.loop = true;
                backgroundMusicSource.playOnAwake = false;
            }
            
            if (ambientSoundSource == null)
            {
                ambientSoundSource = gameObject.AddComponent<AudioSource>();
                ambientSoundSource.loop = true;
                ambientSoundSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
        }
        
        protected override void OnThemeApplied(ITheme theme)
        {
            if (theme is AudioTheme audioTheme)
            {
                ApplyAudioTheme(audioTheme.ThemeData);
            }
        }
        
        /// <summary>
        /// Applies audio theme data to all audio elements
        /// </summary>
        /// <param name="themeData">The audio theme data to apply</param>
        public void ApplyAudioTheme(AudioThemeData themeData)
        {
            currentThemeData = themeData;
            
            if (applyAudioSettings)
                ApplyAudioSettings(themeData);
                
            if (applyMusic)
                ApplyMusicTheme(themeData);
                
            if (applyAmbient)
                ApplyAmbientTheme(themeData);
                
            if (applySFX)
                ApplySFXTheme(themeData);
                
            // Apply audio filters
            ApplyAudioFilters(themeData);
            
            // Apply spatial audio settings
            ApplySpatialAudioSettings(themeData);
        }
        
        private void ApplyAudioSettings(AudioThemeData themeData)
        {
            // Set master volume
            AudioListener.volume = themeData.masterVolume;
            
            // Apply to all audio sources
            foreach (var audioSource in audioSources)
            {
                if (audioSource == null) continue;
                
                audioSource.volume = originalVolumes.ContainsKey(audioSource) ? 
                    originalVolumes[audioSource] * themeData.masterVolume : themeData.masterVolume;
            }
        }
        
        private void ApplyMusicTheme(AudioThemeData themeData)
        {
            if (backgroundMusicSource == null || themeData.backgroundMusic == null)
                return;
                
            // Stop current music with fade out
            if (backgroundMusicSource.isPlaying)
            {
                if (musicFadeCoroutine != null)
                    StopCoroutine(musicFadeCoroutine);
                    
                musicFadeCoroutine = StartCoroutine(FadeOutMusic(themeData.musicFadeOutTime, () =>
                {
                    StartNewMusic(themeData);
                }));
            }
            else
            {
                StartNewMusic(themeData);
            }
        }
        
        private void StartNewMusic(AudioThemeData themeData)
        {
            backgroundMusicSource.clip = themeData.backgroundMusic;
            backgroundMusicSource.volume = themeData.musicVolume * themeData.masterVolume;
            backgroundMusicSource.loop = themeData.loopMusic;
            backgroundMusicSource.outputAudioMixerGroup = themeData.musicMixerGroup;
            
            if (themeData.enableAudio)
            {
                backgroundMusicSource.Play();
                
                // Fade in music
                if (themeData.musicFadeInTime > 0f)
                {
                    backgroundMusicSource.volume = 0f;
                    musicFadeCoroutine = StartCoroutine(FadeInMusic(themeData.musicFadeInTime, themeData.musicVolume));
                }
            }
        }
        
        private void ApplyAmbientTheme(AudioThemeData themeData)
        {
            if (ambientSoundSource == null || themeData.ambientSounds == null || themeData.ambientSounds.Length == 0)
                return;
                
            // Stop current ambient with fade out
            if (ambientSoundSource.isPlaying)
            {
                if (ambientFadeCoroutine != null)
                    StopCoroutine(ambientFadeCoroutine);
                    
                ambientFadeCoroutine = StartCoroutine(FadeOutAmbient(themeData.ambientFadeOutTime, () =>
                {
                    StartNewAmbient(themeData);
                }));
            }
            else
            {
                StartNewAmbient(themeData);
            }
        }
        
        private void StartNewAmbient(AudioThemeData themeData)
        {
            // Select random ambient sound
            var ambientClip = themeData.ambientSounds[Random.Range(0, themeData.ambientSounds.Length)];
            
            ambientSoundSource.clip = ambientClip;
            ambientSoundSource.volume = themeData.ambientVolume * themeData.masterVolume;
            ambientSoundSource.loop = themeData.loopAmbient;
            ambientSoundSource.outputAudioMixerGroup = themeData.ambientMixerGroup;
            
            if (themeData.enableAudio)
            {
                ambientSoundSource.Play();
                
                // Fade in ambient
                if (themeData.ambientFadeInTime > 0f)
                {
                    ambientSoundSource.volume = 0f;
                    ambientFadeCoroutine = StartCoroutine(FadeInAmbient(themeData.ambientFadeInTime, themeData.ambientVolume));
                }
            }
        }
        
        private void ApplySFXTheme(AudioThemeData themeData)
        {
            if (sfxSource == null)
                return;
                
            sfxSource.volume = themeData.sfxVolume * themeData.masterVolume;
            sfxSource.outputAudioMixerGroup = themeData.sfxMixerGroup;
        }
        
        private void ApplyAudioFilters(AudioThemeData themeData)
        {
            foreach (var audioSource in audioSources)
            {
                if (audioSource == null) continue;
                
                // Apply Low Pass Filter
                var lowPassFilter = audioSource.GetComponent<AudioLowPassFilter>();
                if (themeData.enableLowPassFilter)
                {
                    if (lowPassFilter == null)
                        lowPassFilter = audioSource.gameObject.AddComponent<AudioLowPassFilter>();
                        
                    lowPassFilter.cutoffFrequency = themeData.lowPassCutoff;
                    lowPassFilter.lowpassResonanceQ = themeData.lowPassResonance;
                }
                else if (lowPassFilter != null)
                {
                    DestroyImmediate(lowPassFilter);
                }
                
                // Apply High Pass Filter
                var highPassFilter = audioSource.GetComponent<AudioHighPassFilter>();
                if (themeData.enableHighPassFilter)
                {
                    if (highPassFilter == null)
                        highPassFilter = audioSource.gameObject.AddComponent<AudioHighPassFilter>();
                        
                    highPassFilter.cutoffFrequency = themeData.highPassCutoff;
                    highPassFilter.highpassResonanceQ = themeData.highPassResonance;
                }
                else if (highPassFilter != null)
                {
                    DestroyImmediate(highPassFilter);
                }
                
                // Apply Echo Filter
                var echoFilter = audioSource.GetComponent<AudioEchoFilter>();
                if (themeData.enableEchoFilter)
                {
                    if (echoFilter == null)
                        echoFilter = audioSource.gameObject.AddComponent<AudioEchoFilter>();
                        
                    echoFilter.delay = themeData.echoDelay;
                    echoFilter.decayRatio = themeData.echoDecayRatio;
                }
                else if (echoFilter != null)
                {
                    DestroyImmediate(echoFilter);
                }
                
                // Apply Distortion Filter
                var distortionFilter = audioSource.GetComponent<AudioDistortionFilter>();
                if (themeData.enableDistortionFilter)
                {
                    if (distortionFilter == null)
                        distortionFilter = audioSource.gameObject.AddComponent<AudioDistortionFilter>();
                        
                    distortionFilter.distortionLevel = themeData.distortionLevel;
                }
                else if (distortionFilter != null)
                {
                    DestroyImmediate(distortionFilter);
                }
            }
        }
        
        private void ApplySpatialAudioSettings(AudioThemeData themeData)
        {
            foreach (var audioSource in audioSources)
            {
                if (audioSource == null) continue;
                
                if (themeData.enableSpatialAudio)
                {
                    audioSource.spatialBlend = themeData.spatialBlend;
                    audioSource.dopplerLevel = themeData.dopplerLevel;
                    audioSource.spread = themeData.spread;
                    audioSource.minDistance = themeData.minDistance;
                    audioSource.maxDistance = themeData.maxDistance;
                    audioSource.rolloffMode = AudioRolloffMode.Custom;
                    
                    // Set custom rolloff curve
                    var animationCurve = audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
                    if (animationCurve.keys.Length == 0)
                    {
                        animationCurve = themeData.rolloffMode;
                        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, animationCurve);
                    }
                }
                else
                {
                    audioSource.spatialBlend = 0f; // 2D audio
                }
            }
        }
        
        private IEnumerator FadeOutMusic(float duration, System.Action onComplete)
        {
            float startVolume = backgroundMusicSource.volume;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }
            
            backgroundMusicSource.Stop();
            backgroundMusicSource.volume = startVolume;
            onComplete?.Invoke();
        }
        
        private IEnumerator FadeInMusic(float duration, float targetVolume)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                backgroundMusicSource.volume = Mathf.Lerp(0f, targetVolume * currentThemeData.masterVolume, t);
                yield return null;
            }
            
            backgroundMusicSource.volume = targetVolume * currentThemeData.masterVolume;
        }
        
        private IEnumerator FadeOutAmbient(float duration, System.Action onComplete)
        {
            float startVolume = ambientSoundSource.volume;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                ambientSoundSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }
            
            ambientSoundSource.Stop();
            ambientSoundSource.volume = startVolume;
            onComplete?.Invoke();
        }
        
        private IEnumerator FadeInAmbient(float duration, float targetVolume)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                ambientSoundSource.volume = Mathf.Lerp(0f, targetVolume * currentThemeData.masterVolume, t);
                yield return null;
            }
            
            ambientSoundSource.volume = targetVolume * currentThemeData.masterVolume;
        }
        
        /// <summary>
        /// Plays a UI sound effect
        /// </summary>
        /// <param name="soundIndex">Index of the sound in the UI sound effects array</param>
        public void PlayUISound(int soundIndex)
        {
            if (currentThemeData == null || currentThemeData.uiSoundEffects == null || 
                soundIndex < 0 || soundIndex >= currentThemeData.uiSoundEffects.Length)
                return;
                
            PlaySFX(currentThemeData.uiSoundEffects[soundIndex]);
        }
        
        /// <summary>
        /// Plays a sound effect
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        public void PlaySFX(AudioClip clip)
        {
            if (sfxSource == null || clip == null)
                return;
                
            sfxSource.PlayOneShot(clip);
        }
        
        /// <summary>
        /// Stops all audio
        /// </summary>
        public void StopAllAudio()
        {
            if (backgroundMusicSource != null)
                backgroundMusicSource.Stop();
                
            if (ambientSoundSource != null)
                ambientSoundSource.Stop();
                
            if (sfxSource != null)
                sfxSource.Stop();
        }
        
        /// <summary>
        /// Pauses all audio
        /// </summary>
        public void PauseAllAudio()
        {
            if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
                backgroundMusicSource.Pause();
                
            if (ambientSoundSource != null && ambientSoundSource.isPlaying)
                ambientSoundSource.Pause();
        }
        
        /// <summary>
        /// Resumes all audio
        /// </summary>
        public void ResumeAllAudio()
        {
            if (backgroundMusicSource != null)
                backgroundMusicSource.UnPause();
                
            if (ambientSoundSource != null)
                ambientSoundSource.UnPause();
        }
        
        [ContextMenu("Auto-Find Audio Elements")]
        private void AutoFindAudioElementsMenu()
        {
            AutoFindAudioElements();
        }
    }
}
