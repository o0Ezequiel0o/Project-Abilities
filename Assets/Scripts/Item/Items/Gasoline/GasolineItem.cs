using System.Collections.Generic;
using UnityEngine;

public class GasolineItem : Item
{
    //Template
    public override ItemData Data => data;
    private GasolineItemData data;

    private ItemHandler itemHandler;
    private GameObject source;

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

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.Hitlayers };
        Physics2D.OverlapCircle(source.transform.position, Radius, contactFilter, hits);

        for (int i = 0; i < hits.Count; ++i)
        {
            if (TeamManager.IsAlly(source, hits[i].gameObject)) continue;

            if (hits[i].TryGetComponent(out StatusEffectHandler statusEffectHandler))
            {
                statusEffectHandler.ApplyEffect(data.StatusEffectToApply, source);
            }
        }
    }
}