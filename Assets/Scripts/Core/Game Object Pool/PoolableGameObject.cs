using System;
using UnityEngine;
using UnityEngine.Events;
using Zeke.PoolableGameObjects;

namespace Zeke.PoolableGameObjects
{
    [DisallowMultipleComponent]
    public class PoolableGameObject : MonoBehaviour
    {
        [SerializeField] private UnityEvent onRetrieved;
        [SerializeField] private UnityEvent onReleased;

        public Action<PoolableGameObject> onRelease;
        public Action<PoolableGameObject> onDestroy;

        private PoolableConfirmator[] confirmators;
        private IPoolableGameObjectListener[] listeners;

        private bool ready = false;
        private int timesRetrieved = 0;

        private struct PoolableConfirmator
        {
            public readonly IPoolableGameObjectConfirmator confirmator;
            public bool ready;

            public PoolableConfirmator(IPoolableGameObjectConfirmator confirmator)
            {
                this.confirmator = confirmator;
                ready = false;
            }
        }

        public void OnRetrievedFromPool()
        {
            ready = false;
            timesRetrieved += 1;

            for (int i = 0; i < confirmators.Length; i++)
            {
                OnPoolableStateUpdate(new PoolableConfirmatorUpdate(confirmators[i].confirmator, false));
            }

            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].OnRetrievedFromPool();
            }

            onRetrieved?.Invoke();
        }

        public void OnReleased()
        {
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].OnSentToPool();
            }

            onReleased?.Invoke();
        }

        public void OnPoolDestroyed()
        {
            if (ready) Destroy(gameObject);
            else PoolableGameObjectsManager.MarkPoolableForDestroy(this);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void Awake()
        {
            IPoolableGameObjectConfirmator[] poolableConfirmators = GetComponentsInChildren<IPoolableGameObjectConfirmator>();

            listeners = GetComponentsInChildren<IPoolableGameObjectListener>();
            confirmators = new PoolableConfirmator[poolableConfirmators.Length];

            for (int i = 0; i < poolableConfirmators.Length; i++)
            {
                PoolableConfirmator poolableConfirmator = new PoolableConfirmator(poolableConfirmators[i]);

                poolableConfirmator.confirmator.PoolableReady += OnPoolableReady;
                poolableConfirmator.confirmator.PoolableBusy += OnPoolableBusy;

                confirmators[i] = poolableConfirmator;
            }
        }

        private void OnPoolableReady(IPoolableGameObjectConfirmator confirmator)
        {
            OnPoolableStateUpdate(new PoolableConfirmatorUpdate(confirmator, true));
        }

        private void OnPoolableBusy(IPoolableGameObjectConfirmator confirmator)
        {
            OnPoolableStateUpdate(new PoolableConfirmatorUpdate(confirmator, false));
        }

        private void OnPoolableStateUpdate(PoolableConfirmatorUpdate confirmatorUpdate)
        {
            ready = true;

            for (int i = 0; i < confirmators.Length; i++)
            {
                if (confirmators[i].confirmator == confirmatorUpdate.confirmator)
                {
                    confirmators[i].ready = confirmatorUpdate.ready;
                }

                if (!confirmators[i].ready)
                {
                    ready = false;
                }
            }

            if (ready)
            {
                gameObject.SetActive(false);
                onRelease?.Invoke(this);
            }
        }

        private void OnDestroy()
        {
            onDestroy?.Invoke(this);
        }
    }
}

public readonly struct PoolableConfirmatorUpdate
{
    public readonly IPoolableGameObjectConfirmator confirmator;
    public readonly bool ready;

    public PoolableConfirmatorUpdate(IPoolableGameObjectConfirmator confirmator, bool ready)
    {
        this.confirmator = confirmator;
        this.ready = ready;
    }
}