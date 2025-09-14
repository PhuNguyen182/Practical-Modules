using UnityEngine;
using UnityEngine.LowLevel;
using PracticalModules.PlayerLoopServices.TimeServices;
using PracticalModules.PlayerLoopServices.UpdateServices;
using FixedUpdate = UnityEngine.PlayerLoop.FixedUpdate;
using Update = UnityEngine.PlayerLoop.Update;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PracticalModules.PlayerLoopServices.Core
{
    public static class PlayerLoopBoostrapper
    {
        private static PlayerLoopSystem _playerLoopSystem;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize()
        {
            PlayerLoopSystem currentLoopSystem = PlayerLoop.GetCurrentPlayerLoop();

            if (!InsertUpdateSystem<Update>(ref currentLoopSystem, 0))
            {
                Debug.LogError("Failed to insert Update system to PlayerLoop.");
                return;
            }

            if (!InsertFixedUpdateSystem<FixedUpdate>(ref currentLoopSystem, 1))
            {
                Debug.LogError("Failed to insert FixedUpdate system to PlayerLoop.");
                return;
            }

            PlayerLoop.SetPlayerLoop(currentLoopSystem);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;

            static void OnPlayModeState(PlayModeStateChange state)
            {
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                    RemoveTimeSystem<Update>(ref currentPlayerLoop);
                    RemoveTimeSystem<FixedUpdate>(ref currentPlayerLoop);
                    PlayerLoop.SetPlayerLoop(currentPlayerLoop);
                    FixedUpdateServiceManager.Clear();
                }
            }
#endif
        }

        private static bool InsertUpdateSystem<T>(ref PlayerLoopSystem playerLoopSystem, int index)
        {
            _playerLoopSystem = new PlayerLoopSystem
            {
                type = typeof(T),
                updateDelegate = Update,
                subSystemList = null
            };

            return PlayerLoopUtils.InsertSystem<T>(ref playerLoopSystem, in _playerLoopSystem, index);
        }
        
        private static bool InsertFixedUpdateSystem<T>(ref PlayerLoopSystem playerLoopSystem, int index)
        {
            _playerLoopSystem = new PlayerLoopSystem
            {
                type = typeof(T),
                updateDelegate = FixedUpdate,
                subSystemList = null
            };

            return PlayerLoopUtils.InsertSystem<T>(ref playerLoopSystem, in _playerLoopSystem, index);
        }

        private static void RemoveTimeSystem<T>(ref PlayerLoopSystem playerLoopSystem) =>
            PlayerLoopUtils.RemoveSystem<T>(ref playerLoopSystem, in _playerLoopSystem);

        private static void Update()
        {
            UpdateServiceManager.UpdateTime();
        }
        
        private static void FixedUpdate()
        {
            FixedUpdateServiceManager.FixedUpdateTime();
        }
    }
}
