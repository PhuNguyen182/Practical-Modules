using System.Collections.Generic;
using PracticalModules.PlayerLoopServices.Core.Handlers;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeFactory;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent;
using PracticalModules.PlayerLoopServices.UpdateServices;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Manager
{
    public class TimeScheduleManager : ITimeScheduleManager, IUpdateHandler
    {
        private readonly TimeSchedulerFactory _timeSchedulerFactory;
        private readonly Dictionary<string, ITimeScheduler> _timeSchedulers;
        
        public TimeScheduleManager()
        {
            _timeSchedulerFactory = new();
            _timeSchedulers = new();
            UpdateServiceManager.RegisterUpdateHandler(this);
            Initialize();
        }
        
        private void Initialize()
        {
            
        }

        public void StartScheduler(string key, ITimeScheduler timeScheduler)
        {
            
        }

        public void RegisterTimeScheduler(string key, ITimeScheduler timeScheduler)
        {
            _timeSchedulers.Add(key, timeScheduler);
        }

        public void UnregisterTimeScheduler(string key)
        {
            _timeSchedulers.Remove(key);
        }

        public void Tick(float deltaTime)
        {
            foreach (var timeScheduler in _timeSchedulers)
            {
                timeScheduler.Value.Tick(deltaTime);
            }
        }

        public void LoadAllSchedulers()
        {
            
        }

        public void SaveAllSchedulers()
        {
            
        }

        public void Clear()
        {
            _timeSchedulers?.Clear();
        }

        public void Dispose()
        {
            UpdateServiceManager.DeregisterUpdateHandler(this);
            Clear();
        }
    }
}
