namespace Foundations.UIModules.UIView
{
    /// <summary>
    /// Interface for updating View data in MVP pattern
    /// View receives data updates from Presenter and displays them
    /// View should NOT contain business logic, only UI logic
    /// </summary>
    /// <typeparam name="TViewData">UI-specific data format</typeparam>
    public interface IViewUpdater<TViewData>
    {
        public TViewData ViewData { get; }
        
        /// <summary>
        /// Updates View with new data from Presenter
        /// This is the only way View should receive data updates
        /// Data flow: Presenter -> View (one-way)
        /// </summary>
        /// <param name="viewData">UI-specific data from Presenter</param>
        public void UpdateData(TViewData viewData);
    }
}