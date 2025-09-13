using System;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.MVP
{
    /// <summary>
    /// Base implementation of IModel with common functionality
    /// </summary>
    public abstract class BaseModel : IModel
    {
        public Action<IModel>? OnModelChanged { get; set; }

        public bool IsInitialized { get; protected set; }
        public string ModelId { get; protected set; } = string.Empty;

        protected bool _disposed = false;

        public BaseModel()
        {
            ModelId = Guid.NewGuid().ToString();
        }

        public virtual void Initialize(object? data = null)
        {
            if (IsInitialized)
            {
                Debug.LogWarning($"Model {ModelId} is already initialized");
                return;
            }

            OnInitialize(data);
            IsInitialized = true;
            OnModelChanged?.Invoke(this);
        }

        /// <summary>
        /// Override this method to implement custom initialization logic
        /// </summary>
        /// <param name="data">Initialization data</param>
        protected abstract void OnInitialize(object? data);

        /// <summary>
        /// Notify that the model has changed
        /// </summary>
        protected virtual void NotifyModelChanged()
        {
            OnModelChanged?.Invoke(this);
        }

        public virtual void Dispose()
        {
            if (_disposed) return;

            OnDispose();
            OnModelChanged = null;
            _disposed = true;
        }

        /// <summary>
        /// Override this method to implement custom disposal logic
        /// </summary>
        protected virtual void OnDispose()
        {
        }
    }

    /// <summary>
    /// Generic base model implementation
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public abstract class BaseModel<T> : BaseModel, IModel<T>
    {
        public Action<T>? OnDataChanged { get; set; }

        private T _data = default!;

        public T Data
        {
            get => _data;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_data, value))
                {
                    _data = value;
                    OnDataChanged?.Invoke(_data);
                    NotifyModelChanged();
                }
            }
        }

        protected override void OnInitialize(object? data)
        {
            if (data is T typedData)
            {
                _data = typedData;
            }
            else
            {
                _data = default!;
            }

            OnInitializeData(_data);
        }

        /// <summary>
        /// Override this method to implement custom data initialization
        /// </summary>
        /// <param name="data">Initial data</param>
        protected abstract void OnInitializeData(T data);

        public virtual void UpdateData(T newData)
        {
            Data = newData;
        }

        protected override void OnDispose()
        {
            OnDataChanged = null;
            base.OnDispose();
        }
    }
}
