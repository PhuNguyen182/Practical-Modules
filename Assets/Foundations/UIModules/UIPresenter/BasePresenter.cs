using System;
using Foundations.UIModules.UIView;
using UnityEngine;

namespace Foundations.UIModules.UIPresenter
{
    public abstract class BaseUIPresenter<T> : MonoBehaviour, IUIPresenter<T>
    {
        public T Data { get; private set; }
        public Action<T> OnDataUpdated { get; set; }
        public abstract IUIView<T> View { get; }
        
        public void UpdatePresenter(T data)
        {
            Data = data;
            OnDataUpdated?.Invoke(data);
        }
    }
}
