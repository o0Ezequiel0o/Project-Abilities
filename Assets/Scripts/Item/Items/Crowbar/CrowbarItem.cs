using UnityEngine;

public class CrowbarItem : Item
{
    public override ItemData Data => data;
    private CrowbarItemData data;

    private ItemHandler itemHandler;
    private GameObject source;

    public CrowbarItem(CrowbarItemData data, ItemHandler itemHandler, GameObject source)
    {
        this.data = data;
        this.source = source;
        this.itemHandler = itemHandler;
    }
    
    public override void OnDealDamage(Damageable.DamageEvent damageEvent)
    {
        if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

        if (damageEvent.Receiver.TryGetComponent(out Damageable damageable))
        {
            float healthRatio = damageable.Health / damageable.MaxHealth.Value;

            if (healthRatio >= data.HealthThreshold)
            {
                damageEvent.damageMultiplier *= data.DamageMultiplier.CalculateValue(stacks);
            }
        }
    }
}