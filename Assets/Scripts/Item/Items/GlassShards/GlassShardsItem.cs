using UnityEngine;

public class GlassShardsItem : Item
{
    public override ItemData Data => data;
    private readonly GlassShardsItemData data;

    private readonly ItemHandler itemHandler;
    private readonly GameObject source;

    public GlassShardsItem(GlassShardsItemData data, ItemHandler itemHandler, GameObject source)
    {
        this.data = data;
        this.source = source;
        this.itemHandler = itemHandler;
    }

    public override void OnHit(Damageable.DamageEvent damageEvent)
    {
        if (!RollProc(data.ProcChance.CalculateValue(stacks), damageEvent.ProcCoefficient, itemHandler.Luck)) return;
        if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

        if (damageEvent.Receiver.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            statusEffectHandler.ApplyEffect(data.StatusEffect, source);
        }
    }
}