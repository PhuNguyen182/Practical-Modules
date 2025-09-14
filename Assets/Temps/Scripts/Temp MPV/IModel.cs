using System;
using System.Collections.Generic;

namespace UISystem.MVP
{
    /// <summary>
    /// Base interface for all Models in the MVP pattern
    /// Models handle data and business logic
    /// </summary>
    public interface IModel : IDisposable
    {
        /// <summary>
        /// Event fired when the model data changes
        /// </summary>
        Action<IModel> OnModelChanged { get; set; }
        
        /// <summary>
        /// Initialize the model with data
        /// </summary>
        /// <param name="data">Initial data for the model</param>
        void Initialize(object? data = null);
        
        /// <summary>
        /// Check if the model is initialized
        /// </summary>
        bool IsInitialized { get; }
        
        /// <summary>
        /// Get the model's unique identifier
        /// </summary>
        string ModelId { get; }
    }

    /// <summary>
    /// Generic model interface with type safety
    /// </summary>
    /// <typeparam name="T">Type of data the model handles</typeparam>
    public interface IModel<T> : IModel
    {
        /// <summary>
        /// Current data of the model
        /// </summary>
        T Data { get; set; }
        
        /// <summary>
        /// Event fired when specific data changes
        /// </summary>
        Action<T> OnDataChanged { get; set; }
        
        /// <summary>
        /// Update the model data
        /// </summary>
        /// <param name="newData">New data to set</param>
        void UpdateData(T newData);
    }
}
