using System;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Manager
{
    public interface ITimeScheduleManager : IDisposable
    {
        public void StartScheduler(string key, ITimeScheduler timeScheduler);
        public void RegisterTimeScheduler(string key, ITimeScheduler timeScheduler);
        public void UnregisterTimeScheduler(string key);
        public void Tick(float deltaTime);
        public void LoadAllSchedulers();
        public void SaveAllSchedulers();
        public void Clear();
    }
}
