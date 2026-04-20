using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageableParticles : MonoBehaviour
{
    [SerializeField] private List<HitParticlesInfo> particlesConfig;

    private readonly List<HitParticles> hitParticles = new List<HitParticles>();

    void Awake()
    {
        if (TryGetComponent(out Damageable damageable))
        {
            SubscribeToEvents(damageable);
            CreateHitParticles();
        }
    }

    void Update()
    {
        for (int i = 0; i < hitParticles.Count; i++)
        {
            hitParticles[i].timer += Time.deltaTime;
        }
    }

    void CreateHitParticles()
    {
        for (int i = 0; i < particlesConfig.Count; i++)
        {
            hitParticles.Add(particlesConfig[i].CreateHitParticles());
        }
    }

    void SubscribeToEvents(Damageable damageable)
    {
        damageable.onTakenDamage += OnDamageTaken;
        damageable.onHitTaken += OnHitTaken;
        damageable.onDeath += OnDeath;
    }

    void OnDamageTaken(Damageable.DamageEvent _)
    {
        SpawnParticles(ParticleTrigger.Damage);
    }

    void OnHitTaken(Damageable.DamageEvent _)
    {
        SpawnParticles(ParticleTrigger.Hit);
    }

    void OnDeath(Damageable.DamageEvent _)
    {
        SpawnParticles(ParticleTrigger.Death);
    }

    void SpawnParticles(ParticleTrigger trigger)
    {
        for (int i = 0; i < particlesConfig.Count; i++)
        {
            if (hitParticles[i].info.trigger != trigger)
            {
                continue;
            }

            if (hitParticles[i].info.minInterval > hitParticles[i].timer)
            {
                continue;
            }

            if (hitParticles[i].info.follow)
            {
                Instantiate(hitParticles[i].info.particles, transform);
            }
            else
            {
                Instantiate(hitParticles[i].info.particles, transform.position, transform.rotation);
            }
        }
    }

    [Serializable]
    private struct HitParticlesInfo
    {
        public ParticleSystem particles;
        public ParticleTrigger trigger;
        public float minInterval;
        public bool follow;

        public HitParticles CreateHitParticles()
        {
            return new HitParticles(this);
        }
    }

    private class HitParticles
    {
        public HitParticlesInfo info;
        public float timer;

        public HitParticles(HitParticlesInfo info)
        {
            this.info = info;
            timer = 0f;
        }
    }

    private enum ParticleTrigger
    {
        Hit,
        Damage,
        Death
    }
}