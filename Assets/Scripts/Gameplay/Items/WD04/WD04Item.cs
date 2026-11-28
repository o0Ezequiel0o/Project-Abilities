using UnityEngine;

namespace Zeke.Items
{
    public class WD04Item : Item
    {
        public override ItemData Data => data;
        private readonly WD04ItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float rotationSpeedFlatModifier = 0;
        private float moveSpeedFlatModifier = 0;

        public WD04Item(WD04ItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateValues();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateValues();
        }

        private void UpdateValues()
        {
            if (source.TryGetComponent(out EntityAim entityAim))
            {
                UpdateRotationSpeedValue(entityAim);
            }
            if (source.TryGetComponent(out EntityMove entityMove))
            {
                UpdateMoveSpeedValue(entityMove);
            }
        }

        private void UpdateRotationSpeedValue(EntityAim entityAim)
        {
            float oldFlatModifier = rotationSpeedFlatModifier;
            rotationSpeedFlatModifier = data.ExtraRotationSpeed.GetValue(stacks);
            UpdateStat(entityAim.RotationSpeed, oldFlatModifier, rotationSpeedFlatModifier);
        }

        private void UpdateMoveSpeedValue(EntityMove entityMove)
        {
            float oldFlatModifier = moveSpeedFlatModifier;
            moveSpeedFlatModifier = data.ExtraMoveSpeed.GetValue(stacks);
            UpdateStat(entityMove.MoveSpeed, oldFlatModifier, moveSpeedFlatModifier);
        }

        private void UpdateStat(Stat stat, float old, float @new)
        {
            stat.ApplyFlatModifier(-old);
            stat.ApplyFlatModifier(@new);
        }
    }
}