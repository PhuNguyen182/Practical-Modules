using System;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Data;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent
{
    /// <summary>
    /// Interface cho bộ đếm thời gian theo thời gian thực với khả năng lưu trữ
    /// </summary>
    public interface ICountdownTimer : IDisposable
    {
        /// <summary>
        /// Khóa định danh của bộ đếm
        /// </summary>
        public string Key { get; }
        
        /// <summary>
        /// Thời gian còn lại tính bằng giây
        /// </summary>
        public float RemainingSeconds { get; }
        
        /// <summary>
        /// Thời gian còn lại dưới dạng TimeSpan
        /// </summary>
        public TimeSpan RemainingTime { get; }
        
        /// <summary>
        /// Tổng thời gian ban đầu (seconds)
        /// </summary>
        public float TotalDuration { get; }
        
        /// <summary>
        /// Kiểm tra bộ đếm có đang hoạt động không
        /// </summary>
        public bool IsActive { get; }
        
        /// <summary>
        /// Kiểm tra bộ đếm đã kết thúc chưa
        /// </summary>
        public bool IsExpired { get; }
        
        /// <summary>
        /// Sự kiện khi bộ đếm được cập nhật
        /// </summary>
        public event Action<float> OnUpdate;
        
        /// <summary>
        /// Sự kiện khi bộ đếm kết thúc
        /// </summary>
        public event Action OnComplete;
        
        /// <summary>
        /// Cập nhật trạng thái bộ đếm theo thời gian thực
        /// </summary>
        public void UpdateRealTime();
        
        /// <summary>
        /// Lấy dữ liệu để lưu trữ
        /// </summary>
        /// <returns>Dữ liệu lưu trữ của bộ đếm</returns>
        public CountdownTimerData GetSaveData();
        
        /// <summary>
        /// Khởi tạo bộ đếm từ dữ liệu đã lưu
        /// </summary>
        /// <param name="data">Dữ liệu đã lưu</param>
        /// <returns>True nếu khởi tạo thành công</returns>
        public bool InitializeFromSaveData(CountdownTimerData data);
        
        /// <summary>
        /// Dừng bộ đếm và kích hoạt sự kiện hoàn thành
        /// </summary>
        public void Complete();
        
        /// <summary>
        /// Làm mới bộ đếm với thời gian mới
        /// </summary>
        /// <param name="newDuration">Thời gian mới (seconds)</param>
        public void Reset(float newDuration);
    }
}
