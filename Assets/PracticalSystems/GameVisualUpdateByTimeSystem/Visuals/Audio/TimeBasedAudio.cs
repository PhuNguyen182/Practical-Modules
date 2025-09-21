using System;
using System.Collections.Generic;
using UnityEngine;
using GameVisualUpdateByTimeSystem.Core.Interfaces;

namespace GameVisualUpdateByTimeSystem.Visuals.Audio
{
    /// <summary>
    /// Manages audio changes based on time of day, month, and season
    /// Supports background music, ambient sounds, and audio effects
    /// </summary>
    public class TimeBasedAudio : BaseVisualComponent, IInterpolatableVisual<AudioState>
    {
        [Header("Audio Configuration")]
        [SerializeField] private AudioSource[] _audioSources = new AudioSource[0];
        [SerializeField] private bool _autoFindAudioSources = true;
        [SerializeField] private bool _controlVolume = true;
        [SerializeField] private bool _controlPitch = true;
        [SerializeField] private bool _controlPan = true;
        
        [Header("Audio States")]
        [SerializeField] private AudioState[] _audioStates = new AudioState[0];
        
        [Header("Performance")]
        [SerializeField] private float _updateInterval = 0.1f;
        [SerializeField] private bool _fadeBetweenStates = true;
        [SerializeField] private float _fadeDuration = 2f;
        
        private AudioState _currentState;
        private AudioState _targetState;
        private float _transitionProgress;
        private float _lastStateUpdateTime;
        
        // Fade tracking
        private Dictionary<AudioSource, AudioFadeInfo> _fadeInfo = new Dictionary<AudioSource, AudioFadeInfo>();
        
        protected override void OnInitialize()
        {
            // Auto-find audio sources if not assigned
            if (_autoFindAudioSources && _audioSources.Length == 0)
            {
                var allAudioSources = FindObjectsOfType<AudioSource>();
                var filteredSources = new List<AudioSource>();
                
                foreach (var source in allAudioSources)
                {
                    // Skip audio sources that are likely UI or effect sounds
                    if (source.playOnAwake && source.loop)
                    {
                        filteredSources.Add(source);
                    }
                }
                
                _audioSources = filteredSources.ToArray();
                
                if (_audioSources.Length == 0)
                {
                    LogWarning("No suitable audio sources found. Component may not function correctly.");
                }
            }
            
            // Validate audio states
            if (_audioStates.Length == 0)
            {
                LogWarning("No audio states configured. Using default states.");
                CreateDefaultAudioStates();
            }
            
            // Initialize with first state
            if (_audioStates.Length > 0)
            {
                _currentState = _audioStates[0];
                _targetState = _audioStates[0];
                ApplyAudioState(_currentState);
            }
            
            LogInfo($"Initialized with {_audioSources.Length} audio sources and {_audioStates.Length} states");
        }
        
        protected override void OnUpdateVisual(ITimeProvider timeProvider, float deltaTime)
        {
            // Update audio state based on current time
            UpdateAudioState(timeProvider);
            
            // Handle fading between states
            if (_fadeBetweenStates && _transitionProgress < 1f)
            {
                _transitionProgress += deltaTime / _fadeDuration;
                _transitionProgress = Mathf.Clamp01(_transitionProgress);
                
                UpdateAudioFading(deltaTime);
                
                if (_transitionProgress >= 1f)
                {
                    _currentState = _targetState;
                    CompleteAudioTransition();
                }
            }
            else if (!_fadeBetweenStates && _transitionProgress < 1f)
            {
                _transitionProgress += deltaTime / _transitionDuration;
                _transitionProgress = Mathf.Clamp01(_transitionProgress);
                
                var interpolatedState = InterpolateAudioStates(_currentState, _targetState, _transitionProgress);
                ApplyAudioState(interpolatedState);
                
                if (_transitionProgress >= 1f)
                {
                    _currentState = _targetState;
                }
            }
        }
        
        private void UpdateAudioState(ITimeProvider timeProvider)
        {
            if (Time.time - _lastStateUpdateTime < _updateInterval) return;
            
            var newTargetState = GetAudioStateForTime(timeProvider);
            if (newTargetState.StateId != _targetState.StateId)
            {
                _currentState = GetCurrentAudioState();
                _targetState = newTargetState;
                _transitionProgress = 0f;
                _lastStateUpdateTime = Time.time;
                
                if (_fadeBetweenStates)
                {
                    StartAudioTransition();
                }
                
                LogInfo($"Transitioning to audio state: {_targetState.StateId}");
            }
        }
        
        private AudioState GetAudioStateForTime(ITimeProvider timeProvider)
        {
            // Find the most appropriate audio state for current time
            AudioState bestState = _audioStates[0];
            float bestScore = float.MaxValue;
            
            foreach (var state in _audioStates)
            {
                float score = CalculateStateScore(state, timeProvider);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestState = state;
                }
            }
            
            return bestState;
        }
        
