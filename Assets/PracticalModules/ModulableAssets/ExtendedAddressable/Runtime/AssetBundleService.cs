#if USE_EXTENDED_ADDRESSABLE
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.Interfaces;
using PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.AssetBundleRuntime;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

namespace PracticalModules.ModulableAssets.ExtendedAddressable.Runtime
{
    public class AssetBundleService : IAssetBundleService
    {
        private AsyncOperationHandle<IResourceLocator> _initializeHandle;

        public IAssetBundleLoader AssetBundleLoader { get; }
        public IAssetBundleDownloader AssetBundleDownloader { get; }
        public IAssetBundleCleaner AssetBundleCleaner { get; }
        public IAssetBundleUpdater AssetBundleUpdater { get; }
        public IAssetBundleResourceLocator AssetBundleResourceLocator { get; }

        public AssetBundleService()
        {
            this.AssetBundleLoader = new AssetBundleLoader();
            this.AssetBundleResourceLocator = new AssetBundleResourceLocator();
            this.AssetBundleCleaner = new AssetBundleCleaner(this.AssetBundleResourceLocator);
            this.AssetBundleDownloader =
                new AssetBundleDownloader(this.AssetBundleResourceLocator, this.AssetBundleCleaner);
            this.AssetBundleUpdater = new AssetBundleUpdater(this.AssetBundleCleaner);
        }

        public async UniTask<bool> Initialize(Action onInitializationComplete = null,
            Action onInitializationFailed = null)
        {
            try
            {
                this._initializeHandle = Addressables.InitializeAsync();
                await this._initializeHandle;

                if (this._initializeHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Addressable initialized successfully.");
                    onInitializationComplete?.Invoke();
                    return true;
                }

                Debug.LogError("Addressable initialization failed.");
                onInitializationFailed?.Invoke();
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during Addressable initialization: {ex.Message}");
                onInitializationFailed?.Invoke();
                return false;
            }
        }

        public async UniTask<bool> InitializeAsync(UniTask onInitializationComplete,
            UniTask onInitializationFailed)
        {
            try
            {
                this._initializeHandle = Addressables.InitializeAsync();
                await this._initializeHandle;

                if (this._initializeHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Addressable initialized successfully.");
                    await onInitializationComplete;
                    return true;
                }

                Debug.LogError("Addressable initialization failed.");
                await onInitializationFailed;
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during Addressable initialization: {ex.Message}");
                await onInitializationFailed;
                return false;
            }
        }

        public void Dispose()
        {
            this.AssetBundleLoader.Dispose();
            this.AssetBundleUpdater.Dispose();

            if (this._initializeHandle.IsValid())
                Addressables.Release(this._initializeHandle);
        }
    }
}
#endif
