using System;
using System.Collections.Generic;
using Foundations.SaveSystem;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Data;
using UnityEngine;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Persistence
{
    /// <summary>
    /// Serializer cho List CountdownTimerData sử dụng JSON
    /// </summary>
    public class TimerDataSerializer : IDataSerializer<List<CountdownTimerData>>
    {
        public string FileExtension => ".json";

        public string Serialize(List<CountdownTimerData> data)
        {
            if (data.Count == 0)
            {
                return string.Empty;
            }

            try
            {
                var wrapper = new TimerDataWrapper { timers = data.ToArray() };
                return JsonUtility.ToJson(wrapper, true);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to serialize timer data: {ex.Message}");
                return string.Empty;
            }
        }

        public List<CountdownTimerData> Deserialize(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                return new List<CountdownTimerData>();
            }

            try
            {
                var wrapper = JsonUtility.FromJson<TimerDataWrapper>(serializedData);
                
                if (wrapper?.timers == null)
                {
                    return new List<CountdownTimerData>();
                }

                var resultList = new List<CountdownTimerData>(wrapper.timers.Length);
                
                for (int i = 0; i < wrapper.timers.Length; i++)
                {
                    resultList.Add(wrapper.timers[i]);
                }
                
                return resultList;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to deserialize timer data: {ex.Message}");
                return new List<CountdownTimerData>();
            }
        }

        [Serializable]
        private class TimerDataWrapper
        {
            public CountdownTimerData[] timers = null!;
        }
    }
}

