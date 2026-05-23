using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

public class HealAreaEffect : AreaEffect
{
    protected float healing = 0f;
    protected float procCoefficient = 0f;

    protected GameObject source;
    protected Teams team;

    public void CreateAreaEffect(int ticks, float tickInterval, float radius, HealData healData, GameObject source, Teams team)
    {
        healing = healData.healing;
        procCoefficient = healData.procCoefficient;

        this.source = source;
        this.team = team;

        CreateAreaEffect(ticks, tickInterval, radius);
        transform.localScale = 2f * radius * Vector3.one;
    }

    protected override void OnTick(List<Collider2D> hits, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject receiver = hits[i].gameObject;

            if (receiver == source) continue;
            if (TeamManager.IsEnemy(source, receiver)) continue;

            if (receiver.TryGetComponent(out Damageable damageable))
            {
                HealInfo heal = new HealInfo(healing, procCoefficient);
                damageable.GiveHealing(heal, source, gameObject);
            }
        }
    }
}