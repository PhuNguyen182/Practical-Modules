using System;
using PracticalModules.PlayerLoopServices.Core.Handlers;
using UnityEngine;

namespace PracticalModules.PlayerLoopServices.TimeServices.Timers
{
    public abstract class BaseTimer : ITimer, IUpdateHandler, IDisposable
    {
        private bool _disposed;
        private float _initialTime;

        public abstract bool IsFinished { get; }
        
        public bool IsRunning { get; private set; }
        public float CurrentTime { get; protected set; }
        
        public float Progress => Mathf.Clamp(CurrentTime / _initialTime, 0f, 1f);

        public Action OnTimerStart { get; set; }
        public Action OnTimerStop { get; set; }
        public Action<float> OnTimerUpdate { get; set; }

        protected BaseTimer(float time) => _initialTime = time;
        
        ~BaseTimer() => Dispose(false);
        
        public void Start()
        {
            CurrentTime = _initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                TimeServiceManager.RegisterUpdateHandler(this);
                OnTimerStart?.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                TimeServiceManager.DeregisterUpdateHandler(this);
                OnTimerStop?.Invoke();
            }
        }

        public abstract void Tick(float deltaTime);
        
        public void Resume() => IsRunning = true;
        
        public void Pause() => IsRunning = false;

        public virtual void Reset() => CurrentTime = _initialTime;

        public virtual void Reset(float newTime)
        {
            _initialTime = newTime;
            Reset();
        }

        // Call Dispose to ensure deregistration of the timer from the TimerManager
        // when the consumer is done with the timer or being destroyed
        public void Dispose()
        {
            Dispose(true);
            ReleaseAllCallbacks();
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) 
                return;

            if (disposing)
                TimeServiceManager.DeregisterUpdateHandler(this);

            _disposed = true;
        }

        protected virtual void ReleaseAllCallbacks()
        {
            OnTimerStart = null;
            OnTimerUpdate = null;
            OnTimerStop = null;
        }
    }
}
