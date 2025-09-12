using UnityEngine;

namespace Foundations.UIModules.UIView
{
    public abstract class UIView<TModel> : MonoBehaviour, IUIView<TModel>
    {
        public abstract void BindData(TModel model);
    }
}
