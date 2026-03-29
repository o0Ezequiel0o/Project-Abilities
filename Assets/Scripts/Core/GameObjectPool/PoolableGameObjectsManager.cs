using System.Collections.Generic;
using UnityEngine;

namespace Zeke.PoolableGameObjects 
{
    public static class PoolableGameObjectsManager 
    {
        private static readonly HashSet<PoolableGameObject> poolableGameObjectsQuickLookUp = new HashSet<PoolableGameObject>();
        private static readonly List<PoolableGameObject> poolableGameObjects = new List<PoolableGameObject>();

        public static void MarkPoolableForDestroy(PoolableGameObject poolableGameObject)
        {
            if (poolableGameObjectsQuickLookUp.Contains(poolableGameObject)) return;

            poolableGameObjects.Add(poolableGameObject);
            poolableGameObjectsQuickLookUp.Add(poolableGameObject);
        }

        public static void UnmarkPoolableForDestroy(PoolableGameObject poolableGameObject)
        {
            if (!poolableGameObjectsQuickLookUp.Contains(poolableGameObject)) return;

            poolableGameObjects.Remove(poolableGameObject);
            poolableGameObjectsQuickLookUp.Remove(poolableGameObject);
        }

        public static void Update()
        {
            if (poolableGameObjects.Count == 0) return;

            for (int i = poolableGameObjects.Count - 1; i >= 0; i--)
            {
                if (poolableGameObjects[i].CanRetrieve())
                {
                    poolableGameObjectsQuickLookUp.Remove(poolableGameObjects[i]);
                    GameObject.Destroy(poolableGameObjects[i].gameObject);
                    poolableGameObjects.RemoveAt(i);
                }
            }
        }
        
        public static void Clear()
        {
            poolableGameObjectsQuickLookUp.Clear();
            poolableGameObjects.Clear();
        }
    }
}