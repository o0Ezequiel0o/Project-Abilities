using UnityEngine;
using Zeke.TeamSystem;

public class SpinnerProjectile : DamageProjectileBase
{
    [SerializeField] private bool allyCollision;

    [Header("Spinner Projectile Settings")]
    [SerializeField] private int maxHits = -1;

    private int currentHits = 0;

    protected override void OnLaunch(Vector3 startPosition, float speed, Vector2 direction, float maxRange)
    {
        currentHits = 0;
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        GameObject receiver = hit.collider.gameObject;

        if (receiver == SourceUser) return;
        if (objectsNotExited.Contains(hit.collider.gameObject)) return;

        if (TeamManager.IsEnemy(Team, receiver))
        {
            Hit(hit.transform.gameObject);
        }
        else if (!allyCollision) return;

        UpdatePiercing();
    }

    protected void UpdatePiercing()
    {
        currentHits += 1;

        if (maxHits >= 0 && currentHits >= maxHits)
        {
            Despawn();
        }
    }

    protected void Hit(GameObject receiver)
    {
        bool damageRejected = DealDamage(receiver);

        if (damageRejected) return;

        if (SourceUser != null)
        {
            Vector2 direction = (receiver.transform.position - SourceUser.transform.position).normalized;
            ApplyKnockback(receiver, direction);
        }
    }
}