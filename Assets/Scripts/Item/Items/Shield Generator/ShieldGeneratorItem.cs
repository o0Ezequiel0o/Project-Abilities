using UnityEngine;

namespace Zeke.Items
{
    public class ShieldGeneratorItem : Item
    {
        public override ItemData Data => data;
        private readonly ShieldGeneratorItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float flatModifier = 0f;

        public ShieldGeneratorItem(ShieldGeneratorItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize() { }

        public override void OnRemoved() { }

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
                flatModifier = data.ExtraShieldGeneration.GetValue(stacks);

                damageable.MaxHealth.ApplyFlatModifier(-oldFlatModifier);
                damageable.MaxHealth.ApplyFlatModifier(flatModifier);
            }
        }
    }
}