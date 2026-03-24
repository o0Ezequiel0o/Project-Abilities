using UnityEngine;

public class FireBallProjectile : Projectile
{
    [Header("Fireball Settings")]
    public StatusEffectData statusEffectToApply;

    [Space]

    public float armorPenetration = 0f;
    public float procCoefficient = 1f;
    public float knockback = 1f;

    private float damageRadius;
    private Collider2D[] hits;

    public void SetDamageRadius(float damageRadius)
    {
        this.damageRadius = damageRadius;
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

    void Explode()
    {
        hits = Physics2D.OverlapCircleAll(TipPosition, damageRadius, hitLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            OnHit(hits[i].gameObject);
        }

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
            Damageable.DamageEvent damageEvent = damageable.DealDamage(new DamageInfo(Damage, procCoefficient, armorPenetration), SourceUser, gameObject);
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
}