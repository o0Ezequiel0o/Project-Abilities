using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

public class DamageAreaEffect : AreaEffect
{
    protected float damage = 0f;
    protected float armorPenetration = 0f;
    protected float procCoefficient = 0f;

    protected GameObject source;
    protected Teams team;

    public void CreateAreaEffect(int ticks, float tickInterval, float radius, DamageData damageData, GameObject source, Teams team)
    {
        damage = damageData.damage;
        armorPenetration = damageData.armorPenetration;
        procCoefficient = damageData.procCoefficient;

        this.source = source;
        this.team = team;

        CreateAreaEffect(ticks, tickInterval, radius);
        transform.localScale = 2f * radius * Vector3.one;
    }

    protected override void OnTick(List<Collider2D> hits, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (hits[i].TryGetComponent(out Damageable damageable))
            {
                DamageInfo damageInfo = new DamageInfo(damage, armorPenetration, procCoefficient)
                {
                    hit = false,
                };

                damageable.DealDamage(damageInfo, source, gameObject);
            }
        }
    }
}