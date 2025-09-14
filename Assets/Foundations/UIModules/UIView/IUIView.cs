namespace Foundations.UIModules.UIView
{
    public interface IUIView : IShowable, IHideable
    {
        public void SetActive(bool active);
        public void SetInteractable(bool interactable);
    }
}
