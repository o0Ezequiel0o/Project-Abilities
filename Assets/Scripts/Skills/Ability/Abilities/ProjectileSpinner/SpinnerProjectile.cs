using UnityEngine;

public class SpinnerProjectile : Projectile
{
    [Header("Basic Projectile Settings")]
    [SerializeField] private float armorPenetration = 0f;
    [SerializeField] private float procCoefficient = 1f;
    [SerializeField] private float knockback = 1f;

    [Header("Spinner Projectile Settings")]
    [SerializeField] private int maxHits = -1;

    private int currentHits = 0;

    protected override void OnLaunch(Vector3 startPosition, float speed, Vector2 direction, float maxRange, float damage, GameObject source)
    {
        currentHits = 0;
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        if (hit.collider.gameObject == SourceUser) return;
        if (objectsNotExited.Contains(hit.collider.gameObject)) return;

        OnImpact(hit);
    }

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
            Vector2 direction = (receiver.transform.position - SourceUser.transform.position).normalized;
            physics.AddForce(direction * knockback);
        }
    }

    private void OnImpact(RaycastHit2D collision)
    {
        HitIfEnemy(collision.collider.gameObject);
    }

    private void HitIfEnemy(GameObject receiver)
    {
        if (TeamManager.IsEnemy(SourceUser, receiver))
        {
            Hit(receiver);
            currentHits += 1;

            if (maxHits < 0) return;

            if (currentHits >= maxHits)
            {
                Despawn();
            }
        }
    }
}