using System;

namespace PracticalModules.PlayerLoopServices.TimeServices.TimeScheduleService.TimeSchedulerComponent
{
    public class TimeScheduler : ITimeScheduler
    {
        private bool _disposed;
        private float _duration;
        
        public float Duration => this._duration;
        public string SchedulerKey { get; }

        public Action OnUpdate { get; set; }
        public Action OnComplete { get; set; }

        public TimeScheduler(string schedulerKey, float duration)
        {
            this.SchedulerKey = schedulerKey;
            this._duration = duration;
        }
        
        ~TimeScheduler() => Dispose(false);

        public TimeSpan GetTimeSpan() => _duration > 0 ? TimeSpan.FromSeconds(_duration) : TimeSpan.Zero;

        public void Tick(float deltaTime)
        {
            _duration -= deltaTime;
            OnUpdate?.Invoke();
            
            if (_duration <= 0)
                Complete();
        }

        public void Complete()
        {
            OnComplete?.Invoke();
        }
        
        private void Dispose(bool disposing)
        {
            if (_disposed) 
                return;

            if (disposing)
            {
                OnUpdate = null;
                OnComplete = null;
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
