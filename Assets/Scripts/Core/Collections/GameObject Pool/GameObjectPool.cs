using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zeke.PoolableGameObjects
{
    public interface IGameObjectPool
    {
        public abstract void Release(PoolableGameObject poolable);

        public abstract void Remove(PoolableGameObject poolable);
    }

    public class GameObjectPool<T> : IGameObjectPool where T : Component
    {
        private readonly Stack<ComponentGameObjectLink> free = new Stack<ComponentGameObjectLink>();
        private readonly List<ComponentGameObjectLink> busy = new List<ComponentGameObjectLink>();

        public int BusyCount => busy.Count;
        public int FreeCount => free.Count;

        public T GetActive(int index) => busy[index].component;

        public void Release(PoolableGameObject poolable)
        {
            for (int i = 0; i < busy.Count; i++)
            {
                if (busy[i].poolableGameObject == poolable)
                {
                    free.Push(busy[i]);
                    busy.RemoveAt(i);

                    poolable.OnReleased();

                    break;
                }
            }
        }

        public void Remove(PoolableGameObject poolable)
        {
            for (int i = 0; i < busy.Count; i++)
            {
                if (busy[i].poolableGameObject == poolable)
                {
                    busy.RemoveAt(i);

                    poolable.onRelease -= Release;
                    poolable.onDestroy -= Remove;

                    break;
                }
            }
        }

        /// <summary>
        /// Returns component of the first free gameObject, if there's none it returns null.
        /// </summary>
        public T Get()
        {
            if (free.Count == 0) return null;

            ComponentGameObjectLink linkedGO = free.Pop();

            linkedGO.poolableGameObject.OnRetrievedFromPool();
            busy.Add(linkedGO);

            return linkedGO.component;
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

            if (Add(gameObject)) return Get();

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
                free.Push(new ComponentGameObjectLink(component, poolableGameObject));

                poolableGameObject.onRelease += Release;
                poolableGameObject.onDestroy += Remove;

                return true;
            }

            return false;
        }

        public void Clear()
        {
            while (free.Count > 0)
            {
                ComponentGameObjectLink linkedGO = free.Pop();
                if (linkedGO.poolableGameObject == null) continue;

                linkedGO.poolableGameObject.onDestroy -= Remove;
                linkedGO.poolableGameObject.onRelease -= Release;

                linkedGO.poolableGameObject.OnPoolDestroyed();
            }

            for (int i = 0; i < busy.Count; i++)
            {
                if (busy[i].poolableGameObject == null) continue;

                busy[i].poolableGameObject.onDestroy -= Remove;
                busy[i].poolableGameObject.onRelease -= Release;

                busy[i].poolableGameObject.OnPoolDestroyed();
            }

            free.Clear();
            busy.Clear();
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