using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PracticalModules.Patterns.SimpleObjectPooling
{
    /// <summary>
    /// The Pool class represents the pool for a particular prefab.
    /// </summary>
    public class SimpleObjectPool : IDisposable
    {
        // We append an id to the name of anything we instantiate.
        // This is purely cosmetic.
        private int _nextId;

        // The structure containing our inactive objects.
        // Why a Stack and not a List? Because we'll never need to
        // pluck an object from the start or middle of the array.
        // We'll always just grab the last one, which eliminates
        // any need to shuffle the objects around in memory.
        private readonly Queue<GameObject> _inactive;

        //A Hashset which contains all GetInstanceIDs from the instantiated GameObjects 
        //so we know which GameObject is a member of this pool.
        public readonly HashSet<int> MemberIDs;

        // The prefab that we are pooling
        private readonly GameObject _prefab;

        public int StackCount => _inactive.Count;

        // Constructor
        public SimpleObjectPool(GameObject prefab, int initialQuantity)
        {
            _nextId = 1;
            _prefab = prefab;
            // If Stack uses a linked list internally, then this
            // whole initialQty thing is a placebo that we could
            // strip out for more minimal code. But it can't *hurt*.
            _inactive = new(initialQuantity);
            MemberIDs = new();
        }

        public void Preload(int initialQuantity, Transform parent = null)
        {
            for (int i = 0; i < initialQuantity; i++)
            {
                // Instantiate a whole new object.
                GameObject gameObject = Object.Instantiate(_prefab, parent);
                gameObject.name = $"{_prefab.name} ({_nextId++})";

                // AddUpdateBehaviour the unique GameObject ID to our MemberHashset so we know this GO belongs to us.
                MemberIDs.Add(gameObject.GetInstanceID());
                gameObject.SetActive(false);
                _inactive.Enqueue(gameObject);
            }
        }

        // Spawn an object from our pool
        public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent, bool worldPositionStay)
        {
            while (true)
            {
                GameObject gameObject;
                if (_inactive.Count == 0)
                {
                    // We don't have an object in our pool, so we
                    // instantiate a whole new object.
                    gameObject = Object.Instantiate(_prefab, position, rotation, parent);
                    gameObject.name = $"{_prefab.name} ({_nextId++})";

                    // AddUpdateBehaviour the unique GameObject ID to our MemberHashset so we know this GO belongs to us.
                    MemberIDs.Add(gameObject.GetInstanceID());
                }
                else
                {
                    // Grab the last object in the inactive array
                    gameObject = _inactive.Dequeue();

                    if (!gameObject)
                    {
                        // The inactive object we expected to find no longer exists.
                        // The most likely causes are:
                        //   - Someone calling DestroyEntity() on our object
                        //   - A scene change (which will destroy all our objects).
                        //     NOTE: This could be prevented with a DontDestroyOnLoad
                        //	   if you really don't want this.
                        // No worries -- we'll just try the next one in our sequence.

                        continue;
                    }
                }

                gameObject.transform.SetPositionAndRotation(position, rotation);
                if (parent)
                    gameObject.transform.SetParent(parent, worldPositionStay);

                gameObject.SetActive(true);
                return gameObject;
            }
        }

        public T Spawn<T>(Vector3 position, Quaternion rotation, Transform parent, bool worldPositionStay)
            where T : Component => Spawn(position, rotation, parent, worldPositionStay).TryGetComponent<T>(out var component)
            ? component
            : null;

        // Return an object to the inactive pool.
        public void Despawn(GameObject gameObject)
        {
            if (!gameObject.activeSelf)
                return;

            gameObject.SetActive(false);
            _inactive.Enqueue(gameObject);
        }

        public void Dispose()
        {
            _inactive.Clear();
            MemberIDs.Clear();
        }
    }
}