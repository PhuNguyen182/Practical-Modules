using System;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Data;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Extensions;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent
{
    /// <summary>
    /// Bộ đếm thời gian theo thời gian thực với khả năng lưu trữ và khôi phục
    /// </summary>
    public class CountdownTimer : ICountdownTimer
    {
        private bool _disposed;
        private string _key;
        private long _endTimeUnix;
        private long _startTimeUnix;
        private float _totalDuration;
        private float _cachedRemainingSeconds;
        private bool _isExpired;

        /// <summary>
        /// Khóa định danh của bộ đếm
        /// </summary>
        public string Key => this._key;
        
        /// <summary>
        /// Thời gian còn lại tính bằng giây
        /// </summary>
        public float RemainingSeconds
        {
            get
            {
                if (this._isExpired)
                {
                    return 0f;
                }
                
                this.UpdateRealTime();
                return this._cachedRemainingSeconds;
            }
        }
        
        /// <summary>
        /// Thời gian còn lại dưới dạng TimeSpan
        /// </summary>
        public TimeSpan RemainingTime => TimeSpan.FromSeconds(this.RemainingSeconds);
        
        /// <summary>
        /// Tổng thời gian ban đầu (seconds)
        /// </summary>
        public float TotalDuration => this._totalDuration;
        
        /// <summary>
        /// Kiểm tra bộ đếm có đang hoạt động không
        /// </summary>
        public bool IsActive => !this._isExpired && this.RemainingSeconds > 0f;
        
        /// <summary>
        /// Kiểm tra bộ đếm đã kết thúc chưa
        /// </summary>
        public bool IsExpired => this._isExpired;
        
        /// <summary>
        /// Sự kiện khi bộ đếm được cập nhật
        /// </summary>
        public event Action<float> OnUpdate;
        
        /// <summary>
        /// Sự kiện khi bộ đếm kết thúc
        /// </summary>
        public event Action OnComplete;

        /// <summary>
        /// Constructor để tạo bộ đếm mới
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <param name="durationSeconds">Thời gian đếm ngược (seconds)</param>
        public CountdownTimer(string key, float durationSeconds)
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
        }

        /// <summary>
        /// Constructor để khôi phục từ dữ liệu đã lưu
        /// </summary>
        /// <param name="data">Dữ liệu đã lưu</param>
        public CountdownTimer(CountdownTimerData data)
        {
            if (!this.InitializeFromSaveData(data))
            {
                throw new ArgumentException("Invalid save data", nameof(data));
            }
        }
        
        ~CountdownTimer() => this.Dispose(false);

        /// <summary>
        /// Cập nhật trạng thái bộ đếm theo thời gian thực
        /// </summary>
        public void UpdateRealTime()
        {
            if (this._isExpired)
            {
                return;
            }

            long currentTimeUnix = TimeExtensions.GetCurrentUtcTimestampInSeconds();
            long remainingTimeUnix = this._endTimeUnix - currentTimeUnix;
            
            this._cachedRemainingSeconds = Math.Max(0f, remainingTimeUnix);
            
            if (remainingTimeUnix <= 0)
            {
                this._isExpired = true;
                this._cachedRemainingSeconds = 0f;
                this.OnComplete?.Invoke();
                return;
            }
            
            this.OnUpdate?.Invoke(this._cachedRemainingSeconds);
        }

        /// <summary>
        /// Lấy dữ liệu để lưu trữ
        /// </summary>
        /// <returns>Dữ liệu lưu trữ của bộ đếm</returns>
        public CountdownTimerData GetSaveData()
        {
            return new CountdownTimerData(
                this._key,
                this._endTimeUnix,
                this._startTimeUnix,
                this._totalDuration
            );
        }

        /// <summary>
        /// Khởi tạo bộ đếm từ dữ liệu đã lưu
        /// </summary>
        /// <param name="data">Dữ liệu đã lưu</param>
        /// <returns>True nếu khởi tạo thành công</returns>
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
            
            // Cập nhật trạng thái hiện tại
            this.UpdateRealTime();
            
            return true;
        }

        /// <summary>
        /// Dừng bộ đếm và kích hoạt sự kiện hoàn thành
        /// </summary>
        public void Complete()
        {
            if (this._isExpired)
            {
                return;
            }

            this._isExpired = true;
            this._cachedRemainingSeconds = 0f;
            this.OnComplete?.Invoke();
        }

        /// <summary>
        /// Làm mới bộ đếm với thời gian mới
        /// </summary>
        /// <param name="newDuration">Thời gian mới (seconds)</param>
        public void Reset(float newDuration)
        {
            if (newDuration <= 0f)
            {
                throw new ArgumentException("Duration must be positive", nameof(newDuration));
            }

            this._totalDuration = newDuration;
            this._startTimeUnix = TimeExtensions.GetCurrentUtcTimestampInSeconds();
            this._endTimeUnix = this._startTimeUnix + (long)newDuration;
            this._cachedRemainingSeconds = newDuration;
            this._isExpired = false;
        }
        
        private void Dispose(bool disposing)
        {
            if (this._disposed) 
            {
                return;
            }

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
