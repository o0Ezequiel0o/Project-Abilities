using UnityEngine;
using Zeke.TeamSystem;

public class BasicProjectile : DamageProjectileBase
{
    [SerializeField] protected bool allyCollision;
    [SerializeField] protected bool allyDamage;

    protected override void OnCollision(RaycastHit2D hit)
    {
        GameObject receiver = hit.collider.gameObject;

        if (receiver == SourceUser) return;

        if (TeamManager.IsEnemy(Team, receiver) || allyDamage)
        {
            Hit(receiver);
            TeleportToHitPoint(hit.point);
        }
        else if (!allyCollision) return;

        Despawn();
    }
    
    protected virtual void Hit(GameObject receiver)
    {
        bool damageRejected = DealDamage(receiver);

        if (!damageRejected)
        {
            ApplyKnockback(receiver, Direction);
        }
    }
}