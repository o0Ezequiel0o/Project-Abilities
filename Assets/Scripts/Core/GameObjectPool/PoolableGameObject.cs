using UnityEngine;

public class PoolableGameObject : MonoBehaviour
{
    private IPoolableGameObjectConfirmator[] poolableConfirmators;

    private void Awake()
    {
        poolableConfirmators = GetComponentsInChildren<IPoolableGameObjectConfirmator>();
    }

    public bool ReadyToRetrieve()
    {
        if (gameObject.activeSelf)
        {
            return false;
        }

        for (int i = 0; i < poolableConfirmators.Length; i++)
        {
            if (!poolableConfirmators[i].CanGetPoolable)
            {
                return false;
            }
        }

        return true;
    }

    public void OnPoolableGet()
    {
        for (int i = 0; i < poolableConfirmators.Length; i++)
        {
            poolableConfirmators[i].OnPoolableGet();
        }
    }
}