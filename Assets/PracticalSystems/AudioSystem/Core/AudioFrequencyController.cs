using System.Collections.Generic;
using PracticalSystems.AudioSystem.Interfaces;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Core
{
    /// <summary>
    /// Controls audio playback frequency to prevent audio spam
    /// Manages concurrent instance limits and minimum time between plays
    /// Essential for high-frequency audio like gunfire
    /// </summary>
    public class AudioFrequencyController
    {
        private class AudioPlaybackInfo
        {
            public float LastPlayTime = -1f;
            public readonly List<IAudioHandle> ActiveInstances = new();
        }
        
        private readonly Dictionary<string, AudioPlaybackInfo> _audioPlaybackInfos = new();

        /// <summary>
        /// Checks if audio can be played based on frequency limits
        /// </summary>
        public bool CanPlayAudio(IAudioEntry audioEntry)
        {
            if (audioEntry == null)
            {
                return false;
            }
            
            var audioId = audioEntry.AudioId;
            
            if (!this._audioPlaybackInfos.TryGetValue(audioId, out var playbackInfo))
            {
                playbackInfo = new AudioPlaybackInfo();
                this._audioPlaybackInfos[audioId] = playbackInfo;
            }
            
            // Check minimum time between plays
            if (audioEntry.MinTimeBetweenPlays > 0f)
            {
                var timeSinceLastPlay = Time.time - playbackInfo.LastPlayTime;
                if (timeSinceLastPlay < audioEntry.MinTimeBetweenPlays)
                {
                    return false;
                }
            }
            
            // Check concurrent instance limit
            this.CleanupInactiveInstances(playbackInfo);
            
            if (audioEntry.MaxConcurrentInstances > 0)
            {
                if (playbackInfo.ActiveInstances.Count >= audioEntry.MaxConcurrentInstances)
                {
                    // Stop oldest instance if limit reached
                    this.StopOldestInstance(playbackInfo);
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Registers a new audio playback instance
        /// </summary>
        public void RegisterPlayback(IAudioEntry audioEntry, IAudioHandle audioHandle)
        {
            if (audioEntry == null || audioHandle == null)
            {
                return;
            }
            
            var audioId = audioEntry.AudioId;
            
            if (!this._audioPlaybackInfos.TryGetValue(audioId, out var playbackInfo))
            {
                playbackInfo = new AudioPlaybackInfo();
                this._audioPlaybackInfos[audioId] = playbackInfo;
            }
            
            playbackInfo.LastPlayTime = Time.time;
            playbackInfo.ActiveInstances.Add(audioHandle);
        }
        
        /// <summary>
        /// Gets the number of active instances for a specific audio
        /// </summary>
        public int GetActiveInstanceCount(string audioId)
        {
            if (!this._audioPlaybackInfos.TryGetValue(audioId, out var playbackInfo))
            {
                return 0;
            }
            
            this.CleanupInactiveInstances(playbackInfo);
            return playbackInfo.ActiveInstances.Count;
        }
        
        /// <summary>
        /// Stops all instances of a specific audio
        /// </summary>
        public void StopAllInstances(string audioId, float fadeOutDuration = 0f)
        {
            if (!this._audioPlaybackInfos.TryGetValue(audioId, out var playbackInfo))
            {
                return;
            }
            
            foreach (var instance in playbackInfo.ActiveInstances.ToArray())
            {
                if (instance != null && instance.IsPlaying)
                {
                    instance.Stop(fadeOutDuration);
                }
            }
            
            playbackInfo.ActiveInstances.Clear();
        }
        
        /// <summary>
        /// Cleans up inactive audio instances
        /// </summary>
        private void CleanupInactiveInstances(AudioPlaybackInfo playbackInfo)
        {
            playbackInfo.ActiveInstances.RemoveAll(handle => handle == null || !handle.IsPlaying);
        }
        
        /// <summary>
        /// Stops the oldest audio instance when limit is reached
        /// </summary>
        private void StopOldestInstance(AudioPlaybackInfo playbackInfo)
        {
            if (playbackInfo.ActiveInstances.Count == 0)
            {
                return;
            }
            
            var oldestInstance = playbackInfo.ActiveInstances[0];
            oldestInstance?.Stop();
            playbackInfo.ActiveInstances.RemoveAt(0);
        }
    }
}

