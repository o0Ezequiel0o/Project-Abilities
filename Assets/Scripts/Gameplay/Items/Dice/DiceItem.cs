using UnityEngine;

namespace Zeke.Items
{
    public class DiceItem : Item
    {
        public override ItemData Data => data;
        private readonly DiceItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float flatModifier = 0f;

        public DiceItem(DiceItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnRemoved()
        {
            itemHandler.Luck.ApplyFlatModifier(-flatModifier);
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateLuckValue();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateLuckValue();
        }

        private void UpdateLuckValue()
        {
            float oldFlatModifier = flatModifier;
            flatModifier = data.ExtraLuck.GetValue(stacks);

            itemHandler.Luck.ApplyFlatModifier(-oldFlatModifier, flatModifier);
        }
    }
}