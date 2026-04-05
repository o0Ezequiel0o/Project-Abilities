using UnityEngine;

public class MagnifyingGlassItem : Item
{
    public override ItemData Data => data;
    private MagnifyingGlassItemData data;

    private ItemHandler itemHandler;
    private GameObject source;

    public MagnifyingGlassItem(MagnifyingGlassItemData data, ItemHandler itemHandler, GameObject source)
    {
        this.data = data;
        this.source = source;
        this.itemHandler = itemHandler;
    }

    public override void OnDealDamage(Damageable.DamageEvent damageEvent)
    {
        if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

        if (Vector3.Distance(source.transform.position, damageEvent.Receiver.transform.position) <= data.MinDistance)
        {
            damageEvent.damageMultiplier *= data.DamageMultiplier.CalculateValue(stacks);
        }
    }
}