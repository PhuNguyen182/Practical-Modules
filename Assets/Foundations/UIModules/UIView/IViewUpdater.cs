namespace Foundations.UIModules.UIView
{
    public interface IViewUpdater<TViewData>
    {
        public TViewData ViewData { get; }
        public void UpdateData(TViewData viewData);
    }
}