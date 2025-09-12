#if USE_EXTENDED_ADDRESSABLE
using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.Interfaces;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.AssetBundleRuntime
{
    public class AssetBundleDownloader : IAssetBundleDownloader
    {
        private readonly IAssetBundleCleaner _bundleCleaner;
        private readonly IAssetBundleResourceLocator _bundleResourceLocator;

        public AssetBundleDownloader(IAssetBundleResourceLocator bundleResourceLocator,
            IAssetBundleCleaner bundleCleaner)
        {
            _bundleCleaner = bundleCleaner;
            _bundleResourceLocator = bundleResourceLocator;
        }

        public async UniTask DownloadAsset(string key, bool autoRelease = true, bool clearCacheAfterDownload = true
            , Action<float>? onProgression = null, Action? onDownloadComplete = null, Action? onDownloadFailed = null)
        {
            long downloadSize = await _bundleResourceLocator.GetDownloadSize(key);
            Debug.Log($"Download size: {downloadSize}");

            if (downloadSize <= 0)
            {
                Debug.LogError($"There is nothing to download for key: {key}.");
                return;
            }

            await DownloadBundleAsync(key, autoRelease, clearCacheAfterDownload, onProgression, onDownloadComplete,
                onDownloadFailed);
        }

        public async UniTask DownloadAsset(List<string> keys, bool autoRelease = true, bool clearCacheAfterDownload = true,
            Action<float>? onProgression = null, Action? onDownloadComplete = null, Action? onDownloadFailed = null)
        {
            long downloadSize = await _bundleResourceLocator.GetDownloadSize(keys);
            Debug.Log($"Download size: {downloadSize}");

            if (downloadSize <= 0)
            {
                Debug.LogError($"There is nothing to download for key: {keys}.");
                return;
            }

            await DownloadBundleAsync(keys, autoRelease, clearCacheAfterDownload, onProgression, onDownloadComplete,
                onDownloadFailed);
        }

        private async UniTask DownloadBundleAsync(object key, bool autoRelease = true, bool clearCacheAfterDownload = true,
            Action<float>? onProgression = null, Action? onDownloadComplete = null, Action? onDownloadFailed = null)
        {
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(key, autoRelease);
            while (!downloadHandle.IsDone)
            {
                onProgression?.Invoke(downloadHandle.PercentComplete);
                await UniTask.NextFrame();
            }

            if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Downloaded remote bundle(s) for {key} successfully.");
                onDownloadComplete?.Invoke();

                // Clear cache after download
                if (clearCacheAfterDownload)
                {
                    await _bundleCleaner.ClearCachedAssetBundles();
                    /*
                    if (key is string clearKey)
                        await _bundleCleaner.ClearDependencyCacheBundles(clearKey);
                    else if (key is List<string> clearKeys)
                        await _bundleCleaner.ClearDependencyCacheBundles(clearKeys);
                    */
                }
            }
            else
            {
                Debug.LogError($"Failed to download remote bundle(s) for {key}.");
                onDownloadFailed?.Invoke();
            }

            if (!autoRelease)
                downloadHandle.Release();
        }
    }
}
#endif
