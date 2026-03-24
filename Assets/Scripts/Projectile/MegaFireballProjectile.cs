using System.Collections.Generic;
using UnityEngine;

public class MegaFireballProjectile : Projectile
{
    [Header("Mega Fireball Settings")]
    [SerializeField] public StatusEffectData statusEffectToApply;
    [SerializeField] private GameObject fireballsPrefab;
    [Space]
    [SerializeField] public float armorPenetration = 0f;
    [SerializeField] public float procCoefficient = 1f;
    [SerializeField] public float knockback = 0f;

    public override bool CanGetPoolable => base.CanGetPoolable && activeFireballs.Count == 0; //FIX THIS, CHECK THE POOLABLEGAMEOBJECT BOOL

    private float damageRadius;
    private Collider2D[] hits;

    private int fireballsAmount;
    private float anglePerFireball;

    private GameObjectPool fireballsPool = new GameObjectPool();
    private HashSet<Projectile> activeFireballs = new HashSet<Projectile>();

    public void SetDamageRadiusAndFireballsAmount(float damageRadius, int fireballsAmount)
    {
        this.damageRadius = damageRadius;
        this.fireballsAmount = fireballsAmount;

        anglePerFireball = 360 / Mathf.Max(1, fireballsAmount);
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

    void Explode()
    {
        hits = Physics2D.OverlapCircleAll(TipPosition, damageRadius, hitLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            OnHit(hits[i].gameObject);
        }

        SpawnFireballs();
        Despawn();
    }

    void OnHit(GameObject receiver)
    {
        bool damageRejected = false;

        if (Physics2D.Linecast(TipPosition, receiver.transform.position, blockLayer))
        {
            return;
        }

        if (TeamManager.IsAlly(SourceUser, receiver))
        {
            return;
        }

        if (receiver.TryGetComponent(out Damageable damageable))
        {
            Damageable.DamageEvent damageEvent = damageable.DealDamage(new DamageInfo(Damage, 0f, 1f), SourceUser, gameObject);
            damageRejected = damageEvent.damageRejected;
        }

        if (damageRejected) return;

        if (receiver.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            statusEffectHandler.ApplyEffect(statusEffectToApply, SourceUser);
        }

        if (receiver.TryGetComponent(out Physics physics))
        {
            physics.AddForce(knockback, Direction);
        }
    }

    void SpawnFireballs()
    {
        for (int i = 0; i < fireballsAmount; i++)
        {
            GameObject fireball = GetNewFireballProjectile();

            float theta = (i + 1) * anglePerFireball * Mathf.PI / 180;
            Vector2 direction = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));

            if (fireball.TryGetComponent(out FireBallProjectile fireballProjectile) && fireball.TryGetComponent(out Projectile projectile))
            {
                projectile.Launch(TipPosition, Speed * .5f, direction, MaxRange * .5f, Damage * .5f, SourceUser);

                fireballProjectile.SetDamageRadius(damageRadius * .5f);
                fireballProjectile.onDespawn += RemoveFromActiveFireballs;
                activeFireballs.Add(projectile);
            }

            fireball.SetActive(true);
        }
    }

    GameObject GetNewFireballProjectile()
    {
        GameObject fireball = fireballsPool.Get();

        if (fireball == null)
        {
            fireball = Instantiate(fireballsPrefab);
            fireballsPool.Add(fireball);
        }

        return fireball;
    }

    void RemoveFromActiveFireballs(Projectile projectile)
    {
        projectile.onDespawn -= RemoveFromActiveFireballs;
        activeFireballs.Remove(projectile);
    }
}