using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
#if NGO_SUPPORT
using Unity.Netcode;
#endif

namespace PracticalUtilities.SceneUtils
{
    public static class SceneLoader
    {
        public static async UniTask LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            await SceneManager.LoadSceneAsync(sceneName, loadMode);
        }

        public static async UniTask LoadScene(string sceneName, IProgress<float> progress, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            AsyncOperation sceneOperator = SceneManager.LoadSceneAsync(sceneName, loadMode);
            await sceneOperator.ToUniTask(progress);
        }

        public static async UniTask LoadSceneWithCondition(string sceneName, LoadSceneCondition condition, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            AsyncOperation sceneOperator = SceneManager.LoadSceneAsync(sceneName, loadMode);
            sceneOperator.allowSceneActivation = false;

            while (!sceneOperator.isDone)
            {
                if(sceneOperator.progress >= 0.9f)
                {
                    if (condition.AllowSceneLoad)
                        sceneOperator.allowSceneActivation = true;
                }

                await UniTask.NextFrame();
            }
        }

#if NGO_SUPPORT
        public static void LoadNetworkScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, loadMode);
        }
#endif

#if UNITASK_ADDRESSABLE_SUPPORT
        public static async UniTask LoadSceneViaAddressable(string key, LoadSceneMode loadMode = LoadSceneMode.Single
            , bool activateOnLoad = true, int priority = 100, CancellationToken cancellationToken = default)
        {
            await AddressablesUtils.LoadSceneViaAddressable(key, loadMode, activateOnLoad, priority, cancellationToken);
        }
#endif
    }
}
