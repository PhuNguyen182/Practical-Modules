namespace Foundations.UIModules.UIPresenter
{
    /// <summary>
    /// Interface for converting business data to view-specific data in MVP pattern
    /// Presenter acts as the converter between business logic (Model) and UI (View)
    /// </summary>
    /// <typeparam name="TPresenterData">Business data managed by Presenter</typeparam>
    /// <typeparam name="TViewData">UI-specific data format for View</typeparam>
    public interface IPresenterViewConverter<in TPresenterData, out TViewData>
    {
        /// <summary>
        /// Converts business data to view-specific data format
        /// This ensures View only receives data in the format it understands
        /// </summary>
        /// <param name="presenterData">Business data from Presenter</param>
        /// <returns>View-specific data format</returns>
        public TViewData ConvertToView(TPresenterData presenterData);
    }
}