using System.Collections.Generic;
using PracticalModules.PlayerLoopServices.Core.Handlers;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeFactory;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent;
using PracticalModules.PlayerLoopServices.UpdateServices;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Manager
{
    /// <summary>
    /// Quản lý lịch trình thời gian với khả năng sử dụng cả TimeScheduler và CountdownTimer
    /// </summary>
    public class TimeScheduleManager : IUpdateHandler
    {
        private readonly ICountdownTimerManager _countdownTimerManager;
        
        public TimeScheduleManager()
        {
            this._countdownTimerManager = new CountdownTimerManager();
            UpdateServiceManager.RegisterUpdateHandler(this);
            this.Initialize();
        }
        
        private void Initialize()
        {
            // Đăng ký sự kiện từ CountdownTimerManager
            this._countdownTimerManager.OnTimerCompleted += this.HandleCountdownTimerCompleted;
        }

        /// <summary>
        /// Bắt đầu một bộ đếm thời gian theo thời gian thực
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <param name="durationSeconds">Thời gian đếm ngược (seconds)</param>
        /// <returns>Bộ đếm thời gian</returns>
        public ICountdownTimer StartCountdownTimer(string key, float durationSeconds)
        {
            return this._countdownTimerManager.GetOrCreateTimer(key, durationSeconds);
        }

        /// <summary>
        /// Lấy bộ đếm thời gian theo key
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <returns>Bộ đếm thời gian hoặc null</returns>
        public ICountdownTimer GetCountdownTimer(string key)
        {
            return this._countdownTimerManager.GetTimer(key);
        }

        /// <summary>
        /// Kiểm tra bộ đếm thời gian có tồn tại không
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <returns>True nếu tồn tại</returns>
        public bool HasCountdownTimer(string key)
        {
            return this._countdownTimerManager.HasTimer(key);
        }

        /// <summary>
        /// Xóa bộ đếm thời gian
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <returns>True nếu xóa thành công</returns>
        public bool RemoveCountdownTimer(string key)
        {
            return this._countdownTimerManager.RemoveTimer(key);
        }

        public void Tick(float deltaTime)
        {
            // Cập nhật các CountdownTimer theo thời gian thực
            this._countdownTimerManager.UpdateTimers(deltaTime);
        }

        public void LoadAllSchedulers()
        {
            // LoadCountdownTimers được gọi tự động trong constructor của CountdownTimerManager
            // Có thể thêm logic load TimeScheduler nếu cần
        }

        public void SaveAllSchedulers()
        {
            this._countdownTimerManager.SaveAllTimers();
            // Có thể thêm logic save TimeScheduler nếu cần
        }

        public void Clear()
        {
            this._countdownTimerManager.ClearAllTimers();
        }

        /// <summary>
        /// Xử lý khi bộ đếm thời gian hoàn thành
        /// </summary>
        /// <param name="key">Khóa của bộ đếm</param>
        private void HandleCountdownTimerCompleted(string key)
        {
            // Có thể thêm logic xử lý khi bộ đếm hoàn thành
            UnityEngine.Debug.Log($"Countdown timer '{key}' completed!");
        }

        public void Dispose()
        {
            UpdateServiceManager.DeregisterUpdateHandler(this);
            
            if (this._countdownTimerManager != null)
            {
                this._countdownTimerManager.OnTimerCompleted -= this.HandleCountdownTimerCompleted;
                this._countdownTimerManager.Dispose();
            }
            
            this.Clear();
        }
    }
}
