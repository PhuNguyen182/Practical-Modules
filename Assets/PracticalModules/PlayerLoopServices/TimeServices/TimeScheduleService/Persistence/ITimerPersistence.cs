using System.Collections.Generic;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Data;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Persistence
{
    /// <summary>
    /// Interface cho persistence layer của timer system
    /// Hỗ trợ multiple storage backends (PlayerPrefs, File, Database, etc.)
    /// </summary>
    public interface ITimerPersistence
    {
        /// <summary>
        /// Lưu danh sách timer data
        /// </summary>
        /// <param name="timerDataList">Danh sách timer cần lưu</param>
        /// <returns>True nếu lưu thành công</returns>
        public bool SaveTimers(List<CountdownTimerData> timerDataList);
        
        /// <summary>
        /// Tải danh sách timer data
        /// </summary>
        /// <returns>Danh sách timer đã lưu</returns>
        public List<CountdownTimerData> LoadTimers();
        
        /// <summary>
        /// Xóa tất cả timer data đã lưu
        /// </summary>
        /// <returns>True nếu xóa thành công</returns>
        public bool ClearTimers();
        
        /// <summary>
        /// Kiểm tra có timer data đã lưu không
        /// </summary>
        /// <returns>True nếu có data</returns>
        public bool HasSavedTimers();
    }
}

