using UnityEngine;
using Zeke.Abilities;

namespace Zeke.Items
{
    public class CoffeeItem : Item
    {
        public override ItemData Data => data;
        private readonly CoffeeItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float chargeSpeedFlatModifier = 0f;
        private float moveSpeedFlatModifier = 0f;

        public CoffeeItem(CoffeeItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out AbilityController abilityController))
            {
                abilityController.rechargeSpeed[data.AbilityType].ApplyFlatModifier(-chargeSpeedFlatModifier);
            }

            if (source.TryGetComponent(out EntityMove entityMove))
            {
                entityMove.MoveSpeed.ApplyFlatModifier(-moveSpeedFlatModifier);
            }
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateChargeSpeedValue();
            UpdateMoveSpeedValue();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateChargeSpeedValue();
            UpdateMoveSpeedValue();
        }

        private void UpdateMoveSpeedValue()
        {
            if (source.TryGetComponent(out AbilityController abilityController))
            {
                float oldFlatModifier = chargeSpeedFlatModifier;
                chargeSpeedFlatModifier = data.ExtraChargeSpeed.GetValue(stacks);

                abilityController.rechargeSpeed[data.AbilityType].ApplyFlatModifier(-oldFlatModifier, chargeSpeedFlatModifier);
            }
        }

        private void UpdateChargeSpeedValue()
        {
            if (source.TryGetComponent(out EntityMove entityMove))
            {
                float oldFlatModifier = moveSpeedFlatModifier;
                moveSpeedFlatModifier = data.ExtraMoveSpeed.GetValue(stacks);

                entityMove.MoveSpeed.ApplyFlatModifier(-oldFlatModifier, moveSpeedFlatModifier);
            }
        }
    }
}