using UnityEngine;

public class BasicProjectile : Projectile
{
    [Header("Basic Projectile Settings")]
    [SerializeField] private float armorPenetration = 0f;
    [SerializeField] private float procCoefficient = 1f;
    [SerializeField] private float knockback = 1f;

    protected override void OnCollision(RaycastHit2D hit)
    {
        if (hit.collider.gameObject != SourceUser)
        {
            OnImpact(hit);
            StopLoopingHits();
        }
    }

    protected override void OnMaxDistanceReached()
    {
        Despawn();
    }

    protected virtual void OnHitInternal(GameObject receiver, bool damageRejected) { }
    
    protected void Hit(GameObject receiver)
    {
        bool damageRejected = false;

        if (receiver.TryGetComponent(out Damageable damageable))
        {
            Damageable.DamageEvent damageEvent = damageable.DealDamage(new DamageInfo(Damage, procCoefficient, armorPenetration), SourceUser, gameObject);

            damageRejected = damageEvent.damageRejected;
        }

        if (!damageRejected && knockback != 0f && receiver.TryGetComponent(out Physics physics))
        {
            physics.AddForce(knockback, Direction);
        }

        OnHitInternal(receiver, damageRejected);
    }

    private void OnImpact(RaycastHit2D collision)
    {
        TeleportToHitPoint(collision.point);
        HitIfEnemy(collision.collider.gameObject);
        Despawn();
    }

    private void HitIfEnemy(GameObject receiver)
    {
        if (TeamManager.IsEnemy(SourceUser, receiver))
        {
            Hit(receiver);
        }
    }
}