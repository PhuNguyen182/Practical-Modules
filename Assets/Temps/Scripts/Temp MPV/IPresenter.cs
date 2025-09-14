using System;
using System.Collections.Generic;

namespace UISystem.MVP
{
    /// <summary>
    /// Base interface for all Presenters in the MVP pattern
    /// Presenters handle communication between Models and Views
    /// </summary>
    public interface IPresenter : IDisposable
    {
        /// <summary>
        /// The associated model
        /// </summary>
        IModel? Model { get; }
        
        /// <summary>
        /// The associated view
        /// </summary>
        IView? View { get; }
        
        /// <summary>
        /// Whether the presenter is initialized
        /// </summary>
        bool IsInitialized { get; }
        
        /// <summary>
        /// Event fired when presenter is initialized
        /// </summary>
        event Action<IPresenter> OnPresenterInitialized;
        
        /// <summary>
        /// Initialize the presenter with model and view
        /// </summary>
        /// <param name="model">The model to use</param>
        /// <param name="view">The view to use</param>
        void Initialize(IModel model, IView view);
        
        /// <summary>
        /// Show the presenter's view
        /// </summary>
        void Show();
        
        /// <summary>
        /// Hide the presenter's view
        /// </summary>
        void Hide();
        
        /// <summary>
        /// Update the presenter with new data
        /// </summary>
        /// <param name="data">Data to update with</param>
        void UpdatePresenter(object data);
    }

    /// <summary>
    /// Generic presenter interface with type safety
    /// </summary>
    /// <typeparam name="TModel">Type of the model</typeparam>
    /// <typeparam name="TView">Type of the view</typeparam>
    /// <typeparam name="TData">Type of data handled</typeparam>
    public interface IPresenter<TModel, TView, TData> : IPresenter
        where TModel : IModel<TData>
        where TView : IView<TData>
    {
        /// <summary>
        /// The typed model
        /// </summary>
        TModel? TypedModel { get; }
        
        /// <summary>
        /// The typed view
        /// </summary>
        TView? TypedView { get; }
        
        /// <summary>
        /// Initialize with typed model and view
        /// </summary>
        /// <param name="model">Typed model</param>
        /// <param name="view">Typed view</param>
        void Initialize(TModel model, TView view);
    }
}
