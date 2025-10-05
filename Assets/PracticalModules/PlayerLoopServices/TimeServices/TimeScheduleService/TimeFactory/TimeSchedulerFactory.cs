using PracticalModules.Patterns.Factory;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeFactory
{
    public class TimeSchedulerFactory : IFactory<TimeSchedulerConfig, TimeScheduler>
    {
        public TimeScheduler Produce(TimeSchedulerConfig config)
        {
            TimeScheduler timeScheduler = new(config.Key, config.Duration);
            return timeScheduler;
        }
    }
}