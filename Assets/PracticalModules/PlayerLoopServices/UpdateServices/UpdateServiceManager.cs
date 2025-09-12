using System.Collections.Generic;
using PracticalModules.PlayerLoopServices.Core.Handlers;
using UnityEngine;

namespace PracticalModules.PlayerLoopServices.UpdateServices
{
    public static class UpdateServiceManager
    {
        private static readonly HashSet<IUpdateHandler> UpdateTimeServices;

        static UpdateServiceManager() => UpdateTimeServices = new();
        
        public static void UpdateTime()
        {
            foreach (IUpdateHandler timeUpdate in UpdateTimeServices)
                timeUpdate.Tick(Time.deltaTime);
        }

        public static void RegisterUpdateHandler(IUpdateHandler updateHandler) => UpdateTimeServices.Add(updateHandler);
        
        public static void DeregisterUpdateHandler(IUpdateHandler updateHandler) =>
            UpdateTimeServices.Remove(updateHandler);
        
        public static void Clear() => UpdateTimeServices.Clear();
    }
}
