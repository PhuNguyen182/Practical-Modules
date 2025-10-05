using System.Collections.Generic;
using UnityEngine;

public static class ObjectPoolManager
{
    // You can avoid resizing of the Stack's internal data by
    // setting this to a number equal to or greater to what you
    // expect most of your pool sizes to be.
    // Note, you can also use Preload() to setSpawnPoints the initial size
    // of a pool -- this can be handy if only some of your pools
    // are going to be exceptionally large (for example, your bullets.)
    private const int DefaultPoolSize = 30;

    // All of our pools
    private static readonly Dictionary<int, SimpleObjectPool> Pools = new();

    #region Initialization

    /// <summary>
    /// Initialize our dictionary.
    /// </summary>
    private static void InitializeObjectPools(GameObject prefab = null, int quantity = DefaultPoolSize)
    {
        if (prefab)
        {
            int prefabID = prefab.GetInstanceID();
            if (!Pools.ContainsKey(prefabID))
                Pools[prefabID] = new(prefab, quantity);
        }
    }

    public static void PoolPreLoad(GameObject prefab, int quantity, Transform newParent = null)
    {
        InitializeObjectPools(prefab, 1);
        Pools[prefab.GetInstanceID()].Preload(quantity, newParent);
    }

    /// <summary>
    /// If you want to preload a few copies of an object at the start
    /// of a scene, you can use this. Really not needed unless you're
    /// going to go from zero instances to 100+ very quickly.
    /// It could technically be optimized more, but in practicing the
    /// Spawn/Despawn sequence is going to be pretty darn quick and
    /// this avoids code duplication.
    /// </summary>
    public static GameObject[] Preload(GameObject prefab, int quantity = 1, Transform newParent = null)
    {
        InitializeObjectPools(prefab, quantity);

        // Make an array to grab the objects we're about to pre-spawn.
        GameObject[] gameObjects = new GameObject[quantity];
        for (int i = 0; i < quantity; i++)
            gameObjects[i] = Spawn(prefab, Vector3.zero, Quaternion.identity, newParent);

        // Now despawn them all.
        for (int i = 0; i < quantity; i++)
            Despawn(gameObjects[i]);

        return gameObjects;
    }

    #endregion

    #region Spawning

    /// <summary>
    /// Spawns a copy of the specified prefab (instantiating one if required).
    /// NOTE: Remember that Awake() or Start() will only run on the very first
    /// spawn and that member variables won't get reset.  OnEnable will run
    /// after spawning -- but remember that toggling CanCheck will also
    /// call that function.
    /// </summary>
    public static GameObject Spawn(GameObject prefab, string tag, Vector3 position, Quaternion rotation)
    {
        InitializeObjectPools(prefab);
        GameObject bullet = Pools[prefab.GetInstanceID()].Spawn(position, rotation, null, true);
        bullet.tag = tag;
        return bullet;
    }

    public static GameObject SpawnInstance(GameObject prefab, Vector3 position, Quaternion rotation,
        Transform parent = null,
        bool worldPositionStay = true)
    {
        InitializeObjectPools(prefab);
        GameObject bullet = Pools[prefab.GetInstanceID()].Spawn(position, rotation, parent, worldPositionStay);
        return bullet;
    }

    public static GameObject Spawn(GameObject prefab) => Spawn(prefab, Vector3.zero, Quaternion.identity, null);

    public static T Spawn<T>(T prefab) where T : Component => Spawn(prefab, Vector3.zero, Quaternion.identity);

    public static T Spawn<T>(T prefab, string tag, Vector3 position = default, Quaternion rotation = default) where T : Component
    {
        InitializeObjectPools(prefab.gameObject);
        T bullet = Pools[prefab.gameObject.GetInstanceID()].Spawn<T>(position, rotation, null, true);
        bullet.tag = tag;
        return bullet;
    }

    public static T SpawnInstance<T>(T prefab, Vector3 position = default, Quaternion rotation = default, 
        Transform parent = null, bool worldPositionStay = true) where T : Component
    {
        InitializeObjectPools(prefab.gameObject);
        T bullet = Pools[prefab.gameObject.GetInstanceID()].Spawn<T>(position, rotation, parent, worldPositionStay);
        return bullet;
    }

    private static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        InitializeObjectPools(prefab);
        return Pools[prefab.GetInstanceID()].Spawn(position, rotation, parent, true);
    }

    private static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        InitializeObjectPools(prefab.gameObject);
        return Pools[prefab.gameObject.GetInstanceID()].Spawn<T>(position, rotation, null, true);
    }

    #endregion

    #region Despawning

    /// <summary>
    /// Despawn the specified GameObject back into its pool.
    /// </summary>
    /// 
    public static void Despawn(GameObject gameObject, Transform parent, bool worldPositionStay = true)
    {
        SimpleObjectPool p = null;
        foreach (var pool in Pools.Values)
        {
            if (pool.MemberIDs.Contains(gameObject.GetInstanceID()))
            {
                p = pool;
                break;
            }
        }

        if (p == null)
        {
            Debug.Log($"Object {gameObject.name} wasn't spawned from a pool. Destroying it instead.");
            Object.Destroy(gameObject);
        }
        else
        {
            gameObject.transform.SetParent(parent, worldPositionStay);
            p.Despawn(gameObject);
        }
    }

    public static void Despawn(GameObject gameObject)
    {
        SimpleObjectPool p = null;
        foreach (var pool in Pools.Values)
        {
            if (pool.MemberIDs.Contains(gameObject.GetInstanceID()))
            {
                p = pool;
                break;
            }
        }

        if (p == null)
        {
            Debug.Log($"Object '{gameObject.name}' wasn't spawned from a pool. Destroying it instead.");
            Object.Destroy(gameObject);
        }
        else
        {
            p.Despawn(gameObject);
        }
    }

    #endregion

    public static int GetStackCount(GameObject prefab)
    {
        if (prefab == null)
            return 0;

        return Pools.ContainsKey(prefab.GetInstanceID()) ? Pools[prefab.GetInstanceID()].StackCount : 0;
    }

    // This function should be called if player completes a section of gameplay, such as exiting from gameplay to the main home scene
    public static void ClearPool()
    {
        if (Pools != null)
        {
            foreach (var pool in Pools.Values)
                pool.Dispose();

            Pools.Clear();
        }
    }
}