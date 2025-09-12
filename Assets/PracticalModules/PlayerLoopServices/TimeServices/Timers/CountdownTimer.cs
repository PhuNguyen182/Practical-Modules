namespace PracticalModules.PlayerLoopServices.TimeServices.Timers
{
    /// <summary>
    /// Timer that counts down from a specific value to zero.
    /// </summary>
    public class CountdownTimer : BaseTimer
    {
        public override bool IsFinished => CurrentTime <= 0;

        public CountdownTimer(float time) : base(time)
        {
        }

        public override void Tick(float deltaTime)
        {
            if (IsRunning && CurrentTime > 0)
            {
                CurrentTime -= deltaTime;
                OnTimerUpdate?.Invoke(CurrentTime);
            }

            if (IsRunning && CurrentTime <= 0)
                Stop();
        }
    }
}
