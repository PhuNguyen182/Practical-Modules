using System;
using UnityEngine;

namespace UISystem.MVP
{
    /// <summary>
    /// Base interface for all Views in the MVP pattern
    /// Views handle UI presentation and user interaction
    /// </summary>
    public interface IView : IDisposable
    {
        /// <summary>
        /// The GameObject associated with this view
        /// </summary>
        GameObject GameObject { get; }
        
        /// <summary>
        /// The RectTransform of the view
        /// </summary>
        RectTransform? RectTransform { get; }
        
        /// <summary>
        /// Whether the view is currently active/visible
        /// </summary>
        bool IsActive { get; }
        
        /// <summary>
        /// Event fired when the view is shown
        /// </summary>
        event Action<IView> OnViewShown;
        
        /// <summary>
        /// Event fired when the view is hidden
        /// </summary>
        event Action<IView> OnViewHidden;
        
        /// <summary>
        /// Initialize the view
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Show the view
        /// </summary>
        void Show();
        
        /// <summary>
        /// Hide the view
        /// </summary>
        void Hide();
        
        /// <summary>
        /// Set the view's active state
        /// </summary>
        /// <param name="active">Whether the view should be active</param>
        void SetActive(bool active);
        
        /// <summary>
        /// Update the view with new data
        /// </summary>
        /// <param name="data">Data to display</param>
        void UpdateView(object data);
    }

    /// <summary>
    /// Generic view interface with type safety
    /// </summary>
    /// <typeparam name="T">Type of data the view displays</typeparam>
    public interface IView<T> : IView
    {
        /// <summary>
        /// Event fired when view data is updated
        /// </summary>
        event Action<T> OnViewDataUpdated;
        
        /// <summary>
        /// Update the view with typed data
        /// </summary>
        /// <param name="data">Typed data to display</param>
        void UpdateView(T data);
    }
}
