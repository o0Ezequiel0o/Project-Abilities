using UnityEngine;

namespace Zeke.Items
{
    public class GreenCrystalItem : Item
    {
        public override ItemData Data => data;
        private readonly GreenCrystalItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float extraHealth = 0f;

        public GreenCrystalItem(GreenCrystalItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnAdded()
        {
            UpdateHealthValue();
        }

        public override void OnRemoved()
        {
            UpdateHealthValue();
        }

        public override void OnStackAdded()
        {
            UpdateHealthValue();
        }

        public override void OnStackRemoved()
        {
            UpdateHealthValue();
        }

        private void UpdateHealthValue()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                RemoveOldHealthModifier(damageable);
                ApplyNewHealthModifier(damageable);
            }
        }

        private void ApplyNewHealthModifier(Damageable damageable)
        {
            extraHealth = data.ExtraHealth.GetValue(stacks);
            damageable.MaxHealth.ApplyFlatModifier(extraHealth);
        }

        private void RemoveOldHealthModifier(Damageable damageable)
        {
            damageable.MaxHealth.ApplyFlatModifier(-extraHealth);
        }
    }
}