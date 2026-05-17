using UnityEngine;
using Zeke.TeamSystem;
using static Damageable;

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
        DealDamage(receiver, OnDamageDealt);
    }

    protected virtual void OnDamageDealt(DamageEvent damageEvent)
    {
        if (!damageEvent.damageRejected)
        {
            ApplyKnockback(damageEvent.Receiver.gameObject, damageEvent.Direction);
        }
    }
}