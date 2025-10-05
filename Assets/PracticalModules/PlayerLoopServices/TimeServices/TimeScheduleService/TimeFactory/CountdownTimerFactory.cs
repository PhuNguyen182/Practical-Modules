using PracticalModules.Patterns.Factory;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Data;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeFactory
{
    /// <summary>
    /// Factory để tạo CountdownTimer
    /// </summary>
    public class CountdownTimerFactory : IFactory<TimeSchedulerConfig, CountdownTimer>
    {
        /// <summary>
        /// Tạo CountdownTimer từ config
        /// </summary>
        /// <param name="config">Cấu hình bộ đếm</param>
        /// <returns>CountdownTimer mới</returns>
        public CountdownTimer Produce(TimeSchedulerConfig config)
        {
            return new CountdownTimer(config.Key, config.Duration);
        }

        /// <summary>
        /// Tạo CountdownTimer từ dữ liệu đã lưu
        /// </summary>
        /// <param name="data">Dữ liệu đã lưu</param>
        /// <returns>CountdownTimer được khôi phục</returns>
        public CountdownTimer ProduceFromSaveData(CountdownTimerData data)
        {
            return new CountdownTimer(data);
        }
    }
}
