using UnityEngine;

namespace Zeke.Items
{
    public class SneakersItem : Item
    {
        public override ItemData Data => data;
        private readonly SneakersItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float flatModifier = 0f;

        public SneakersItem(SneakersItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out EntityMove entityMove))
            {
                entityMove.MoveSpeed.ApplyFlatModifier(-flatModifier);
            }
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateMoveSpeedValue();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateMoveSpeedValue();
        }

        private void UpdateMoveSpeedValue()
        {
            if (source.TryGetComponent(out EntityMove entityMove))
            {
                float oldFlatModifier = flatModifier;
                flatModifier = data.ExtraMoveSpeed.GetValue(stacks);

                entityMove.MoveSpeed.ApplyFlatModifier(-oldFlatModifier, flatModifier);
            }
        }
    }
}