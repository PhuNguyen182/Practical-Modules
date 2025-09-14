using System.Collections.Generic;
using PracticalModules.PlayerLoopServices.Core.Handlers;

namespace PracticalModules.PlayerLoopServices.TimeServices
{
    public static class FixedUpdateServiceManager
    {
        private static readonly List<IFixedUpdateHandler> FixedUpdateTimeServices = new();
        private static readonly List<IFixedUpdateHandler> PendingFixedUpdateTimeServices = new();
        private static int _currentIndex;
        
        public static void FixedUpdateTime()
        {
            for (_currentIndex = FixedUpdateTimeServices.Count - 1; _currentIndex >= 0; _currentIndex--)
                FixedUpdateTimeServices[_currentIndex].Tick();
            
            FixedUpdateTimeServices.AddRange(PendingFixedUpdateTimeServices);
            PendingFixedUpdateTimeServices.Clear();
        }
        
        public static void RegisterFixedUpdateHandler(IFixedUpdateHandler updateHandler) =>
            PendingFixedUpdateTimeServices.Add(updateHandler);
        
        public static void DeregisterFixedUpdateHandler(IFixedUpdateHandler updateHandler)
        {
            FixedUpdateTimeServices.Remove(updateHandler);
            _currentIndex--;
        }

        public static void Clear()
        {
            FixedUpdateTimeServices.Clear();
            PendingFixedUpdateTimeServices.Clear();
        }
    }
}
