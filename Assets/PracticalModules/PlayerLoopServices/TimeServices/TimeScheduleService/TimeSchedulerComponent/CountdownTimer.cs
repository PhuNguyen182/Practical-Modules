using System;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Data;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Extensions;
using UnityEngine;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent
{
    public class CountdownTimer : ICountdownTimer
    {
        private const float UpdateThresholdSeconds = 1f;
        
        private bool _disposed;
        private string _key;
        private long _endTimeUnix;
        private long _startTimeUnix;
        private float _totalDuration;
        private float _cachedRemainingSeconds;
        private bool _isExpired;
        private bool _isPaused;
        private long _pausedTimeUnix;
        private long _lastUpdateTimeUnix;
        
        public string Key => this._key;
        
        public float RemainingSeconds
        {
            get
            {
                if (this._isExpired)
                {
                    return 0f;
                }
                
                if (this._isPaused)
                {
                    return this._cachedRemainingSeconds;
                }
                
                this.UpdateRealTime();
                return this._cachedRemainingSeconds;
            }
        }
        
        public TimeSpan RemainingTime => TimeSpan.FromSeconds(this.RemainingSeconds);
        
        public float TotalDuration => this._totalDuration;
        
        public bool IsActive => !this._isExpired && !this._isPaused && this.RemainingSeconds > 0f;
        public bool IsExpired => this._isExpired;
        public bool IsPaused => this._isPaused;
        
        public event Action<float> OnUpdate;
        public event Action OnComplete;
        
        public CountdownTimer(string key, float durationSeconds, bool startPaused = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }
            
            if (durationSeconds <= 0f)
            {
                throw new ArgumentException("Duration must be positive", nameof(durationSeconds));
            }

            this._key = key;
            this._totalDuration = durationSeconds;
            this._startTimeUnix = TimeExtensions.GetCurrentUtcTimestampInSeconds();
            this._endTimeUnix = this._startTimeUnix + (long)durationSeconds;
            this._cachedRemainingSeconds = durationSeconds;
            this._isExpired = false;
            this._isPaused = startPaused;
            this._pausedTimeUnix = startPaused ? this._startTimeUnix : 0;
            this._lastUpdateTimeUnix = this._startTimeUnix;
        }
        
        public CountdownTimer(CountdownTimerData data)
        {
            if (!this.InitializeFromSaveData(data))
            {
                throw new ArgumentException("Invalid save data", nameof(data));
            }
        }
        
        ~CountdownTimer() => this.Dispose(false);
        
        public void UpdateRealTime()
        {
            if (this._isExpired || this._isPaused)
            {
                return;
            }

            long currentTimeUnix = TimeExtensions.GetCurrentUtcTimestampInSeconds();
            
            // Performance optimization: only update if enough time has passed
            if (currentTimeUnix - this._lastUpdateTimeUnix < UpdateThresholdSeconds)
            {
                return;
            }
            
            this._lastUpdateTimeUnix = currentTimeUnix;
            long remainingTimeUnix = this._endTimeUnix - currentTimeUnix;
            this._cachedRemainingSeconds = Mathf.Max(0f, remainingTimeUnix);
            
            if (remainingTimeUnix <= 0)
            {
                this.Complete();
                return;
            }
            
            this.OnUpdate?.Invoke(this._cachedRemainingSeconds);
        }
        
        public CountdownTimerData GetSaveData()
        {
            CountdownTimerData data = new CountdownTimerData
            {
                key = this._key,
                endTimeUnix = this._endTimeUnix,
                startTimeUnix = this._startTimeUnix,
                totalDuration = this._totalDuration
            };
            
            return data;
        }
        
        public bool InitializeFromSaveData(CountdownTimerData data)
        {
            if (string.IsNullOrEmpty(data.key))
            {
                return false;
            }
            
            if (data.totalDuration <= 0f)
            {
                return false;
            }

            this._key = data.key;
            this._endTimeUnix = data.endTimeUnix;
            this._startTimeUnix = data.startTimeUnix;
            this._totalDuration = data.totalDuration;
            this._isExpired = false;
            this._isPaused = true; // Start in paused state when loading
            this._pausedTimeUnix = TimeExtensions.GetCurrentUtcTimestampInSeconds();
            this._lastUpdateTimeUnix = this._pausedTimeUnix;
            this.UpdateRealTime();
            return true;
        }
        
        public void Resume()
        {
            if (!this._isPaused || this._isExpired)
            {
                return;
            }

            long currentTimeUnix = TimeExtensions.GetCurrentUtcTimestampInSeconds();
            long pausedDuration = currentTimeUnix - this._pausedTimeUnix;
            
            // Adjust end time based on pause duration
            this._endTimeUnix += pausedDuration;
            this._isPaused = false;
            this._pausedTimeUnix = 0;
            this._lastUpdateTimeUnix = currentTimeUnix;
            
            this.UpdateRealTime();
        }
        
        public void Pause()
        {
            if (this._isPaused || this._isExpired)
            {
                return;
            }

            this._isPaused = true;
            this._pausedTimeUnix = TimeExtensions.GetCurrentUtcTimestampInSeconds();
            this.UpdateRealTime();
        }
        
        public void Complete()
        {
            if (this._isExpired)
                return;

            this._isExpired = true;
            this._cachedRemainingSeconds = 0f;
            this.OnComplete?.Invoke();
        }
        
        public void Reset(float newDuration)
        {
            if (newDuration <= 0f)
                return;

            this._totalDuration = newDuration;
            this._startTimeUnix = TimeExtensions.GetCurrentUtcTimestampInSeconds();
            this._endTimeUnix = this._startTimeUnix + (long)newDuration;
            this._cachedRemainingSeconds = newDuration;
            this._isExpired = false;
        }
        
        private void Dispose(bool disposing)
        {
            if (this._disposed) 
                return;

            if (disposing)
            {
                this.OnUpdate = null;
                this.OnComplete = null;
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
