using UnityEngine;
using Zeke.Abilities;
using static Damageable;

namespace Zeke.Items
{
    public class MilitaryManualItem : Item
    {
        public override ItemData Data => data;
        private readonly MilitaryManualItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float flatModifier = 0f;

        public MilitaryManualItem(MilitaryManualItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            DamageEvent.onDealDamage.Subscribe(source, OnDealDamage, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out AbilityController abilityController))
            {
                abilityController.rechargeSpeed[data.AbilityType].ApplyFlatModifier(-flatModifier);
            }

            DamageEvent.onDealDamage.Unsubscribe(source, OnDealDamage);
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

                abilityController.rechargeSpeed[data.AbilityType].ApplyFlatModifier(-oldFlatModifier, flatModifier);
            }
        }

        public void OnDealDamage(DamageEvent damageEvent)
        {
            if (damageEvent.Receiver.gameObject == source) return;
            damageEvent.Multiplier.ApplyFlatModifier(data.FlatMultDamage.GetValue(stacks));
        }
    }
}