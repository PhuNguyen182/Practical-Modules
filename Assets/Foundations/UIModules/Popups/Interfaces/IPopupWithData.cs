using System;

namespace Foundations.UIModules.Popups.Interfaces
{
    /// <summary>
    /// Interface for popups that can receive data
    /// </summary>
    /// <typeparam name="TData">Type of data to receive</typeparam>
    public interface IPopup<TData> : IPopup
    {
        /// <summary>
        /// Current data in the popup
        /// </summary>
        public TData Data { get; }
        
        /// <summary>
        /// Updates the popup with new data
        /// </summary>
        /// <param name="data">New data</param>
        public void UpdateData(TData data);
        
        /// <summary>
        /// Event triggered when data is updated
        /// </summary>
        public event Action<IPopup<TData>, TData> OnDataUpdated;
    }
}
