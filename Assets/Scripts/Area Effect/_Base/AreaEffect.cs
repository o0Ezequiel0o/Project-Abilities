using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zeke.PoolableGameObjects;

public abstract class AreaEffect : MonoBehaviour, IPoolableGameObjectConfirmator, IPoolableGameObjectListener
{
    [SerializeField] private LayerMask hitLayers;

    [field: Header("Events")]
    [field: SerializeField] public UnityEvent<AreaEffect> OnDespawn { get; private set; }

    public Action<IPoolableGameObjectConfirmator> PoolableReady { get; set; }
    public Action<IPoolableGameObjectConfirmator> PoolableBusy { get; set; }

    protected float tickInterval = 0f;
    protected int currentTick = 0;
    protected int ticks = 0;

    protected float radius = 0f;

    private float timer = 0f;

    private readonly List<Collider2D> hits = new List<Collider2D>();

    public virtual void OnSentToPool() { }

    public virtual void OnRetrievedFromPool()
    {
        hits.Clear();
        timer = 0f;
        currentTick = 0;
    }

    public void CreateAreaEffect(int ticks, float tickInterval, float radius)
    {
        this.ticks = ticks;
        this.tickInterval = tickInterval;

        this.radius = radius;
    }

    protected abstract void OnTick(List<Collider2D> hits, int count);

    private void Update()
    {
        if (ticks > 0)
        {
            timer += Time.deltaTime;

            if (timer >= tickInterval)
            {
                timer = 0f;
                Tick();
            }
        }
        else
        {
            Despawn();
        }
    }

    private void Tick()
    {
        currentTick += 1;

        ContactFilter2D contactFilter = new ContactFilter2D()
        {
            layerMask = hitLayers,
            useLayerMask = true
        };

        OnTick(hits, Physics2D.OverlapCircle(transform.position, radius, contactFilter, hits));

        if (currentTick > ticks)
        {
            Despawn();
        }
    }

    private void Despawn()
    {
        OnDespawn?.Invoke(this);
        PoolableReady?.Invoke(this);
    }
}