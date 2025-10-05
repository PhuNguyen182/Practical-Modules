using System;
using System.Collections.Generic;
using System.Linq;
using PracticalModules.PlayerLoopServices.Core.Handlers;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Data;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent;
using PracticalModules.PlayerLoopServices.UpdateServices;
using UnityEngine;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Manager
{
    /// <summary>
    /// Quản lý các bộ đếm thời gian theo thời gian thực với khả năng lưu trữ
    /// </summary>
    public class CountdownTimerManager : ICountdownTimerManager, IUpdateHandler
    {
        private readonly Dictionary<string, ICountdownTimer> _timers;
        private readonly List<string> _expiredTimerKeys;
        private bool _disposed;

        /// <summary>
        /// Lấy số lượng bộ đếm đang hoạt động
        /// </summary>
        public int ActiveTimerCount => this._timers.Count;

        /// <summary>
        /// Sự kiện khi bộ đếm hoàn thành
        /// </summary>
        public event Action<string> OnTimerCompleted;

        /// <summary>
        /// Sự kiện khi bộ đếm được tạo
        /// </summary>
        public event Action<string, float> OnTimerCreated;

        /// <summary>
        /// Sự kiện khi bộ đếm bị xóa
        /// </summary>
        public event Action<string> OnTimerRemoved;

        public CountdownTimerManager()
        {
            this._timers = new Dictionary<string, ICountdownTimer>();
            this._expiredTimerKeys = new List<string>();
            UpdateServiceManager.RegisterUpdateHandler(this);
            this.LoadAllTimers();
        }

        /// <summary>
        /// Tạo hoặc lấy bộ đếm thời gian theo key
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <param name="durationSeconds">Thời gian đếm ngược (seconds)</param>
        /// <returns>Bộ đếm thời gian</returns>
        public ICountdownTimer GetOrCreateTimer(string key, float durationSeconds)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }

            // Nếu đã tồn tại, trả về bộ đếm hiện tại
            if (this._timers.TryGetValue(key, out var existingTimer))
            {
                return existingTimer;
            }

            // Tạo bộ đếm mới
            var newTimer = new CountdownTimer(key, durationSeconds);
            this._timers[key] = newTimer;
            
            // Đăng ký sự kiện hoàn thành
            newTimer.OnComplete += () => this.HandleTimerCompleted(key);
            
            this.OnTimerCreated?.Invoke(key, durationSeconds);
            
            return newTimer;
        }

        /// <summary>
        /// Lấy bộ đếm thời gian theo key
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <returns>Bộ đếm thời gian hoặc null nếu không tồn tại</returns>
        public ICountdownTimer GetTimer(string key)
        {
            return this._timers.GetValueOrDefault(key);
        }

        /// <summary>
        /// Kiểm tra bộ đếm có tồn tại không
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <returns>True nếu tồn tại</returns>
        public bool HasTimer(string key)
        {
            return this._timers.ContainsKey(key);
        }

        /// <summary>
        /// Xóa bộ đếm thời gian
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <returns>True nếu xóa thành công</returns>
        public bool RemoveTimer(string key)
        {
            if (!this._timers.TryGetValue(key, out var timer))
            {
                return false;
            }

            timer.Dispose();
            this._timers.Remove(key);
            this.OnTimerRemoved?.Invoke(key);
            
            return true;
        }

        /// <summary>
        /// Lấy tất cả các bộ đếm đang hoạt động
        /// </summary>
        /// <returns>Dictionary các bộ đếm</returns>
        public IReadOnlyDictionary<string, ICountdownTimer> GetAllTimers()
        {
            return this._timers;
        }

        /// <summary>
        /// Cập nhật tất cả các bộ đếm (được gọi trong Update loop)
        /// </summary>
        /// <param name="deltaTime">Thời gian delta</param>
        public void UpdateTimers(float deltaTime)
        {
            if (this._disposed)
            {
                return;
            }

            // Cập nhật tất cả các bộ đếm theo thời gian thực
            foreach (var timer in this._timers.Values)
            {
                timer.UpdateRealTime();
            }

            // Xử lý các bộ đếm đã hết hạn
            this.ProcessExpiredTimers();
        }

        /// <summary>
        /// Lưu tất cả các bộ đếm
        /// </summary>
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

        /// <summary>
        /// Tải tất cả các bộ đếm từ dữ liệu đã lưu
        /// </summary>
        public void LoadAllTimers()
        {
            var timerDataList = this.LoadTimerData();
            
            foreach (var data in timerDataList)
            {
                try
                {
                    var timer = new CountdownTimer(data);
                    
                    // Kiểm tra bộ đếm có còn hoạt động không
                    if (timer.IsActive)
                    {
                        this._timers[data.key] = timer;
                        timer.OnComplete += () => this.HandleTimerCompleted(data.key);
                    }
                    else
                    {
                        // Bộ đếm đã hết hạn, kích hoạt sự kiện hoàn thành
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

        /// <summary>
        /// Xóa tất cả các bộ đếm
        /// </summary>
        public void ClearAllTimers()
        {
            foreach (var timer in this._timers.Values)
            {
                timer.Dispose();
            }
            
            this._timers.Clear();
        }

        /// <summary>
        /// Xử lý các bộ đếm đã hết hạn
        /// </summary>
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

            // Xóa các bộ đếm đã hết hạn
            foreach (var key in this._expiredTimerKeys)
            {
                this.RemoveTimer(key);
            }
        }

        /// <summary>
        /// Xử lý khi bộ đếm hoàn thành
        /// </summary>
        /// <param name="key">Khóa của bộ đếm</param>
        private void HandleTimerCompleted(string key)
        {
            this.OnTimerCompleted?.Invoke(key);
            
            // Tự động xóa bộ đếm sau khi hoàn thành
            this.RemoveTimer(key);
        }

        /// <summary>
        /// Lưu dữ liệu bộ đếm (có thể override để lưu vào file, database, etc.)
        /// </summary>
        /// <param name="timerDataList">Danh sách dữ liệu bộ đếm</param>
        protected virtual void SaveTimerData(List<CountdownTimerData> timerDataList)
        {
            // Implement lưu trữ theo nhu cầu (PlayerPrefs, JSON file, etc.)
            var json = JsonUtility.ToJson(new SerializableList<CountdownTimerData>(timerDataList));
            PlayerPrefs.SetString("CountdownTimers", json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Tải dữ liệu bộ đếm (có thể override để tải từ file, database, etc.)
        /// </summary>
        /// <returns>Danh sách dữ liệu bộ đếm</returns>
        protected virtual List<CountdownTimerData> LoadTimerData()
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
            {
                return;
            }

            this.SaveAllTimers();
            this.ClearAllTimers();
            UpdateServiceManager.DeregisterUpdateHandler(this);
            
            this.OnTimerCompleted = null;
            this.OnTimerCreated = null;
            this.OnTimerRemoved = null;
            
            this._disposed = true;
        }

        /// <summary>
        /// Wrapper class để serialize List<CountdownTimerData>
        /// </summary>
        [Serializable]
        private class SerializableList<T>
        {
            public T[] items;
            
            public SerializableList(List<T> list)
            {
                this.items = list.ToArray();
            }
        }

        public void Tick(float deltaTime)
        {
            
        }
    }
}
