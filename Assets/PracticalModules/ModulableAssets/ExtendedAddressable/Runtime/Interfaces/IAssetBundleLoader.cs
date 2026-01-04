#if USE_EXTENDED_ADDRESSABLE
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace PracticalModules.ModulableAssets.ExtendedAddressable.Runtime.Interfaces
{
    public interface IAssetBundleLoader : IDisposable
    {
        public UniTask LoadScene(string key, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true);

        public UniTask UnloadScene(UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects,
            bool autoReleaseHandle = true);

        public UniTask<GameObject> LoadAsset(string key);
        public UniTask<T> LoadAsset<T>(string key);
        public UniTask<T> LoadComponentAsset<T>(string key) where T : Component;
        public void UnloadAsset<T>(T asset) where T : Component;
    }
}
#endif