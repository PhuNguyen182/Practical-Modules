using System.Collections.Generic;
using UnityEngine;

public static class ObjectPoolManager
{
    private const int DefaultPoolSize = 30;
    private static readonly Dictionary<int, BaseObjectPool> Pools = new();

    #region Initialization
    
    private static void InitializeObjectPools(GameObject prefab = null, int quantity = DefaultPoolSize)
    {
        if (!prefab) 
            return;
        
        int prefabID = prefab.GetInstanceID();
        if (!Pools.ContainsKey(prefabID))
            Pools[prefabID] = new SimpleObjectPool(prefab, quantity);
    }
    
    private static void InitializeObjectPools<T>(T prefab = null, int quantity = DefaultPoolSize) where T : Component
    {
        if (!prefab) 
            return;
        
        int prefabID = prefab.GetInstanceID();
        if (!Pools.ContainsKey(prefabID))
            Pools[prefabID] = new GenericObjectPool<T>(prefab, quantity);
    }
    
    #endregion

    #region Preloading
    
    public static void PoolPreLoad(GameObject prefab, int quantity, Transform newParent = null)
    {
        InitializeObjectPools(prefab, 1);
        Pools[prefab.GetInstanceID()].Preload(quantity, newParent);
    }
    
    public static void PoolPreLoad<T>(T prefab, int quantity, Transform newParent = null) where T : Component
    {
        InitializeObjectPools(prefab, 1);
        Pools[prefab.GetInstanceID()].Preload(quantity, newParent);
    }
    
    public static GameObject[] Preload(GameObject prefab, int quantity = 1, Transform newParent = null)
    {
        InitializeObjectPools(prefab, quantity);
        GameObject[] gameObjects = new GameObject[quantity];
        
        for (int i = 0; i < quantity; i++)
            gameObjects[i] = Spawn(prefab, Vector3.zero, Quaternion.identity, newParent);
        
        for (int i = 0; i < quantity; i++)
            Despawn(gameObjects[i]);

        return gameObjects;
    }

    #endregion

    #region Spawning
    
    public static GameObject Spawn(GameObject prefab, string tag, Vector3 position, Quaternion rotation)
    {
        InitializeObjectPools(prefab);
        BaseObjectPool pool = Pools[prefab.GetInstanceID()];
        if (pool is not SimpleObjectPool simplePool) 
            return null;
        
        GameObject bullet = simplePool.Spawn(position, rotation, null, true);
        bullet.tag = tag;
        return bullet;

    }

    public static GameObject SpawnInstance(GameObject prefab, Vector3 position, Quaternion rotation,
        Transform parent = null,
        bool worldPositionStay = true)
    {
        InitializeObjectPools(prefab);
        BaseObjectPool pool = Pools[prefab.GetInstanceID()];
        if (pool is not SimpleObjectPool simplePool) 
            return null;
        
        GameObject bullet = simplePool.Spawn(position, rotation, parent, worldPositionStay);
        return bullet;
    }

    public static GameObject Spawn(GameObject prefab) => Spawn(prefab, Vector3.zero, Quaternion.identity, null);

    public static T Spawn<T>(T prefab) where T : Component => Spawn(prefab, Vector3.zero, Quaternion.identity);

    public static T Spawn<T>(T prefab, string tag, Vector3 position = default, Quaternion rotation = default) where T : Component
    {
        InitializeObjectPools(prefab);
        BaseObjectPool pool = Pools[prefab.gameObject.GetInstanceID()];
        if (pool is not GenericObjectPool<T> genericPool) 
            return null;
        
        T bullet = genericPool.Spawn(position, rotation, null, true);
        bullet.tag = tag;
        return bullet;
    }

    public static T SpawnInstance<T>(T prefab, Vector3 position = default, Quaternion rotation = default, 
        Transform parent = null, bool worldPositionStay = true) where T : Component
    {
        InitializeObjectPools(prefab);
        BaseObjectPool pool = Pools[prefab.gameObject.GetInstanceID()];
        if (pool is not GenericObjectPool<T> genericPool) 
            return null;
        
        T bullet = genericPool.Spawn(position, rotation, parent, worldPositionStay);
        return bullet;
    }

