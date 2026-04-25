using UnityEngine;

namespace Zeke.Items
{
    public class RiotShieldItem : Item
    {
        public override ItemData Data => data;
        private readonly RiotShieldItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float regenFlatModifier = 0;
        private float shieldFlatModifier = 0;

        public RiotShieldItem(RiotShieldItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnStacksAdded(int amount)
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                UpdateShieldRegenValue(damageable);
                UpdateShieldValue(damageable);
            }
        }

        public override void OnStacksRemoved(int amount)
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                UpdateShieldRegenValue(damageable);
                UpdateShieldValue(damageable);
            }
        }

        private void UpdateShieldRegenValue(Damageable damageable)
        {
            float oldFlatModifier = regenFlatModifier;
            regenFlatModifier = data.ExtraShieldRegen.GetValue(stacks);
            UpdateDamageableStat(damageable, oldFlatModifier, regenFlatModifier);
        }

        private void UpdateShieldValue(Damageable damageable)
        {
            float oldFlatModifier = shieldFlatModifier;
            shieldFlatModifier = data.ExtraShield.GetValue(stacks);
            UpdateDamageableStat(damageable, oldFlatModifier, shieldFlatModifier);
        }

        private void UpdateDamageableStat(Damageable damageable, float old, float @new)
        {
            damageable.MaxShield.ApplyFlatModifier(-old);
            damageable.MaxShield.ApplyFlatModifier(@new);
        }
    }
}