using UnityEngine;

namespace Zeke.Items
{
    public class GreenLiquidItem : Item
    {
        public override ItemData Data => data;
        private readonly GreenLiquidItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float flatModifier = 0f;

        public GreenLiquidItem(GreenLiquidItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateArmorValue();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateArmorValue();
        }

        private void UpdateArmorValue()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                float oldFlatModifier = flatModifier;
                flatModifier = data.ExtraHealthRegen.GetValue(stacks);

                damageable.HealthRegen.ApplyFlatModifier(-oldFlatModifier);
                damageable.HealthRegen.ApplyFlatModifier(flatModifier);
            }
        }
    }
}