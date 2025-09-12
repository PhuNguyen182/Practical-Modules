#if USE_EXTENDED_ADDRESSABLE
using System;
using UnityEngine;
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
            AssetBundleLoader = new AssetBundleLoader();
            AssetBundleResourceLocator = new AssetBundleResourceLocator();
            AssetBundleCleaner = new AssetBundleCleaner(AssetBundleResourceLocator);
            AssetBundleDownloader = new AssetBundleDownloader(AssetBundleResourceLocator, AssetBundleCleaner);
            AssetBundleUpdater = new AssetBundleUpdater(AssetBundleCleaner);
        }

        public async UniTask<bool> Initialize(Action? onInitializationComplete = null,
            Action? onInitializationFailed = null)
        {
            try
            {
                _initializeHandle = Addressables.InitializeAsync();
                await _initializeHandle.Task;

                if (_initializeHandle.Status == AsyncOperationStatus.Succeeded)
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

        public void Dispose()
        {
            AssetBundleLoader.Dispose();
            AssetBundleUpdater.Dispose();

            if (_initializeHandle.IsValid())
                _initializeHandle.Release();
        }
    }
}
#endif
