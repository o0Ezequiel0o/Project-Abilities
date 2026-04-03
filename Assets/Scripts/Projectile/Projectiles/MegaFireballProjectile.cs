using System.Collections.Generic;
using UnityEngine;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

public class MegaFireballProjectile : DamageProjectileBase
{
    [Header("Mega Fireball | Settings")]
    [SerializeField] public StatusEffectData statusEffectToApply;
    [SerializeField] private GameObject fireballsPrefab;

    public override bool CanGetPoolable => base.CanGetPoolable && activeFireballs.Count == 0;

    private float damageRadius;

    private int fireballsAmount;
    private float anglePerFireball;

    private readonly List<Collider2D> hits = new List<Collider2D>();
    private readonly GameObjectPool fireballsPool = new GameObjectPool();
    private readonly HashSet<Projectile> activeFireballs = new HashSet<Projectile>();

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, float damage, float damageRadius, int fireballsAmount, GameObject source, Teams team)
    {
        this.damageRadius = damageRadius;
        this.fireballsAmount = fireballsAmount;

        anglePerFireball = 360 / Mathf.Max(1, fireballsAmount);
        Launch(position, speed, direction, maxRange, damage, source, team);
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        if (hit.collider.gameObject == SourceUser) return;

        TeleportToHitPoint(hit.point);
        StopLoopingHits();
        Explode();
    }

    protected override void OnMaxDistanceReached()
    {
        Explode();
    }

    protected override void OnDestroy()
    {
        fireballsPool.Clear();
    }

    private void Explode()
    {
        hits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayer, useLayerMask = true };
        Physics2D.OverlapCircle(TipPosition, damageRadius, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            Hit(hits[i].gameObject);
        }

        SpawnFireballs();
        Despawn();
    }

    private void Hit(GameObject receiver)
    {
        if (TeamManager.IsAlly(Team, receiver)) return;

        if (Physics2D.Linecast(TipPosition, receiver.transform.position, blockLayer)) return;

        bool damageRejected = DealDamage(receiver);

        if (damageRejected) return;

        if (receiver.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            statusEffectHandler.ApplyEffect(statusEffectToApply, SourceUser);
        }

        ApplyKnockback(receiver, Direction);
    }

    private void SpawnFireballs()
    {
        for (int i = 0; i < fireballsAmount; i++)
        {
            GameObject fireball = GetNewFireballProjectile();

            float theta = (i + 1) * anglePerFireball * Mathf.PI / 180;
            Vector2 direction = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));

            if (fireball.TryGetComponent(out FireBallProjectile fireballProjectile))
            {
                fireballProjectile.Launch(TipPosition, Speed * .5f, direction, MaxRange * .5f, Damage * .5f, damageRadius * .5f, SourceUser, Team);
                fireballProjectile.onDespawn += RemoveFromActiveFireballs;
                activeFireballs.Add(fireballProjectile);
            }

            fireball.SetActive(true);
        }
    }

    private GameObject GetNewFireballProjectile()
    {
        GameObject fireball = fireballsPool.Get();

        if (fireball == null)
        {
            fireball = Instantiate(fireballsPrefab);
            fireballsPool.Add(fireball);
        }

        return fireball;
    }

    private void RemoveFromActiveFireballs(Projectile projectile)
    {
        projectile.onDespawn -= RemoveFromActiveFireballs;
        activeFireballs.Remove(projectile);
    }
}