    private static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        InitializeObjectPools(prefab);
        BaseObjectPool pool = Pools[prefab.GetInstanceID()];
        if (pool is not SimpleObjectPool simplePool) 
            return null;
        
        return simplePool.Spawn(position, rotation, parent, true);
    }

    private static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        InitializeObjectPools(prefab);
        BaseObjectPool pool = Pools[prefab.gameObject.GetInstanceID()];
        if (pool is not GenericObjectPool<T> genericPool) 
            return null;
        
        return genericPool.Spawn(position, rotation, null, true);
    }

    #endregion

    #region Despawning
    
    public static void Despawn(GameObject gameObject, Transform parent, bool worldPositionStay = true)
    {
        BaseObjectPool objectPool = null;
        foreach (var pool in Pools.Values)
        {
            if (!pool.ContainsInstance(gameObject.GetInstanceID())) 
                continue;
            
            objectPool = pool;
            break;
        }

        if (objectPool == null)
        {
            Debug.Log($"Object {gameObject.name} wasn't spawned from a pool. Destroying it instead.");
            Object.Destroy(gameObject);
        }
        else
        {
            gameObject.transform.SetParent(parent, worldPositionStay);
            if (objectPool is SimpleObjectPool simplePool)
                simplePool.Despawn(gameObject);
        }
    }

    public static void Despawn(GameObject gameObject)
    {
        BaseObjectPool objectPool = null;
        foreach (var pool in Pools.Values)
        {
            if (!pool.ContainsInstance(gameObject.GetInstanceID())) 
                continue;
            
            objectPool = pool;
            break;
        }

        switch (objectPool)
        {
            case null:
                Debug.Log($"Object '{gameObject.name}' wasn't spawned from a pool. Destroying it instead.");
                Object.Destroy(gameObject);
                break;
            case SimpleObjectPool simplePool:
                simplePool.Despawn(gameObject);
                break;
        }
    }
    
    public static void Despawn<T>(T instance, Transform parent, bool worldPositionStay = true) where T : Component
    {
        BaseObjectPool objectPool = null;
        foreach (var pool in Pools.Values)
        {
            if (!pool.ContainsInstance(instance.GetInstanceID())) 
                continue;
            
            objectPool = pool;
            break;
        }

        if (objectPool == null)
        {
            Debug.Log($"Object {instance.name} wasn't spawned from a pool. Destroying it instead.");
            Object.Destroy(instance.gameObject);
        }
        else
        {
            instance.transform.SetParent(parent, worldPositionStay);
            if (objectPool is GenericObjectPool<T> simplePool)
                simplePool.Despawn(instance);
        }
    }

    public static void Despawn<T>(T instance) where T : Component
    {
        BaseObjectPool objectPool = null;
        foreach (var pool in Pools.Values)
        {
            if (!pool.ContainsInstance(instance.GetInstanceID())) 
                continue;
            
            objectPool = pool;
            break;
        }

        switch (objectPool)
        {
            case null:
                Debug.Log($"Object '{instance.name}' wasn't spawned from a pool. Destroying it instead.");
                Object.Destroy(instance.gameObject);
                break;
            case GenericObjectPool<T> simplePool:
                simplePool.Despawn(instance);
                break;
        }
    }

    #endregion

    public static int GetStackCount(GameObject prefab)
    {
        if (!prefab)
            return 0;

        return Pools.ContainsKey(prefab.GetInstanceID()) ? Pools[prefab.GetInstanceID()].StackCount : 0;
    }
    
    public static void ClearPool()
    {
        if (Pools is not { Count: > 0 }) 
            return;
        
        foreach (var pool in Pools.Values)
            pool.Dispose();

        Pools.Clear();
    }
}