using UnityEngine;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Manager
{
    /// <summary>
    /// Service singleton để quản lý TimeScheduleManager
    /// </summary>
    public class TimeScheduleService : MonoBehaviour
    {
        private static TimeScheduleService _instance;
        private static TimeScheduleManager _manager;
        private static bool _isInitialized;

        /// <summary>
        /// Instance singleton của TimeScheduleService
        /// </summary>
        public static TimeScheduleService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TimeScheduleService>();
                    
                    if (_instance == null)
                    {
                        var go = new GameObject("TimeScheduleService");
                        _instance = go.AddComponent<TimeScheduleService>();
                        DontDestroyOnLoad(go);
                    }
                }
                
                return _instance;
            }
        }

        /// <summary>
        /// Manager để quản lý các bộ đếm thời gian
        /// </summary>
        public static TimeScheduleManager Manager
        {
            get
            {
                if (!_isInitialized)
                {
                    Initialize();
                }
                
                return _manager;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _manager?.Dispose();
                _manager = null;
                _isInitialized = false;
                _instance = null;
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                _manager?.SaveAllSchedulers();
            }
            else
            {
                _manager?.LoadAllSchedulers();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                _manager?.SaveAllSchedulers();
            }
            else
            {
                _manager?.LoadAllSchedulers();
            }
        }

        private static void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _manager = new TimeScheduleManager();
            _isInitialized = true;
        }

        #region Public API Methods

        /// <summary>
        /// Bắt đầu một bộ đếm thời gian theo thời gian thực
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <param name="durationSeconds">Thời gian đếm ngược (seconds)</param>
        /// <returns>Bộ đếm thời gian</returns>
        public static ICountdownTimer StartCountdownTimer(string key, float durationSeconds)
        {
            return Manager.StartCountdownTimer(key, durationSeconds);
        }

        /// <summary>
        /// Lấy bộ đếm thời gian theo key
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <returns>Bộ đếm thời gian hoặc null</returns>
        public static ICountdownTimer GetCountdownTimer(string key)
        {
            return Manager.GetCountdownTimer(key);
        }

        /// <summary>
        /// Kiểm tra bộ đếm thời gian có tồn tại không
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <returns>True nếu tồn tại</returns>
        public static bool HasCountdownTimer(string key)
        {
            return Manager.HasCountdownTimer(key);
        }

        /// <summary>
        /// Xóa bộ đếm thời gian
        /// </summary>
        /// <param name="key">Khóa định danh</param>
        /// <returns>True nếu xóa thành công</returns>
        public static bool RemoveCountdownTimer(string key)
        {
            return Manager.RemoveCountdownTimer(key);
        }

        /// <summary>
        /// Lưu tất cả các bộ đếm thời gian
        /// </summary>
        public static void SaveAllTimers()
        {
            Manager.SaveAllSchedulers();
        }

        /// <summary>
        /// Tải tất cả các bộ đếm thời gian
        /// </summary>
        public static void LoadAllTimers()
        {
            Manager.LoadAllSchedulers();
        }

        #endregion
    }
}
