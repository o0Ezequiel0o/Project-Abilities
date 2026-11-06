using UnityEngine;
using Zeke.Abilities;

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
            if (source.TryGetComponent(out DamageableEventListener eventListener))
            {
                eventListener.OnDealDamage.Subscribe(OnDealDamage, data.ProcPriority);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out AbilityController abilityController))
            {
                abilityController.AbilityTypeSlots[data.AbilityType].RechargeSpeed.ApplyFlatModifier(-flatModifier);
            }

            if (source.TryGetComponent(out DamageableEventListener eventListener))
            {
                eventListener.OnDealDamage.Unsubscribe(OnDealDamage);
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

                abilityController.AbilityTypeSlots[data.AbilityType].RechargeSpeed.ApplyFlatModifier(-oldFlatModifier, flatModifier);
            }
        }

        public void OnDealDamage(Damageable.DamageEvent.OnDealDamageEvent dealDamageEvent)
        {
            Damageable.DamageEvent damageEvent = dealDamageEvent.damageEvent;

            if (damageEvent.Receiver.gameObject == source) return;
            damageEvent.Multiplier.ApplyFlatModifier(data.FlatMultDamage.GetValue(stacks));
        }
    }
}