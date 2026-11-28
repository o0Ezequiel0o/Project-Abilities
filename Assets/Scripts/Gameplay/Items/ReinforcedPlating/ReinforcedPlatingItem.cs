using UnityEngine;

namespace Zeke.Items
{
    public class ReinforcedPlatingItem : Item
    {
        public override ItemData Data => data;
        private readonly ReinforcedPlatingItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float flatModifier = 0f;

        public ReinforcedPlatingItem(ReinforcedPlatingItemData data, ItemHandler itemHandler, GameObject source)
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
                flatModifier = data.ExtraArmor.GetValue(stacks);

                damageable.Armor.ApplyFlatModifier(-oldFlatModifier);
                damageable.Armor.ApplyFlatModifier(flatModifier);
            }
        }
    }
}