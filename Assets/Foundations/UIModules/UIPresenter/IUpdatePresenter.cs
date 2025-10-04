namespace Foundations.UIModules.UIPresenter
{
    /// <summary>
    /// Interface for updating Presenter data following MVP pattern
    /// Data flows from external sources (Model/Service) to Presenter
    /// Presenter then converts and sends data to View
    /// </summary>
    public interface IUpdatePresenter<TPresenterData>
    {
        public TPresenterData PresenterData { get; }
        
        /// <summary>
        /// Updates Presenter with new data from Model/Service
        /// This is the entry point for data flow: External -> Presenter -> View
        /// </summary>
        /// <param name="presenterData">New data from business logic layer</param>
        public void UpdatePresenter(TPresenterData presenterData);
    }
}