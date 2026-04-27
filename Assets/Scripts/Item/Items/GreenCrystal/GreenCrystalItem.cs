using UnityEngine;

namespace Zeke.Items
{
    public class GreenCrystalItem : Item
    {
        public override ItemData Data => data;
        private readonly GreenCrystalItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float flatModifier = 0f;

        public GreenCrystalItem(GreenCrystalItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                damageable.MaxHealth.ApplyFlatModifier(-flatModifier);
            }
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateHealthValue();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateHealthValue();
        }

        private void UpdateHealthValue()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                float oldFlatModifier = flatModifier;
                flatModifier = data.ExtraHealth.GetValue(stacks);

                damageable.MaxHealth.ApplyFlatModifier(-oldFlatModifier, flatModifier);
            }
        }
    }
}