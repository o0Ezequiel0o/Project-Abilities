using UnityEngine;
using Zeke.TeamSystem;

public class BombItemBomb : Bomb
{
    protected override void Hit(Collider2D hit)
    {
        if (TeamManager.IsAlly(team, hit.gameObject)) return;

        bool damageRejected = false;

        Vector2 direction = (hit.transform.position - transform.position).normalized;

        if (hit.gameObject.TryGetComponent(out Damageable damageable))
        {
            DamageInfo damageInfo = new DamageInfo(damage, armorPenetration, procCoefficient)
            {
                direction = direction,
                hit = true
            };

            damageRejected = damageable.DealDamage(damageInfo, source, gameObject).damageRejected;
        }

        if (!damageRejected)
        {
            ApplyKnockback(hit.gameObject, direction);
        }
    }
}