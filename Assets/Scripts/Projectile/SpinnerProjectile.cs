using UnityEngine;

public class SpinnerProjectile : DamageProjectileBase
{
    [Header("Spinner Projectile Settings")]
    [SerializeField] private int maxHits = -1;

    private int currentHits = 0;

    protected override void OnLaunch(Vector3 startPosition, float speed, Vector2 direction, float maxRange)
    {
        currentHits = 0;
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        if (hit.collider.gameObject == SourceUser) return;
        if (objectsNotExited.Contains(hit.collider.gameObject)) return;

        Hit(hit.transform.gameObject);
    }

    protected void Hit(GameObject receiver)
    {
        if (TeamManager.IsAlly(Team, receiver)) return;

        currentHits += 1;

        if (maxHits >= 0 && currentHits >= maxHits)
        {
            Despawn();
        }

        bool damageRejected = DealDamage(receiver);

        if (damageRejected) return;

        if (SourceUser != null)
        {
            Vector2 direction = (receiver.transform.position - SourceUser.transform.position).normalized;
            ApplyKnockback(receiver, direction);
        }
    }
}