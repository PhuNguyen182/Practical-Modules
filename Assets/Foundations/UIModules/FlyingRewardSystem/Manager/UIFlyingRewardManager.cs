using System;
using System.Collections.Generic;
using Foundations.UIModules.FlyingRewardSystem.Components;

namespace Foundations.UIModules.FlyingRewardSystem.Manager
{
    public class UIFlyingRewardManager : IUIFlyingRewardManager, IDisposable
    {
        private bool _disposed;
        private readonly Dictionary<string, IUITargetObject> _targetObjects;
        
        public UIFlyingRewardManager()
        {
            _disposed = false;
            _targetObjects = new Dictionary<string, IUITargetObject>();
        }

        public IUITargetObject GetRewardTargetObject(string key)
        {
            return this._targetObjects.GetValueOrDefault(key);
        }

        public bool RegisterRewardTargetObject(string key, IUITargetObject targetObject)
        {
            return this._targetObjects.TryAdd(key, targetObject);
        }

        public bool UnregisterRewardTargetObject(string key)
        {
            return this._targetObjects.Remove(key);
        }

        private void ReleaseUnmanagedResources()
        {
            this._targetObjects.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
                return;
            
            if (disposing)
            {
                ReleaseUnmanagedResources();
            }
            
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UIFlyingRewardManager()
        {
            Dispose(false);
        }
    }
}