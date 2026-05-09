using System;
using UnityEngine;
using Zeke.PoolableGameObjects;

public class SoundInstance : MonoBehaviour, IPoolableGameObjectConfirmator
{
    [SerializeField] private AudioSource audioSource;

    public Action<IPoolableGameObjectConfirmator> PoolableReady { get; set; }
    public Action<IPoolableGameObjectConfirmator> PoolableBusy { get; set; }

    private bool startedPlaying = false;

    public void OnRetrievedFromPool() { }

    public void OnSentToPool()
    {
        startedPlaying = false;
    }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            if (!startedPlaying)
            {
                PoolableBusy.Invoke(this);
                startedPlaying = true;
            }
        }
        else if (startedPlaying)
        {
            PoolableReady?.Invoke(this);
            startedPlaying = false;
        }
    }
}