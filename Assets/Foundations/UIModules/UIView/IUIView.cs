namespace Foundations.UIModules.UIView
{
    public interface IUIView
    {
        
    }

    public interface IUIView<in TModel> : IUIView
    {
        public void BindData(TModel model);
    }
}
