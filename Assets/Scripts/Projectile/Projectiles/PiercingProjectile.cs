using UnityEngine;
using Zeke.TeamSystem;

public class PiercingProjectile : DamageProjectileBase
{
    [Header("Piercing")]
    [SerializeField] private int pierce = -1;

    private int currentHits = 0;

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, float damage, int pierce, GameObject source, Teams team)
    {
        this.pierce = pierce;
        Launch(position, speed, direction, maxRange, damage, source, team);
    }

    protected virtual void OnHit(GameObject receiver) { }

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

    protected virtual void Hit(GameObject receiver)
    {
        if (TeamManager.IsAlly(Team, receiver)) return;

        currentHits += 1;

        if (pierce >= 0 && currentHits > pierce)
        {
            Despawn();
        }

        bool damageRejected = DealDamage(receiver);

        if (damageRejected)
        {
            ApplyKnockback(receiver, Direction);
        }

        OnHit(receiver);
    }
}