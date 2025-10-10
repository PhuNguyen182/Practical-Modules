namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Persistence
{
    /// <summary>
    /// Loại persistence backend cho timer system
    /// </summary>
    public enum TimerPersistenceType
    {
        /// <summary>
        /// Lưu vào file JSON (Recommended - ưu tiên)
        /// </summary>
        File,
        
        /// <summary>
        /// Lưu vào PlayerPrefs
        /// </summary>
        PlayerPrefs
    }
}

