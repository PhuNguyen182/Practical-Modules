#if USE_EXTENDED_ADDRESSABLE
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.AddressableAssets;

namespace PracticalModules.ModulableAssets.ExtendedAddressable.Runtime
{
    public static class AddressableManager
    {
        /// <summary>
        /// Initializes Addressable and CCD.
        /// </summary>
        public static async UniTask<bool> InitializeAsync()
        {
            try
            {
                AsyncOperationHandle<IResourceLocator> handle = Addressables.InitializeAsync();
                await handle.Task.AsUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Addressable initialized successfully.");
                    return true;
                }

                Debug.LogError("Addressable initialization failed.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during Addressable initialization: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a key or label exists in Addressable.
        /// </summary>
        private static async UniTask<bool> KeyOrLabelExistsAsync(string keyOrLabel)
        {
            AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync(keyOrLabel);
            await handle.Task.AsUniTask();
            bool exists = handle is { Status: AsyncOperationStatus.Succeeded, Result: { Count: > 0 } };

            if (!exists)
                Debug.LogWarning($"Addressable key or label '{keyOrLabel}' does not exist.");

            return exists;
        }

        /// <summary>
        /// Downloads remote bundles from CCD for a given key or label.
        /// </summary>
        public static async UniTask DownloadRemoteBundleAsync(string keyOrLabel, bool autoRelease = true,
            Action<float> onProgress = null)
        {
            if (!await KeyOrLabelExistsAsync(keyOrLabel))
            {
                Debug.LogError($"Addressable key or label '{keyOrLabel}' does not exist.");
                return;
            }

            try
            {
                AsyncOperationHandle<IList<IResourceLocation>> locationsHandle =
                    Addressables.LoadResourceLocationsAsync(keyOrLabel);
                await locationsHandle.Task.AsUniTask();

                if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"Failed to get resource locations for {keyOrLabel}");
                    return;
                }

                IList<IResourceLocation> locations = locationsHandle.Result;
                AsyncOperationHandle<long> sizeHandle = Addressables.GetDownloadSizeAsync(locations);
                await sizeHandle.Task.AsUniTask();

                if (sizeHandle.Result > 0)
                {
                    AsyncOperationHandle downloadHandle =
                        Addressables.DownloadDependenciesAsync(locations, autoRelease);
                    while (!downloadHandle.IsDone)
                    {
                        onProgress?.Invoke(downloadHandle.PercentComplete);
                        await UniTask.NextFrame();
                    }

                    if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                        Debug.Log($"Downloaded remote bundle(s) for {keyOrLabel} successfully.");
                    else
                        Debug.LogError($"Failed to download remote bundle(s) for {keyOrLabel}.");
                }

                else
                {
                    Debug.Log($"No download needed for {keyOrLabel} (already cached).");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during bundle download: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads an Addressable asset asynchronously.
        /// </summary>
        public static async UniTask<T> LoadAssetAsync<T>(string key) where T : UnityEngine.Object
        {
            if (!await KeyOrLabelExistsAsync(key))
            {
                Debug.LogError($"Addressable key or label '{key}' does not exist.");
                return null;
            }

            try
            {
                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
                await handle.Task.AsUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Loaded asset {key} successfully.");
                    return handle.Result;
                }

                Debug.LogError($"Failed to load asset {key}.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during asset load: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Unloads an Addressable asset.
        /// </summary>
        public static void UnloadAsset<T>(T asset) where T : UnityEngine.Object
        {
            if (asset != null)
            {
                Addressables.Release(asset);
                Debug.Log($"Unloaded asset {asset.name}.");
            }
        }

        /// <summary>
        /// Unloads dependencies for a key or label.
        /// </summary>
        public static void UnloadDependencies(string keyOrLabel)
        {
            AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync(keyOrLabel);
            handle.WaitForCompletion();

            if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result?.Count == 0)
            {
                Debug.LogWarning($"Addressable key or label '{keyOrLabel}' does not exist. Skipping unload.");
                return;
            }

            Addressables.Release(keyOrLabel);
            Debug.Log($"Released dependencies for {keyOrLabel}.");
        }

        /// <summary>
        /// Deletes all cached bundles on the device.
        /// </summary>
        public static async UniTask ClearCacheAsync()
        {
            try
            {
                bool result = await Addressables.CleanBundleCache().Task.AsUniTask();
                Debug.Log(result ? "Cleared all Addressable cache." : "Failed to clear Addressable cache.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during cache clear: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes cached bundles for a specific key or label.
        /// </summary>
        public static async UniTask ClearCacheForKeyAsync(string keyOrLabel, bool autoRelease = true)
        {
            if (!await KeyOrLabelExistsAsync(keyOrLabel))
            {
                Debug.LogError($"Addressable key or label '{keyOrLabel}' does not exist.");
                return;
            }

            try
            {
                bool result = await Addressables.ClearDependencyCacheAsync(keyOrLabel, autoRelease).Task.AsUniTask();
                Debug.Log(result ? $"Cleared cache for {keyOrLabel}." : $"Failed to clear cache for {keyOrLabel}.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during cache clear for {keyOrLabel}: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks the download size for a key or label.
        /// </summary>
        public static async UniTask<long> GetDownloadSizeAsync(string keyOrLabel)
        {
            if (!await KeyOrLabelExistsAsync(keyOrLabel))
            {
                Debug.LogError($"Addressable key or label '{keyOrLabel}' does not exist.");
                return -1;
            }

            try
            {
                AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(keyOrLabel);
                await handle.Task.AsUniTask();
                return handle.Result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during download size check: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Checks for available Addressable catalog updates.
        /// </summary>
        public static async UniTask<List<string>> CheckForCatalogUpdatesAsync()
        {
            try
            {
                AsyncOperationHandle<List<string>> handle = Addressables.CheckForCatalogUpdates();
                await handle.Task.AsUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Catalogs with available updates: {string.Join(", ", handle.Result)}");
                    return handle.Result;
                }

                Debug.LogError("Failed to check for catalog updates.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during catalog update check: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Updates catalogs and optionally removes unused bundles from cache.
        /// </summary>
        /// <param name="autoCleanBundleCache">If true, removes any nonreferenced bundles in the cache after updating catalogs.</param>
        /// <param name="catalogs">Catalog IDs to update. If null, all catalogs with updates will be updated.</param>
        public static async UniTask<List<IResourceLocator>> UpdateCatalogsAsync(bool autoCleanBundleCache = true,
            IEnumerable<string> catalogs = null)
        {
            try
            {
                AsyncOperationHandle<List<IResourceLocator>> handle =
                    Addressables.UpdateCatalogs(autoCleanBundleCache, catalogs);
                await handle.Task.AsUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Updated catalogs. Count: {handle.Result?.Count ?? 0}");
                    return handle.Result;
                }

                Debug.LogError("Failed to update catalogs.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during catalog update: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Removes unused or unusable old cached bundles from the device.
        /// </summary>
        /// <param name="catalogIds">Catalog IDs whose bundle cache entries to preserve. If null, all currently loaded catalogs will be preserved.</param>
        public static async UniTask<bool> CleanUnusedBundleCacheAsync(IEnumerable<string> catalogIds = null)
        {
            try
            {
                AsyncOperationHandle<bool> handle = Addressables.CleanBundleCache(catalogIds);
                await handle.Task.AsUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Cleaned unused bundle cache successfully.");
                    return handle.Result;
                }

                Debug.LogError("Failed to clean unused bundle cache.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during cleaning unused bundle cache: {ex.Message}");
                return false;
            }
        }
    }
}
#endif
