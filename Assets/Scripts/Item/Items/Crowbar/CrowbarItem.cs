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
            if (source.TryGetComponent(out DamageableEventListener eventListener))
            {
                eventListener.OnDealDamage.Subscribe(OnDealDamage, data.ProcPriority);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out DamageableEventListener eventListener))
            {
                eventListener.OnDealDamage.Unsubscribe(OnDealDamage);
            }
        }

        private void OnDealDamage(Damageable.DamageEvent.OnDealDamageEvent dealDamageEvent)
        {
            Damageable.DamageEvent damageEvent = dealDamageEvent.damageEvent;

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