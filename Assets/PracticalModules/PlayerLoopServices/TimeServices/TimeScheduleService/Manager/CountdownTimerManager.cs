using System;
using System.Collections.Generic;
using System.Linq;
using PracticalModules.PlayerLoopServices.Core.Handlers;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Data;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeFactory;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent;
using PracticalModules.PlayerLoopServices.UpdateServices;
using UnityEngine;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Manager
{
    public class CountdownTimerManager : ICountdownTimerManager, IUpdateHandler
    {
        private readonly CountdownTimerFactory _timerFactory;
        private readonly Dictionary<string, ICountdownTimer> _timers;
        private readonly List<string> _expiredTimerKeys;
        private bool _disposed;

        public int ActiveTimerCount => this._timers.Count;
        public event Action<string> OnTimerCompleted;
        public event Action<string, float> OnTimerCreated;
        public event Action<string> OnTimerRemoved;

        public CountdownTimerManager()
        {
            _timerFactory = new();
            this._timers = new ();
            this._expiredTimerKeys = new();
            UpdateServiceManager.RegisterUpdateHandler(this);
        }

        public ICountdownTimer GetOrCreateTimer(string key, float durationSeconds)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }
            
            if (this._timers.TryGetValue(key, out var existingTimer))
                return existingTimer;
            
            var newTimer = _timerFactory.Produce( new TimeSchedulerConfig
            {
                Key = key,
                Duration = durationSeconds
            });
            
            this._timers[key] = newTimer;
            newTimer.OnComplete += () => this.HandleTimerCompleted(key);
            this.OnTimerCreated?.Invoke(key, durationSeconds);
            return newTimer;
        }

        public ICountdownTimer GetTimer(string key) => this._timers.GetValueOrDefault(key);

        public bool HasTimer(string key) => this._timers.ContainsKey(key);

        public bool RemoveTimer(string key)
        {
            if (!this._timers.TryGetValue(key, out var timer))
                return false;

            timer.Dispose();
            this._timers.Remove(key);
            this.OnTimerRemoved?.Invoke(key);
            return true;
        }

        public IReadOnlyDictionary<string, ICountdownTimer> GetAllTimers() => this._timers;
        
        public void Tick(float deltaTime) => UpdateTimers(deltaTime);

        private void UpdateTimers(float deltaTime)
        {
            if (this._disposed)
                return;
            
            foreach (var timer in this._timers.Values)
            {
                timer.UpdateRealTime();
            }
            
            this.ProcessExpiredTimers();
        }

        public void SaveAllTimers()
        {
            var timerDataList = new List<CountdownTimerData>();
            
            foreach (var timer in this._timers.Values)
            {
                if (timer.IsActive)
                {
                    timerDataList.Add(timer.GetSaveData());
                }
            }

            this.SaveTimerData(timerDataList);
        }

        public void LoadAllTimers()
        {
            var timerDataList = this.LoadTimerData();
            foreach (var data in timerDataList)
            {
                try
                {
                    var timer = _timerFactory.ProduceFromSaveData(data);
                    
                    if (timer.IsActive)
                    {
                        this._timers[data.key] = timer;
                        timer.OnComplete += () => this.HandleTimerCompleted(data.key);
                    }
                    else
                    {
                        this.OnTimerCompleted?.Invoke(data.key);
                        timer.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to load timer {data.key}: {ex.Message}");
                }
            }
        }

        public void ClearAllTimers()
        {
            foreach (var timer in this._timers.Values)
            {
                timer.Dispose();
            }
            
            this._timers.Clear();
        }

        private void ProcessExpiredTimers()
        {
            this._expiredTimerKeys.Clear();
            
            foreach (var kvp in this._timers)
            {
                if (!kvp.Value.IsActive && kvp.Value.IsExpired)
                {
                    this._expiredTimerKeys.Add(kvp.Key);
                }
            }
            
            foreach (var key in this._expiredTimerKeys)
            {
                this.RemoveTimer(key);
            }
        }

        private void HandleTimerCompleted(string key)
        {
            this.OnTimerCompleted?.Invoke(key);
            this.RemoveTimer(key);
        }

        public void SaveTimerData(List<CountdownTimerData> timerDataList)
        {
            // Implement lưu trữ theo nhu cầu (PlayerPrefs, JSON file, etc.)
            var json = JsonUtility.ToJson(new SerializableList<CountdownTimerData>(timerDataList));
            PlayerPrefs.SetString("CountdownTimers", json);
            PlayerPrefs.Save();
        }

        public List<CountdownTimerData> LoadTimerData()
        {
            // Implement tải dữ liệu theo nhu cầu
            var json = PlayerPrefs.GetString("CountdownTimers", string.Empty);
            
            if (string.IsNullOrEmpty(json))
            {
                return new List<CountdownTimerData>();
            }

            try
            {
                var serializableList = JsonUtility.FromJson<SerializableList<CountdownTimerData>>(json);
                return serializableList?.items?.ToList() ?? new List<CountdownTimerData>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load timer data: {ex.Message}");
                return new List<CountdownTimerData>();
            }
        }

        public void Dispose()
        {
            if (this._disposed)
                return;

            this.SaveAllTimers();
            this.ClearAllTimers();
            UpdateServiceManager.DeregisterUpdateHandler(this);
            
            this.OnTimerCompleted = null;
            this.OnTimerCreated = null;
            this.OnTimerRemoved = null;
            
            this._disposed = true;
        }

        [Serializable]
        private class SerializableList<T>
        {
            public T[] items;
            
            public SerializableList(List<T> list)
            {
                this.items = list.ToArray();
            }
        }
    }
}
