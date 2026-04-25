using UnityEngine;

namespace Zeke.Items
{
    public class GreenGemItem : Item
    {
        public override ItemData Data => data;
        private readonly GreenGemItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float regenFlatModifier = 0;
        private float healthFlatModifier = 0;

        public GreenGemItem(GreenGemItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnStacksAdded(int amount)
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                UpdateHealthRegenValue(damageable);
                UpdateHealthValue(damageable);
            }
        }

        public override void OnStacksRemoved(int amount)
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                UpdateHealthRegenValue(damageable);
                UpdateHealthValue(damageable);
            }
        }

        private void UpdateHealthRegenValue(Damageable damageable)
        {
            float oldFlatModifier = regenFlatModifier;
            regenFlatModifier = data.ExtraHealthRegen.GetValue(stacks);
            damageable.HealthRegen.ApplyFlatModifier(-oldFlatModifier, regenFlatModifier);
        }

        private void UpdateHealthValue(Damageable damageable)
        {
            float oldFlatModifier = healthFlatModifier;
            healthFlatModifier = data.ExtraHealth.GetValue(stacks);
            damageable.MaxHealth.ApplyFlatModifier(-oldFlatModifier, healthFlatModifier);
        }
    }
}