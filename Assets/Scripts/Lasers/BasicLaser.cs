using UnityEngine;
using Zeke.TeamSystem;

public class FireBasicLaser : Laser
{
    protected override void OnCollision(GameObject hit)
    {
        if (TeamManager.IsAlly(source, hit)) return;

        if (hit.TryGetComponent(out Damageable damageable))
        {
            damageable.DealDamage(new DamageInfo(damage, armorPenetration, procCoefficient), source, gameObject);
        }
    }
}