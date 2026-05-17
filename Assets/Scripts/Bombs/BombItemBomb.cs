using UnityEngine;
using Zeke.TeamSystem;
using static Damageable;

public class BombItemBomb : Bomb
{
    protected override void Hit(Collider2D hit)
    {
        if (TeamManager.IsAlly(team, hit.gameObject)) return;

        Vector2 direction = (hit.transform.position - transform.position).normalized;

        if (hit.gameObject.TryGetComponent(out Damageable damageable))
        {
            DamageInfo damageInfo = new DamageInfo(damage, armorPenetration, procCoefficient)
            {
                direction = direction,
                hit = true
            };

            damageable.DealDamage(damageInfo, source, gameObject, OnDamageDealt);
        }
    }

    private void OnDamageDealt(DamageEvent damageEvent)
    {
        if (!damageEvent.damageRejected)
        {
            ApplyKnockback(damageEvent.Receiver.gameObject, damageEvent.Direction);
        }
    }
}