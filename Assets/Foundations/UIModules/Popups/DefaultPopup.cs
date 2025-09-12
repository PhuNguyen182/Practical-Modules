using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using PracticalModules.Patterns.SimpleObjectPooling;
using Foundations.UIModules.UIComponents;
using Cysharp.Threading.Tasks;

namespace Foundations.UIModules.Popups
{
    public abstract class DefaultPopup<TModel> : MonoBehaviour, IPopup<TModel>
    {
        [SerializeField] private UIButton closeButton;
        [SerializeField] private CanvasGroup targetView;

        private AsyncOperationHandle<GameObject> _opHandle;

        private Action _onPopupOpenAction;
        private Action _onPopupCloseAction;
        protected TModel PopupModel { get; private set; }

        private void Awake()
        {
            closeButton.AddOnClickListener(Close);
        }

        private void Start()
        {
            _onPopupOpenAction?.Invoke();
            OnPopupOpened();
        }

        public virtual void BindData(TModel modelData)
        {
            PopupModel = modelData;
        }
        
        public void SetOnOpenAction(Action onOpenAction) => _onPopupOpenAction = onOpenAction;
        
        public void SetOnCloseAction(Action onCloseAction) => _onPopupCloseAction = onCloseAction;

        public void Close()
        {
            _onPopupCloseAction?.Invoke();
            OnPopupClosed();
            Release();
        }

        protected virtual void OnPopupOpened()
        {
            
        }

        protected virtual void OnPopupClosed()
        {

        }

        public async UniTask PreloadFromAddress(string address, TModel modelData = default)
        {
            DefaultPopup<TModel> instance = await CreateFromAddress(address, modelData);

            if (instance != null)
                ObjectPoolManager.Despawn(instance.gameObject);
        }

        public async UniTask<DefaultPopup<TModel>> CreateFromAddress(string address, TModel modelData = default)
        {
            DefaultPopup<TModel> instance = null;
            _opHandle = Addressables.LoadAssetAsync<GameObject>(address);
            await _opHandle;

            if (_opHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (!ObjectPoolManager.Spawn(_opHandle.Result).TryGetComponent(out instance)) return instance;
                instance.BindData(modelData);
                instance.gameObject.SetActive(true);
            }

            else Release();

            return instance;
        }

        private void Release()
        {
            if (_opHandle.IsValid())
                Addressables.Release(_opHandle);
        }
    }
}
