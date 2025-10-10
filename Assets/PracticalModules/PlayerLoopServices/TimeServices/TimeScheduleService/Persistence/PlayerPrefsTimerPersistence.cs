using System;
using System.Collections.Generic;
using Foundations.SaveSystem;
using Foundations.SaveSystem.CustomDataSaverService;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Data;
using UnityEngine;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Persistence
{
    /// <summary>
    /// Implementation của ITimerPersistence sử dụng PlayerPrefDataSaveService
    /// Lưu timer data vào PlayerPrefs dưới dạng JSON
    /// </summary>
    public class PlayerPrefsTimerPersistence : ITimerPersistence
    {
        private const string SaveKey = "CountdownTimers";
        
        private readonly IDataSaveService<List<CountdownTimerData>> _dataSaveService;

        public PlayerPrefsTimerPersistence()
        {
            var serializer = new TimerDataSerializer();
            this._dataSaveService = new PlayerPrefDataSaveService<List<CountdownTimerData>>(serializer);
        }

        public bool SaveTimers(List<CountdownTimerData> timerDataList)
        {
            if (timerDataList == null || timerDataList.Count == 0)
            {
                return this.ClearTimers();
            }
            
            try
            {
                this._dataSaveService.SaveData(SaveKey, timerDataList);
                Debug.Log($"[PlayerPrefsTimerPersistence] Saved {timerDataList.Count} timers to PlayerPrefs");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlayerPrefsTimerPersistence] Failed to save timer data: {ex.Message}");
                return false;
            }
        }

        public List<CountdownTimerData> LoadTimers()
        {
            try
            {
                var loadTask = this._dataSaveService.LoadData(SaveKey);
                var timerDataList = loadTask.GetAwaiter().GetResult();
                
                if (timerDataList != null && timerDataList.Count > 0)
                {
                    Debug.Log($"[PlayerPrefsTimerPersistence] Loaded {timerDataList.Count} timers from PlayerPrefs");
                }
                
                return timerDataList ?? new List<CountdownTimerData>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlayerPrefsTimerPersistence] Failed to load timer data: {ex.Message}");
                return new List<CountdownTimerData>();
            }
        }

        public bool ClearTimers()
        {
            try
            {
                this._dataSaveService.DeleteData(SaveKey);
                Debug.Log("[PlayerPrefsTimerPersistence] Cleared all timer data from PlayerPrefs");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlayerPrefsTimerPersistence] Failed to clear timer data: {ex.Message}");
                return false;
            }
        }

        public bool HasSavedTimers()
        {
            try
            {
                var timerDataList = this.LoadTimers();
                return timerDataList != null && timerDataList.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}

