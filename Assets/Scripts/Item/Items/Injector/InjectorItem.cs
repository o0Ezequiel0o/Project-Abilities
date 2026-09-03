using UnityEngine;
using Zeke.Abilities;

namespace Zeke.Items
{
    public class InjectorItem : Item
    {
        public override ItemData Data => data;
        private readonly InjectorItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float flatModifier = 0f;

        public InjectorItem(InjectorItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out AbilityController abilityController))
            {
                abilityController.Abilities[data.AbilityType].RechargeSpeed.ApplyFlatModifier(-flatModifier);
            }
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateChargeSpeedValue();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateChargeSpeedValue();
        }

        private void UpdateChargeSpeedValue()
        {
            if (source.TryGetComponent(out AbilityController abilityController))
            {
                float oldFlatModifier = flatModifier;
                flatModifier = data.ExtraChargeSpeed.GetValue(stacks);

                abilityController.Abilities[data.AbilityType].RechargeSpeed.ApplyFlatModifier(-oldFlatModifier, flatModifier);
            }
        }
    }
}