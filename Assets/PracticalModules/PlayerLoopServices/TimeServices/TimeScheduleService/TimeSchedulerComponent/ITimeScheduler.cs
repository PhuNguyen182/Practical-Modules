using System;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent
{
    public interface ITimeScheduler : IDisposable
    {
        public float Duration { get; }
        public string SchedulerKey { get; }
        public Action OnUpdate { get; set; }
        public Action OnComplete { get; set; }
        
        public TimeSpan GetTimeSpan();
        public void Tick(float deltaTime);
        public void Complete();
    }
}
