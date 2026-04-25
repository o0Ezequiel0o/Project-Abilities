using UnityEngine;

namespace Zeke.Items
{
    public class ElectricHeartItem : Item
    {
        public override ItemData Data => data;
        private readonly ElectricHeartItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;
        private float regenFlatModifier = 0f;

        private bool hasRequiredComponents = false;

        public ElectricHeartItem(ElectricHeartItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            hasRequiredComponents = source.TryGetComponent(out damageable);
        }

        public override void OnRemoved()
        {
            if (!hasRequiredComponents) return;
            damageable.HealthRegen.ApplyFlatModifier(-regenFlatModifier);
        }

        public override void OnUpdate()
        {
            if (!hasRequiredComponents) return;
            UpdateRegenValue();
        }

        private void UpdateRegenValue()
        {
            float healthShieldRatio = damageable.MaxShield.Value / damageable.MaxHealth.Value;
            float healthRegen = data.HealthRegen.GetValue(stacks) * healthShieldRatio;

            if (damageable.Shield < damageable.MaxShield.Value)
            {
                healthRegen = 0f;
            }

            float oldFlatModifier = regenFlatModifier;
            regenFlatModifier = healthRegen;

            damageable.HealthRegen.ApplyFlatModifier(-oldFlatModifier, regenFlatModifier);
        }
    }
}