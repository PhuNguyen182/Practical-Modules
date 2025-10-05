using PracticalModules.Patterns.Factory;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Data;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeFactory
{
    public class CountdownTimerFactory : IFactory<TimeSchedulerConfig, CountdownTimer>
    {
        public CountdownTimer Produce(TimeSchedulerConfig config)
        {
            return new CountdownTimer(config.Key, config.Duration);
        }

        public CountdownTimer ProduceFromSaveData(CountdownTimerData data)
        {
            return new CountdownTimer(data);
        }
    }
}
