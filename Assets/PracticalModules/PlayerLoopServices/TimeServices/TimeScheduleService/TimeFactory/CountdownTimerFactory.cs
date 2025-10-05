using PracticalModules.Patterns.Factory;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent;
using PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.Data;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeFactory
{
    public class CountdownTimerFactory : BaseFactory<TimeSchedulerConfig, CountdownTimer>
    {
        public override CountdownTimer Produce(TimeSchedulerConfig config)
        {
            CountdownTimer countdownTimer = new(config.Key, config.Duration);
            return countdownTimer;
        }

        public CountdownTimer ProduceFromSaveData(CountdownTimerData data)
        {
            CountdownTimer countdownTimer = new(data);
            return countdownTimer;
        }
    }
}
