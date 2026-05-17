using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

public abstract class Bomb : MonoBehaviour, IPoolableGameObjectConfirmator, IPoolableGameObjectListener
{
    [SerializeField] private LayerMask hitLayers;

    [field: Header("Events")]
    [field: SerializeField] public UnityEvent<Bomb> OnDespawn { get; private set; }

    public Action<IPoolableGameObjectConfirmator> PoolableReady { get; set; }
    public Action<IPoolableGameObjectConfirmator> PoolableBusy { get; set; }

    protected float knockback = 0f;
    protected float armorPenetration = 0f;
    protected float procCoefficient = 0f;

    protected GameObject source;
    protected Teams team;

    protected float damage = 0f;
    private float fuseTime = 0f;
    private float radius = 0f;

    private bool fuseStarted = false;
    private float fuseTimer = 0f;

    public virtual void OnSentToPool()
    {
        fuseStarted = false;
        fuseTimer = 0f;
    }

    public virtual void OnRetrievedFromPool() { }

    public void StartFuse(float duration, float radius, DamageData damageData, float knockback, GameObject source, Teams team)
    {
        if (fuseStarted) return;

        fuseTime = duration;

        damage = damageData.damage;
        armorPenetration = damageData.armorPenetration;
        procCoefficient = damageData.procCoefficient;

        this.knockback = knockback;

        this.radius = radius;
        this.source = source;
        this.team = team;

        fuseStarted = true;
        fuseTimer = 0f;
    }

    private void Update()
    {
        if (!fuseStarted) return;

        fuseTimer += Time.deltaTime;

        if (fuseTimer > fuseTime)
        {
            Explode();
            Despawn();
        }
    }

    protected abstract void Hit(Collider2D hit);

    protected void ApplyKnockback(GameObject receiver, Vector2 direction)
    {
        if (knockback != 0f && receiver.TryGetComponent(out Physics physics))
        {
            physics.AddForce(knockback, direction);
        }
    }

    private void Explode()
    {
        List<Collider2D> hits = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D()
        {
            layerMask = hitLayers,
            useLayerMask = true
        };

        Physics2D.OverlapCircle(transform.position, radius, contactFilter, hits);

        for (int i  = 0; i < hits.Count; i++)
        {
            Hit(hits[i]);
        }
    }

    private void Despawn()
    {
        OnDespawn?.Invoke(this);
        PoolableReady?.Invoke(this);
    }
}