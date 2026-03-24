using System.Collections.Generic;
using UnityEngine;
using System;

public class ParticleHandler : MonoBehaviour, IPoolableGameObjectConfirmator
{
    [Header("Settings")]
    [SerializeField] private StopTrigger stopAction;
    [SerializeField] private List<ParticleHandlerContainer> spawnParticles;

    public bool CanGetPoolable => !IsAlive();

    private readonly List<ParticlesData> spawnedParticles = new List<ParticlesData>();

    private void Awake()
    {
        SpawnParticles();
    }

    private void Start()
    {
        CallEvents(EventTrigger.Start);
    }

    private void OnEnable()
    {
        CallEvents(EventTrigger.OnEnable);
    }

    private void OnDisable()
    {
        CallEvents(EventTrigger.OnDisable);
    }

    private void LateUpdate()
    {
        Follow();
    }

    private void OnDestroy()
    {
        CallEvents(EventTrigger.OnDestroy);
        DestroyParticles();
    }

    public void SpawnParticles()
    {
        for (int i = 0; i < spawnParticles.Count; i++)
        {
            SpawnParticle(spawnParticles[i].SpawnParticles(transform.position));
        }
    }

    public bool IsAlive()
    {
        for (int i = 0; i < spawnedParticles.Count; i++)
        {
            if (spawnedParticles[i].particles.IsAlive())
            {
                return true;
            }
        }

        return false;
    }

    public void DestroyParticles()
    {
        for (int i = 0; i < spawnedParticles.Count; i++)
        {
            if (spawnedParticles[i].particles == null) continue;

            if (spawnedParticles[i].enable == EventTrigger.OnDestroy)
            {
                ScheduleDestroy(spawnedParticles[i].particles);
            }
            else
            {
                Destroy(spawnedParticles[i].particles.gameObject);
            }
        }
    }

    public void DestroyParticlesInstantly()
    {
        for (int i = 0; i < spawnedParticles.Count; i++)
        {
            if (spawnedParticles[i].particles == null) continue;
            Destroy(spawnedParticles[i].particles.gameObject);
        }
    }

    public void EnableParticles()
    {
        for (int i = 0; i < spawnedParticles.Count; i++)
        {
            spawnedParticles[i].particles.Play();
            spawnedParticles[i].particles.gameObject.SetActive(true);
        }
    }

    public void DisableParticles()
    {
        for (int i = 0; i < spawnedParticles.Count; i++)
        {
            spawnedParticles[i].particles.gameObject.SetActive(false);
        }
    }

    public void OnPoolableGet() { }

    private void SpawnParticle(ParticlesData particleData)
    {
        spawnedParticles.Add(particleData);
        particleData.particles.Stop();

        particleData.particles.gameObject.SetActive(false);

        ParticleSystem.MainModule main = particleData.particles.main;
        main.stopAction = GetParticleSystemStopAction();
    }

    private void CallEvents(EventTrigger eventTrigger)
    {
        EnableParticles(eventTrigger);

        switch (eventTrigger)
        {
            case EventTrigger.OnDisable:
                StopParticles(StopTrigger.OnDisable);
                break;

            case EventTrigger.OnDestroy:
                StopParticles(StopTrigger.OnDestroy);
                break;
        }
    }

    private void ScheduleDestroy(ParticleSystem particles)
    {
        ParticleSystem.MainModule main = particles.main;
        main.stopAction = ParticleSystemStopAction.Destroy;

        if (main.loop)
        {
            Debug.LogWarning($"{name} | You're spawning a looping particleSystem onDestroy. It is automatically destroyed");
        }
    }

    private void EnableParticles(EventTrigger enableTrigger)
    {
        for (int i = 0; i < spawnedParticles.Count; i++)
        {
            if (spawnedParticles[i].particles == null) continue;

            if (spawnedParticles[i].enable == enableTrigger)
            {
                spawnedParticles[i].particles.gameObject.SetActive(true);
                spawnedParticles[i].particles.Play();
            }
        }
    }

    private void StopParticles(StopTrigger stopTrigger)
    {
        if (stopAction == stopTrigger)
        {
            StopLooping();
        }
    }

    private void Follow()
    {
        for (int i = 0; i < spawnedParticles.Count; i++)
        {
            spawnedParticles[i].particles.transform.SetPositionAndRotation(
                transform.position + spawnedParticles[i].offset,
                Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 90f));
        }
    }

    private void StopLooping()
    {
        for (int i = 0; i < spawnedParticles.Count; i++)
        {
            if (spawnedParticles[i].particles.main.loop)
            {
                spawnedParticles[i].particles.Stop();
            }
        }
    }

    private ParticleSystemStopAction GetParticleSystemStopAction()
    {
        switch (stopAction)
        {
            case StopTrigger.OnDisable:
                return ParticleSystemStopAction.Disable;

            case StopTrigger.OnDestroy:
                return ParticleSystemStopAction.Destroy;

            case StopTrigger.None:
                return ParticleSystemStopAction.None;

            default:
                return ParticleSystemStopAction.None;
        }
    }

    [Serializable]
    private struct ParticleHandlerContainer
    {
        [SerializeField] private ParticleSystem particlesPrefab;
        [SerializeField] private Vector3 prefabOffset;
        
        [SerializeField] public EventTrigger enable;

        public ParticlesData SpawnParticles(Vector3 position)
        {
            return new ParticlesData(Instantiate(particlesPrefab, position + prefabOffset, Quaternion.identity), prefabOffset, enable);
        }
    }

    private struct ParticlesData
    {
        public ParticleSystem particles;
        public Vector3 offset;

        public EventTrigger enable;

        public ParticlesData(ParticleSystem particles, Vector3 offset, EventTrigger enable)
        {
            this.particles = particles;
            this.offset = offset;
            this.enable = enable;
        }
    }

    private enum StopTrigger
    {
        None,
        OnDisable,
        OnDestroy
    }

    private enum EventTrigger
    {
        Start,
        OnEnable,
        OnDisable,
        OnDestroy
    }
}