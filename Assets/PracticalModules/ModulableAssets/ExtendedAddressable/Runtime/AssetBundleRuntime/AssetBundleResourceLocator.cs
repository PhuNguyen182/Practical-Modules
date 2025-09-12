#if USE_EXTENDED_ADDRESSABLE
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.Interfaces;
using UnityEngine.ResourceManagement.ResourceLocations;
using Cysharp.Threading.Tasks;

namespace PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.AssetBundleRuntime
{
    public class AssetBundleResourceLocator : IAssetBundleResourceLocator
    {
        public async UniTask<bool> IsKeyValid(string key)
        {
            bool exists = await KeyOrLabelExistsAsync(key);
            return exists;
        }

        public async UniTask<bool> IsKeyValid(List<string> keys)
        {
            bool exists = await KeyOrLabelExistsAsync(keys);
            return exists;
        }

        public async UniTask<long> GetDownloadSize(string key)
        {
            bool keyExists = await IsKeyValid(key);
            if (!keyExists)
            {
                Debug.LogError($"Addressable key '{key}' does not exist.");
                return -1;
            }
            
            long downloadSize = await GetDownloadSizeAsync(key);
            return downloadSize;
        }

        public async UniTask<long> GetDownloadSize(List<string> keys)
        {
            bool keyExists = await IsKeyValid(keys);
            if (!keyExists)
            {
                Debug.LogError($"Addressable key '{keys}' does not exist.");
                return -1;
            }
            
            long downloadSize = await GetDownloadSizeAsync(keys);
            return downloadSize;
        }

        private static async UniTask<long> GetDownloadSizeAsync(object key)
        {
            AsyncOperationHandle<IList<IResourceLocation>> locationsHandle =
                Addressables.LoadResourceLocationsAsync(key);
            IList<IResourceLocation> locations = await locationsHandle.Task;

            if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to get resource locations for {key}");
                locationsHandle.Release();
                return -1;
            }
            
            AsyncOperationHandle<long> sizeHandle = Addressables.GetDownloadSizeAsync(locations);
            long sizeToDownload = await sizeHandle.Task;
            locationsHandle.Release();
            sizeHandle.Release();
            return sizeToDownload;
        }
        
        private static async UniTask<bool> KeyOrLabelExistsAsync(object keyOrLabel)
        {
            AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync(keyOrLabel);
            await handle.Task.AsUniTask();
            bool exists = handle is { Status: AsyncOperationStatus.Succeeded, Result: { Count: > 0 } };

            Debug.LogError(!exists
                ? $"Addressable key or label {keyOrLabel} not exist."
                : $"Addressable key or label {keyOrLabel} exist.");

            handle.Release();
            return exists;
        }
    }
}
#endif
