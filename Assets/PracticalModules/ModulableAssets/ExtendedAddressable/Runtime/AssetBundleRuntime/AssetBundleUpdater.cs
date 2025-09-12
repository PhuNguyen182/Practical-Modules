#if USE_EXTENDED_ADDRESSABLE
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.Interfaces;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.AssetBundleRuntime
{
    public class AssetBundleUpdater : IAssetBundleUpdater
    {
        private readonly IAssetBundleCleaner _assetBundleCleaner;
        private AsyncOperationHandle<List<string>> _catalogUpdates;
        private AsyncOperationHandle<List<IResourceLocator>> _bundleUpdates;

        public AssetBundleUpdater(IAssetBundleCleaner assetBundleCleaner)
            => _assetBundleCleaner = assetBundleCleaner;

        public async UniTask<List<string>> CheckForUpdates()
        {
            _catalogUpdates = Addressables.CheckForCatalogUpdates();
            List<string> catalogUpdateKeys = await _catalogUpdates.Task;
            return catalogUpdateKeys;
        }

        public async UniTask UpdateCatalogs(bool autoCleanBundleCached = true, bool autoRelease = true,
            bool wantToPreserve = false, Action? onUpdateComplete = null, Action? onUpdateFailed = null)
        {
            List<string> catalogUpdateKeys = await CheckForUpdates();
            _bundleUpdates = Addressables.UpdateCatalogs(autoCleanBundleCached, catalogUpdateKeys, autoRelease);
            await _bundleUpdates.Task;

            if (_bundleUpdates.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Updated catalogs successfully.");
                if (!autoCleanBundleCached)
                {
                    List<string>? preserveUpdateKeys = wantToPreserve ? catalogUpdateKeys : null;
                    await _assetBundleCleaner.ClearCachedAssetBundles(preserveUpdateKeys);
                }
                
                onUpdateComplete?.Invoke();
            }
            else
            {
                Debug.Log("Failed to update catalogs.");
                onUpdateFailed?.Invoke();
            }
        }

        public void Dispose()
        {
            if (_catalogUpdates.IsValid())
                _catalogUpdates.Release();

            if (_bundleUpdates.IsValid())
                _bundleUpdates.Release();
        }
    }
}
#endif
