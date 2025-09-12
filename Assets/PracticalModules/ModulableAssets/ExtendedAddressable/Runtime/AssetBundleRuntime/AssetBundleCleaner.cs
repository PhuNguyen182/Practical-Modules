#if USE_EXTENDED_ADDRESSABLE
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.Interfaces;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.AssetBundleRuntime
{
    public class AssetBundleCleaner : IAssetBundleCleaner
    {
        private readonly IAssetBundleResourceLocator _resourceLocator;
        
        public AssetBundleCleaner(IAssetBundleResourceLocator resourceLocator)
            => _resourceLocator = resourceLocator;

        public bool ClearAll()
        {
            bool isClearAll = Caching.ClearCache();
            Debug.Log(isClearAll ? "Cleared all cached asset bundles." : "Failed to clear all cached asset bundles.");
            return isClearAll;
        }
        
        public async UniTask ClearDependencyCacheBundles(string key, bool autoRelease = true)
        {
            if (!await _resourceLocator.IsKeyValid(key))
            {
                Debug.LogError($"Addressable key or label '{key}' does not exist.");
                return;
            }

            AsyncOperationHandle<bool> clearDependencyCacheAsync =
                Addressables.ClearDependencyCacheAsync(key, autoRelease);
            bool result = await clearDependencyCacheAsync.Task;
            Debug.Log(result ? $"Cleared cache for {key}." : $"Failed to clear cache for {key}.");

            if (!autoRelease)
                clearDependencyCacheAsync.Release();
        }

        public async UniTask ClearDependencyCacheBundles(IEnumerable<string> keys, bool autoRelease = true)
        {
            List<string> keysList = keys.ToList();
            if (!await _resourceLocator.IsKeyValid(keysList))
            {
                Debug.LogError($"Addressable key or label '{keysList}' does not exist.");
                return;
            }

            AsyncOperationHandle<bool> clearDependencyCacheAsync =
                Addressables.ClearDependencyCacheAsync(keysList, autoRelease);
            bool result = await clearDependencyCacheAsync.Task;
            Debug.Log(result ? $"Cleared cache for {keysList}." : $"Failed to clear cache for {keysList}.");

            if (!autoRelease)
                clearDependencyCacheAsync.Release();
        }

        public async UniTask ClearCachedAssetBundles(IEnumerable<string>? catalogIds = null)
        {
            AsyncOperationHandle<bool> clearDependencyCacheAsync = catalogIds != null
                ? Addressables.CleanBundleCache(catalogIds)
                : Addressables.CleanBundleCache();
            
            bool result = await clearDependencyCacheAsync.Task;
            Debug.Log(result ? "Cleared all cached asset bundles." : "Failed to clear all cached asset bundles.");
            clearDependencyCacheAsync.Release();
        }
    }
}
#endif
