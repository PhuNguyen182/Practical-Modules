#if USE_EXTENDED_ADDRESSABLE
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.Interfaces;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.AssetBundleRuntime
{
    public class AssetBundleLoader : IAssetBundleLoader
    {
        private AsyncOperationHandle<GameObject> _loadRequest;
        private AsyncOperationHandle<SceneInstance> _sceneLoadRequest;

        public async UniTask LoadScene(string key, LoadSceneMode mode = LoadSceneMode.Single,
            bool activateOnLoad = true)
        {
            _sceneLoadRequest = Addressables.LoadSceneAsync(key, mode, activateOnLoad);
            await _sceneLoadRequest;
        }

        public async UniTask UnloadScene(UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects,
            bool autoReleaseHandle = true)
        {
            if (_sceneLoadRequest.IsValid())
            {
                var sceneOperation = Addressables.UnloadSceneAsync(_sceneLoadRequest, options, autoReleaseHandle);
                await sceneOperation;
            }
        }

        public async UniTask<GameObject> LoadAsset(string key)
        {
            _loadRequest = Addressables.LoadAssetAsync<GameObject>(key);
            GameObject asset = await _loadRequest.Task;

            if (_loadRequest.Status == AsyncOperationStatus.Succeeded)
                return asset;

            _loadRequest.Release();
            return null;
        }

        public async UniTask<T> LoadAsset<T>(string key) where T : Component
        {
            _loadRequest = Addressables.LoadAssetAsync<GameObject>(key);
            GameObject gameObject = await _loadRequest.Task;

            if (_loadRequest.Status == AsyncOperationStatus.Succeeded)
            {
                gameObject.TryGetComponent(out T asset);
                return asset;
            }

            _loadRequest.Release();
            return null;
        }

        public void UnloadAsset<T>(T asset) where T : Component
        {
            if (asset)
                Addressables.Release(asset);
        }

        public void Dispose()
        {
            if (_loadRequest.IsValid())
                _loadRequest.Release();

            if (_sceneLoadRequest.IsValid())
                _sceneLoadRequest.Release();
        }
    }
}
#endif
