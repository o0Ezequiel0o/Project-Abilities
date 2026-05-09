using System.Collections.Generic;
using UnityEngine;

namespace Zeke.PoolableGameObjects 
{
    public static class PoolableGameObjectsManager
    {
        private static readonly HashSet<PoolableGameObject> poolableGameObjects = new HashSet<PoolableGameObject>();

        public static void MarkPoolableForDestroy(PoolableGameObject poolableGameObject)
        {
            if (poolableGameObjects.Contains(poolableGameObject)) return;

            poolableGameObject.onRelease += DestroyPoolable;
            poolableGameObject.onDestroy += UnmarkPoolableForDestroy;

            poolableGameObjects.Add(poolableGameObject);
        }

        public static void UnmarkPoolableForDestroy(PoolableGameObject poolableGameObject)
        {
            if (!poolableGameObjects.Contains(poolableGameObject)) return;

            poolableGameObjects.Remove(poolableGameObject);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Clear()
        {
            poolableGameObjects.Clear();
        }

        private static void DestroyPoolable(PoolableGameObject poolableGameObject)
        {
            poolableGameObjects.Remove(poolableGameObject);
            GameObject.Destroy(poolableGameObject.gameObject);
        }
    }
}