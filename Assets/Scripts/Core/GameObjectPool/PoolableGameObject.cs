using UnityEngine;

namespace Zeke.PoolableGameObjects
{
    public class PoolableGameObject : MonoBehaviour
    {
        private IPoolableGameObjectConfirmator[] poolableConfirmators;

        private void Awake()
        {
            poolableConfirmators = GetComponentsInChildren<IPoolableGameObjectConfirmator>();
        }

        public bool CanRetrieve()
        {
            if (gameObject.activeSelf) return false;

            for (int i = 0; i < poolableConfirmators.Length; i++)
            {
                if (!poolableConfirmators[i].CanGetPoolable)
                {
                    return false;
                }
            }

            return true;
        }

        public void OnRetrievedFromPool()
        {
            for (int i = 0; i < poolableConfirmators.Length; i++)
            {
                poolableConfirmators[i].OnRetrievedFromPool();
            }
        }

        public void OnPoolDestroyed()
        {
            if (CanRetrieve())
            {
                Destroy(gameObject);
            }
            else
            {
                PoolableGameObjectsManager.MarkPoolableForDestroy(this);
            }
        }

        private void OnDestroy()
        {
            PoolableGameObjectsManager.UnmarkPoolableForDestroy(this);
        }
    }
}