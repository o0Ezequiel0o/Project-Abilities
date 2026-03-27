using UnityEngine;

public class BasicProjectile : DamageProjectileBase
{
    protected override void OnCollision(RaycastHit2D hit)
    {
        if (hit.collider.gameObject == SourceUser) return;

        Hit(hit.transform.gameObject);
        TeleportToHitPoint(hit.point);

        Despawn();
    }
    
    protected void Hit(GameObject receiver)
    {
        if (TeamManager.IsAlly(Team, receiver)) return;

        bool damageRejected = DealDamage(receiver);

        if (!damageRejected)
        {
            ApplyKnockback(receiver, Direction);
        }
    }
}