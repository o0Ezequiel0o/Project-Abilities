using System;
using UnityEngine;
using Zeke.PoolableGameObjects;

public class ParticleController : MonoBehaviour, IPoolableGameObjectConfirmator
{
    [SerializeField] private ParticleSystem particlePrefab;
    [SerializeField] private Vector3 offset;

    [Space]

    [SerializeField] private bool follow;

    public Action<IPoolableGameObjectConfirmator> PoolableReady { get; set; }
    public Action<IPoolableGameObjectConfirmator> PoolableBusy { get; set; }

    private ParticleSystem particleInstance;
    private bool isRunning = false;

    public void TriggerParticles()
    {
        if (particleInstance == null) return;

        particleInstance.transform.position = gameObject.transform.position;
        particleInstance.gameObject.SetActive(true);
        particleInstance.Play();
        isRunning = true;

        PoolableBusy?.Invoke(this);
    }

    public void StopParticles()
    {
        if (particleInstance == null) return;

        particleInstance.gameObject.SetActive(false);
        particleInstance.Stop();
        isRunning = false;

        PoolableReady?.Invoke(this);
    }

    private void Awake()
    {
        particleInstance = Instantiate(particlePrefab);
        particleInstance.gameObject.SetActive(false);

        ParticleSystem.MainModule main = particleInstance.main;
        main.stopAction = ParticleSystemStopAction.Disable;
    }

    private void Update()
    {
        if (!isRunning || particleInstance.IsAlive()) return;

        particleInstance.gameObject.SetActive(false);
        particleInstance.Stop();
        isRunning = false;

        PoolableReady?.Invoke(this);
    }

    private void LateUpdate()
    {
        if (particleInstance == null && isRunning && follow) return;
        particleInstance.transform.position = transform.position + offset;
    }

    private void OnDestroy()
    {
        if (particleInstance == null) return;

        if (particleInstance.isPlaying)
        {
            ParticleSystem.MainModule main = particleInstance.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }
        else
        {
            Destroy(particleInstance.gameObject);
        }
    }
}