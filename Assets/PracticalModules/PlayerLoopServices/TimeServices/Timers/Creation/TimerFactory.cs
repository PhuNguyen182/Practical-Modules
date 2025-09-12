using PracticalModules.Patterns.Factory;

namespace PracticalModules.PlayerLoopServices.TimeServices.Timers.Creation
{
    public class TimerFactory : BaseFactory<TimerConfig, BaseTimer>
    {
        public override BaseTimer Produce(TimerConfig arg)
        {
            return arg.Type switch
            {
                TimerType.Countdown => new CountdownTimer(arg.Time),
                TimerType.Frequency => new FrequencyTimer((int)arg.Time),
                TimerType.Stopwatch => new StopwatchTimer(arg.Time),
                _ => null,
            };
        }
    }
}
