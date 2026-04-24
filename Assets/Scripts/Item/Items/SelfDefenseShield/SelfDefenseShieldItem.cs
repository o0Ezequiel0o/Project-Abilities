using UnityEngine;

namespace Zeke.Items
{
    public class SelfDefenseShieldItem : Item
    {
        public override ItemData Data => data;
        private readonly SelfDefenseShieldItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float flatModifier = 0f;

        public SelfDefenseShieldItem(SelfDefenseShieldItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateShieldValue();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateShieldValue();
        }

        private void UpdateShieldValue()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                float oldFlatModifier = flatModifier;
                flatModifier = data.ExtraShield.GetValue(stacks);

                damageable.MaxShield.ApplyFlatModifier(-oldFlatModifier);
                damageable.MaxShield.ApplyFlatModifier(flatModifier);
            }
        }
    }
}