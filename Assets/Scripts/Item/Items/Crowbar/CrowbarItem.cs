using UnityEngine;

namespace Zeke.Items
{
    public class CrowbarItem : Item
    {
        public override ItemData Data => data;
        private readonly CrowbarItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public CrowbarItem(CrowbarItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            Damageable.DamageEvent.onDealDamage.Subscribe(source, OnDealDamage, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            Damageable.DamageEvent.onDealDamage.Unsubscribe(source, OnDealDamage);
        }

        private void OnDealDamage(Damageable.DamageEvent damageEvent)
        {
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            if (damageEvent.Receiver.TryGetComponent(out Damageable damageable))
            {
                float healthRatio = damageable.Health / damageable.MaxHealth.Value;

                if (healthRatio >= data.HealthThreshold)
                {
                    damageEvent.Multiplier.Multiply(data.DamageMult.GetValue(stacks));
                }
            }
        }
    }
}