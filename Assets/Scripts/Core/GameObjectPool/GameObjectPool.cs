using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.PoolableGameObjects
{
    public class GameObjectPool
    {
        private readonly List<PoolableGameObject> poolableGameObjects = new List<PoolableGameObject>();

        public GameObject Get()
        {
            for (int i = 0; i < poolableGameObjects.Count; i++)
            {
                if (poolableGameObjects[i].CanRetrieve())
                {
                    poolableGameObjects[i].OnRetrievedFromPool();
                    return poolableGameObjects[i].gameObject;
                }
            }

            return null;
        }

        public GameObject Get(GameObject prefab)
        {
            return Get(prefab, null);
        }

        public GameObject Get(GameObject prefab, Transform parent)
        {
            GameObject poolObject = Get();

            if (poolObject != null)
            {
                poolObject.transform.SetParent(parent);
                return poolObject;
            }

            poolObject = UnityEngine.Object.Instantiate(prefab, parent);

            if (Add(poolObject)) return poolObject;

            throw new NullReferenceException(prefab.name + " is not a PoolableGameObject");
        }

        public bool Add(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out PoolableGameObject poolableGameObject))
            {
                poolableGameObjects.Add(poolableGameObject);
                return true;
            }

            return false;
        }

        public bool Remove(GameObject gameObject)
        {
            return Remove(gameObject, false);
        }

        public bool Remove(GameObject gameObject, bool destroy)
        {
            if (!gameObject.TryGetComponent(out PoolableGameObject poolableGameObject)) return false;

            for (int i = 0; i < poolableGameObjects.Count; i++)
            {
                if (poolableGameObjects[i] == poolableGameObject)
                {
                    if (destroy) UnityEngine.Object.Destroy(poolableGameObjects[i].gameObject);

                    poolableGameObjects.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            for (int i = 0; i < poolableGameObjects.Count; i++)
            {
                if (poolableGameObjects[i]== null) continue;
                poolableGameObjects[i].OnPoolDestroyed();
            }
            poolableGameObjects.Clear();
        }
    }

    public class GameObjectPool<T> where T : Component
    {
        private readonly List<ComponentGameObjectLink> poolableGameObjects = new List<ComponentGameObjectLink>();

        public int Count => poolableGameObjects.Count;

        public T this[int index]
        {
            get
            {
                return poolableGameObjects[index].component;
            }
        }

        /// <summary>
        /// Returns component of the first free gameObject, if there's none it returns null.
        /// </summary>
        public T Get()
        {
            for (int i = 0; i < poolableGameObjects.Count; i++)
            {
                if (poolableGameObjects[i].poolableGameObject.CanRetrieve())
                {
                    poolableGameObjects[i].poolableGameObject.OnRetrievedFromPool();
                    return poolableGameObjects[i].component;
                }
            }

            return null;
        }

        /// <summary>
        /// <para>Returns component of the first free gameObject, if there's none it creates one with the prefab and returns it.</para>
        /// The prefab must implement the component and PoolableGameObject.
        /// </summary>
        public T Get(GameObject prefab)
        {
            return Get(prefab, null);
        }

        /// <summary>
        /// <para>Returns component of the first free gameObject, if there's none it creates one with the prefab and returns it.</para>
        /// The prefab must implement the component and PoolableGameObject.
        /// </summary>
        public T Get(T prefab)
        {
            return Get(prefab.gameObject, null);
        }

        /// <summary>
        /// <para>Returns component of the first free gameObject, if there's none it creates one with the prefab as a children of the transform and returns it.</para>
        /// The prefab must implement the component and PoolableGameObject.
        /// </summary>
        public T Get(T prefab, Transform parent)
        {
            return Get(prefab.gameObject, parent);
        }

        /// <summary>
        /// <para>Returns component of the first free gameObject, if there's none it creates one with the prefab as a children of the transform and returns it.</para>
        /// The prefab must implement the component and PoolableGameObject.
        /// </summary>
        public T Get(GameObject prefab, Transform parent)
        {
            T component = Get();

            if (component != null)
            {
                component.transform.SetParent(parent);
                return component;
            }

            GameObject gameObject = UnityEngine.Object.Instantiate(prefab, parent);

            if (Add(gameObject)) return poolableGameObjects[^1].component;

            throw new NullReferenceException(prefab.name + " is not a PoolableGameObject or contains component of type " + typeof(T));
        }

        public bool Add(T component)
        {
            return Add(component.gameObject);
        }

        public bool Add(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out PoolableGameObject poolableGameObject) && gameObject.TryGetComponent(out T component))
            {
                poolableGameObjects.Add(new ComponentGameObjectLink(component, poolableGameObject));
                return true;
            }

            return false;
        }

        public bool Remove(GameObject gameObject)
        {
            return Remove(gameObject, false);
        }

        public bool Remove(GameObject gameObject, bool destroy)
        {
            if (!gameObject.TryGetComponent(out PoolableGameObject poolableGameObject)) return false;

            for (int i = 0; i < poolableGameObjects.Count; i++)
            {
                if (poolableGameObjects[i].poolableGameObject == poolableGameObject)
                {
                    if (destroy) UnityEngine.Object.Destroy(poolableGameObjects[i].poolableGameObject.gameObject);

                    poolableGameObjects.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            for (int i = 0; i < poolableGameObjects.Count; i++)
            {
                if (poolableGameObjects[i].poolableGameObject == null) continue;
                poolableGameObjects[i].poolableGameObject.OnPoolDestroyed();
            }
            poolableGameObjects.Clear();
        }

        private readonly struct ComponentGameObjectLink
        {
            public readonly T component;
            public readonly PoolableGameObject poolableGameObject;

            public ComponentGameObjectLink(T component, PoolableGameObject poolableGameObject)
            {
                this.component = component;
                this.poolableGameObject = poolableGameObject;
            }
        }
    }
}