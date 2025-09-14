namespace Foundations.UIModules.UIPresenter
{
    public interface IPresenterViewConverter<in TPresenterData, out TViewData>
    {
        public TViewData ConvertToView(TPresenterData presenterData);
    }
}