using System.Collections.Generic;
using UnityEngine;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

public class MegaFireballProjectile : DamageProjectileBase
{
    [SerializeField] private bool allyCollision;

    [Header("Mega Fireball | Settings")]
    [SerializeField] public StatusEffectData statusEffectToApply;
    [SerializeField] private FireBallProjectile fireballsPrefab;

    private float damageRadius;

    private int fireballsAmount;
    private float anglePerFireball;

    private readonly List<Collider2D> hits = new List<Collider2D>();
    private readonly GameObjectPool<FireBallProjectile> fireballsPool = new GameObjectPool<FireBallProjectile>();
    private readonly HashSet<Projectile> activeFireballs = new HashSet<Projectile>();

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, DamageData damageData, float knockback, float damageRadius, int fireballsAmount, GameObject source, Teams team)
    {
        this.damageRadius = damageRadius;
        this.fireballsAmount = fireballsAmount;

        anglePerFireball = 360 / Mathf.Max(1, fireballsAmount);
        Launch(position, speed, direction, maxRange, damageData, knockback, source, team);
    }

    protected override void OnDespawned() { }

    protected override void OnCollision(RaycastHit2D hit)
    {
        GameObject receiver = hit.collider.gameObject;

        if (receiver == SourceUser) return;

        if (TeamManager.IsEnemy(Team, receiver))
        {
            TeleportToHitPoint(hit.point);
            Explode();
        }
        else if (!allyCollision) return;
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
            if (TeamManager.IsEnemy(Team, hits[i].gameObject))
            {
                Hit(hits[i].gameObject);
            }
        }

        SpawnFireballs();
        Despawn();
    }

    private void Hit(GameObject receiver)
    {
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
            FireBallProjectile fireball = GetNewFireballProjectile();

            float theta = (i + 1) * anglePerFireball * Mathf.PI / 180;
            Vector2 direction = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));

            DamageData fireballProjectileDamageData = new DamageData(Damage * 5f, armorPenetration, procCoefficient);
            fireball.Launch(TipPosition, Speed * .5f, direction, MaxRange * .5f, fireballProjectileDamageData, damageRadius * .5f, SourceUser, Team);
            fireball.OnDespawn.AddListener(RemoveFromActiveFireballs);
            activeFireballs.Add(fireball);

            fireball.gameObject.SetActive(true);
        }
    }

    private FireBallProjectile GetNewFireballProjectile()
    {
        FireBallProjectile fireball = fireballsPool.Get();

        if (fireball == null)
        {
            fireball = Instantiate(fireballsPrefab);
            fireballsPool.Add(fireball);
        }

        return fireball;
    }

    private void RemoveFromActiveFireballs(Projectile projectile)
    {
        projectile.OnDespawn.RemoveListener(RemoveFromActiveFireballs);
        activeFireballs.Remove(projectile);

        if (activeFireballs.Count <= 0)
        {
            PoolableReady?.Invoke(this);
        }
    }
}