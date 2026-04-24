using System.Collections.Generic;
using UnityEngine;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

public abstract class Bomb : MonoBehaviour, IPoolableGameObjectConfirmator
{
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private DespawnAction despawnAction;

    [Space]

    [SerializeField] protected float knockback;
    [SerializeField] protected float armorPenetration;
    [SerializeField] protected float procCoefficient;

    public bool CanGetPoolable => true;

    protected GameObject source;
    protected Teams team;

    protected float damage = 0f;
    private float fuseTime = 0f;
    private float radius = 0f;

    private bool fuseStarted = false;
    private float fuseTimer = 0f;

    public virtual void OnRetrievedFromPool()
    {
        fuseStarted = false;
        fuseTimer = 0f;
    }

    public void StartFuse(float duration, float damage, float radius, GameObject source, Teams team)
    {
        if (fuseStarted) return;

        fuseTime = duration;
        this.damage = damage;
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
        switch (despawnAction)
        {
            case DespawnAction.Destroy:
                Destroy(gameObject);
                break;

            case DespawnAction.Disable:
                gameObject.SetActive(false);
                break;
        }
    }
}