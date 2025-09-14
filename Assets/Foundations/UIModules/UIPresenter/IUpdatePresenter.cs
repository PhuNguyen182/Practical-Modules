namespace Foundations.UIModules.UIPresenter
{
    public interface IUpdatePresenter<TPresenterData>
    {
        public TPresenterData PresenterData { get; }
        public void UpdatePresenter(TPresenterData presenterData);
    }
}