        private float CalculateStateScore(AudioState state, ITimeProvider timeProvider)
        {
            float score = 0f;
            var condition = state.TimeCondition;
            
            // Hour-based scoring
            if (condition.Hour != -1)
            {
                int hourDiff = Mathf.Abs(timeProvider.CurrentHour - condition.Hour);
                score += hourDiff * 10f;
            }
            
            // Season-based scoring
            if (condition.UseSeason)
            {
                var currentSeason = GetSeasonFromTime(timeProvider);
                if (currentSeason != condition.Season)
                {
                    score += 50f;
                }
            }
            
            // Month-based scoring
            if (condition.Month != -1)
            {
                int monthDiff = Mathf.Abs(timeProvider.CurrentMonth - condition.Month);
                score += monthDiff * 5f;
            }
            
            return score;
        }
        
        private Season GetSeasonFromTime(ITimeProvider timeProvider)
        {
            return timeProvider.CurrentMonth switch
            {
                3 or 4 or 5 => Season.Spring,
                6 or 7 or 8 => Season.Summer,
                9 or 10 or 11 => Season.Autumn,
                _ => Season.Winter
            };
        }
        
        private void ApplyAudioState(AudioState state)
        {
            for (int i = 0; i < _audioSources.Length && i < state.AudioSources.Length; i++)
            {
                var audioSource = _audioSources[i];
                var sourceState = state.AudioSources[i];
                
                if (audioSource == null) continue;
                
                ApplyAudioSourceState(audioSource, sourceState);
            }
        }
        
        private void ApplyAudioSourceState(AudioSource audioSource, AudioSourceState sourceState)
        {
            // Apply audio clip
            if (sourceState.AudioClip != null && audioSource.clip != sourceState.AudioClip)
            {
                audioSource.clip = sourceState.AudioClip;
                if (sourceState.PlayOnTransition)
                {
                    audioSource.Play();
                }
            }
            
            // Apply volume
            if (_controlVolume)
            {
                audioSource.volume = sourceState.Volume;
            }
            
            // Apply pitch
            if (_controlPitch)
            {
                audioSource.pitch = sourceState.Pitch;
            }
            
            // Apply pan
            if (_controlPan)
            {
                audioSource.panStereo = sourceState.Pan;
            }
            
            // Apply other properties
            audioSource.loop = sourceState.Loop;
            audioSource.mute = sourceState.Mute;
        }
        
        private AudioState InterpolateAudioStates(AudioState from, AudioState to, float t)
        {
            var result = new AudioState
            {
                stateId = to.StateId,
                timeCondition = to.TimeCondition,
                AudioSources = new AudioSourceState[Mathf.Max(from.AudioSources.Length, to.AudioSources.Length)]
            };
            
            for (int i = 0; i < result.AudioSources.Length; i++)
            {
                var fromSource = i < from.AudioSources.Length ? from.AudioSources[i] : new AudioSourceState();
                var toSource = i < to.AudioSources.Length ? to.AudioSources[i] : new AudioSourceState();
                
                result.AudioSources[i] = new AudioSourceState
                {
                    AudioClip = t > 0.5f ? toSource.AudioClip : fromSource.AudioClip,
                    Volume = SmoothInterpolate(fromSource.Volume, toSource.Volume, t),
                    Pitch = SmoothInterpolate(fromSource.Pitch, toSource.Pitch, t),
                    Pan = SmoothInterpolate(fromSource.Pan, toSource.Pan, t),
                    Loop = toSource.Loop,
                    Mute = toSource.Mute,
                    PlayOnTransition = toSource.PlayOnTransition
                };
            }
            
            return result;
        }
        
        private AudioState GetCurrentAudioState()
        {
            var state = new AudioState
            {
                stateId = _currentState.StateId,
                timeCondition = _currentState.TimeCondition,
                AudioSources = new AudioSourceState[_audioSources.Length]
            };
            
            // Capture current audio state
            for (int i = 0; i < _audioSources.Length; i++)
            {
                var audioSource = _audioSources[i];
                if (audioSource != null)
                {
                    state.AudioSources[i] = new AudioSourceState
                    {
                        AudioClip = audioSource.clip,
                        Volume = audioSource.volume,
                        Pitch = audioSource.pitch,
                        Pan = audioSource.panStereo,
                        Loop = audioSource.loop,
                        Mute = audioSource.mute,
                        PlayOnTransition = false
                    };
                }
            }
            
            return state;
        }
        
