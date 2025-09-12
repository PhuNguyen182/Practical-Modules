using System;

namespace PracticalModules.PlayerLoopServices.TimeServices.Timers
{
    /// <summary>
    /// Timer that ticks at a specific frequency. (N times per second)
    /// </summary>
    public class FrequencyTimer : BaseTimer
    {
        private float _timeThreshold;
        
        public int TicksPerSecond { get; private set; }
        public override bool IsFinished => !IsRunning;

        public Action OnTick = delegate { };

        public FrequencyTimer(int ticksPerSecond) : base(0) => CalculateTimeThreshold(ticksPerSecond);

        public override void Tick(float deltaTime)
        {
            if (IsRunning && CurrentTime >= _timeThreshold)
            {
                CurrentTime -= _timeThreshold;
                OnTimerUpdate?.Invoke(CurrentTime);
                OnTick.Invoke();
            }

            if (IsRunning && CurrentTime < _timeThreshold)
                CurrentTime += deltaTime;
        }
        
        public override void Reset() => CurrentTime = 0;
        
        public void Reset(int newTicksPerSecond)
        {
            CalculateTimeThreshold(newTicksPerSecond);
            Reset();
        }

        private void CalculateTimeThreshold(int ticksPerSecond)
        {
            TicksPerSecond = ticksPerSecond;
            _timeThreshold = 1f / TicksPerSecond;
        }

        protected override void ReleaseAllCallbacks()
        {
            base.ReleaseAllCallbacks();
            OnTick = null;
        }
    }
}
