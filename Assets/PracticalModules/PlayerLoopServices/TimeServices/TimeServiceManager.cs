using System.Collections.Generic;
using PracticalModules.PlayerLoopServices.Core.Handlers;
using UnityEngine;

namespace PracticalModules.PlayerLoopServices.TimeServices
{
    public static class TimeServiceManager
    {
        private static readonly HashSet<IUpdateHandler> UpdateTimeServices;
        private static readonly HashSet<IFixedUpdateHandler> FixedUpdateTimeServices;

        static TimeServiceManager()
        {
            UpdateTimeServices = new();
            FixedUpdateTimeServices = new();
        }

        public static void UpdateTime()
        {
            foreach (IUpdateHandler timeUpdate in UpdateTimeServices)
                timeUpdate.Tick(Time.deltaTime);
        }
        
        public static void FixedUpdateTime()
        {
            foreach (IFixedUpdateHandler timeUpdate in FixedUpdateTimeServices)
                timeUpdate.Tick();
        }

        public static void RegisterUpdateHandler(IUpdateHandler updateHandler) => UpdateTimeServices.Add(updateHandler);
        
        public static void RegisterFixedUpdateHandler(IFixedUpdateHandler updateHandler) =>
            FixedUpdateTimeServices.Add(updateHandler);

        public static void DeregisterUpdateHandler(IUpdateHandler updateHandler) =>
            UpdateTimeServices.Remove(updateHandler);
        
        public static void DeregisterFixedUpdateHandler(IFixedUpdateHandler updateHandler) =>
            FixedUpdateTimeServices.Remove(updateHandler);

        public static void Clear()
        {
            UpdateTimeServices.Clear();
            FixedUpdateTimeServices.Clear();
        }
    }
}