        private void StartAudioTransition()
        {
            _fadeInfo.Clear();
            
            // Setup fade info for each audio source
            for (int i = 0; i < _audioSources.Length; i++)
            {
                var audioSource = _audioSources[i];
                if (audioSource == null) continue;
                
                var currentSourceState = i < _currentState.AudioSources.Length ? _currentState.AudioSources[i] : new AudioSourceState();
                var targetSourceState = i < _targetState.AudioSources.Length ? _targetState.AudioSources[i] : new AudioSourceState();
                
                _fadeInfo[audioSource] = new AudioFadeInfo
                {
                    StartVolume = audioSource.volume,
                    TargetVolume = targetSourceState.Volume,
                    StartPitch = audioSource.pitch,
                    TargetPitch = targetSourceState.Pitch,
                    StartPan = audioSource.panStereo,
                    TargetPan = targetSourceState.Pan,
                    StartClip = audioSource.clip,
                    TargetClip = targetSourceState.AudioClip,
                    FadeStartTime = Time.time
                };
            }
        }
        
        private void UpdateAudioFading(float deltaTime)
        {
            foreach (var kvp in _fadeInfo)
            {
                var audioSource = kvp.Key;
                var fadeInfo = kvp.Value;
                
                if (audioSource == null) continue;
                
                float fadeProgress = Mathf.Clamp01(_transitionProgress);
                
                // Fade volume
                if (_controlVolume)
                {
                    audioSource.volume = Mathf.Lerp(fadeInfo.StartVolume, fadeInfo.TargetVolume, fadeProgress);
                }
                
                // Fade pitch
                if (_controlPitch)
                {
                    audioSource.pitch = Mathf.Lerp(fadeInfo.StartPitch, fadeInfo.TargetPitch, fadeProgress);
                }
                
                // Fade pan
                if (_controlPan)
                {
                    audioSource.panStereo = Mathf.Lerp(fadeInfo.StartPan, fadeInfo.TargetPan, fadeProgress);
                }
                
                // Handle audio clip transitions
                if (fadeInfo.TargetClip != fadeInfo.StartClip)
                {
                    if (fadeProgress > 0.5f && audioSource.clip != fadeInfo.TargetClip)
                    {
                        audioSource.clip = fadeInfo.TargetClip;
                        if (fadeInfo.TargetClip != null)
                        {
                            audioSource.Play();
                        }
                    }
                }
            }
        }
        
        private void CompleteAudioTransition()
        {
            ApplyAudioState(_targetState);
            _fadeInfo.Clear();
        }
        
        private void CreateDefaultAudioStates()
        {
            _audioStates = new AudioState[]
            {
                // Day audio
                new AudioState
                {
                    stateId = "Day",
                    timeCondition = new TimeCondition(12, -1, -1, -1, TimeType.Hour),
                    AudioSources = new AudioSourceState[]
                    {
                        new AudioSourceState
                        {
                            Volume = 0.8f,
                            Pitch = 1.0f,
                            Pan = 0f,
                            Loop = true,
                            Mute = false
                        }
                    }
                },
                
                // Night audio
                new AudioState
                {
                    stateId = "Night",
                    timeCondition = new TimeCondition(0, -1, -1, -1, TimeType.Hour),
                    AudioSources = new AudioSourceState[]
                    {
                        new AudioSourceState
                        {
                            Volume = 0.4f,
                            Pitch = 0.8f,
                            Pan = 0f,
                            Loop = true,
                            Mute = false
                        }
                    }
                }
            };
        }
        
        #region IInterpolatableVisual Implementation
        
        public void Interpolate(AudioState from, AudioState to, float t)
        {
            var interpolatedState = InterpolateAudioStates(from, to, t);
            ApplyAudioState(interpolatedState);
        }
        
        public AudioState GetCurrentState()
        {
            return GetCurrentAudioState();
        }
        
        public void SetState(AudioState state)
        {
            _currentState = state;
            _targetState = state;
            _transitionProgress = 1f;
            ApplyAudioState(state);
        }
        
        #endregion
        
        #region Editor Helpers
        
        [ContextMenu("Create Default Audio States")]
        private void EditorCreateDefaultStates()
        {
            CreateDefaultAudioStates();
            LogInfo("Created default audio states");
        }
        
        [ContextMenu("Apply Current State")]
        private void EditorApplyCurrentState()
        {
            if (_currentState.StateId != null)
            {
                ApplyAudioState(_currentState);
                LogInfo($"Applied audio state: {_currentState.StateId}");
            }
        }
        
        [ContextMenu("Find Audio Sources")]
        private void EditorFindAudioSources()
        {
            var allAudioSources = FindObjectsOfType<AudioSource>();
            var filteredSources = new List<AudioSource>();
            
            foreach (var source in allAudioSources)
            {
                if (source.playOnAwake && source.loop)
                {
                    filteredSources.Add(source);
                }
            }
            
            _audioSources = filteredSources.ToArray();
            LogInfo($"Found {_audioSources.Length} audio sources");
        }
        
        #endregion
    }
    
    /// <summary>
    /// Information for audio fading between states
    /// </summary>
    [Serializable]
    public class AudioFadeInfo
    {
        public float StartVolume;
        public float TargetVolume;
        public float StartPitch;
        public float TargetPitch;
        public float StartPan;
        public float TargetPan;
        public AudioClip StartClip;
        public AudioClip TargetClip;
        public float FadeStartTime;
    }
}
