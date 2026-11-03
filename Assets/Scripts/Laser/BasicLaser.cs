using UnityEngine;
using Zeke.TeamSystem;

public class FireBasicLaser : Laser
{
    protected override void OnCollision(GameObject hit)
    {
        if (TeamManager.IsAlly(source, hit)) return;

        if (hit.TryGetComponent(out Damageable damageable))
        {
            DamageInfo damageInfo = new DamageInfo(damage, armorPenetration, procCoefficient)
            {
                direction = (hit.transform.position - source.transform.position).normalized
            };

            damageable.DealDamage(damageInfo, source, gameObject);
        }
    }
}