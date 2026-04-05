using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

public class GasolineItem : Item
{
    public override ItemData Data => data;
    private readonly GasolineItemData data;

    private readonly ItemHandler itemHandler;
    private readonly GameObject source;

    private float Radius => data.Radius.CalculateValue(stacks);

    private readonly List<Collider2D> hits = new List<Collider2D>();

    public GasolineItem(GasolineItemData data, ItemHandler itemHandler, GameObject source)
    {
        this.data = data;
        this.source = source;
        this.itemHandler = itemHandler;
    }

    public override void OnKill(Damageable.DamageEvent damageEvent) 
    {
        hits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.Hitlayers, useLayerMask = true };
        Physics2D.OverlapCircle(damageEvent.Receiver.transform.position, Radius, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            if (hits[i].transform == source.transform) continue;
            if (TeamManager.IsAlly(source, hits[i].gameObject)) continue;

            if (hits[i].TryGetComponent(out StatusEffectHandler statusEffectHandler))
            {
                statusEffectHandler.ApplyEffect(data.StatusEffectToApply, source);
            }
        }
    }
}