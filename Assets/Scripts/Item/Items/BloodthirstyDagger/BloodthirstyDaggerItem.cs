using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class BloodthirstyDaggerItem : Item
    {
        public override ItemData Data => data;
        private readonly BloodthirstyDaggerItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public BloodthirstyDaggerItem(BloodthirstyDaggerItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            DamageEvent.onDealDamage.Subscribe(source, OnDealDamage, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            DamageEvent.onDealDamage.Unsubscribe(source, OnDealDamage);
        }

        private void OnDealDamage(DamageEvent damageEvent)
        {
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            float healthRatio = damageEvent.Receiver.Health / damageEvent.Receiver.MaxHealth.Value;

            if (healthRatio <= data.HealthRatioRequired)
            {
                damageEvent.Multiplier.Multiply(data.DamageMultiplier.GetValue(stacks));
            }
        }
    }
}