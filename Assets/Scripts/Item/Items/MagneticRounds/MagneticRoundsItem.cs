using UnityEngine;

namespace Zeke.Items
{
    public class MagneticRoundsItem : Item
    {
        public override ItemData Data => data;
        private readonly MagneticRoundsItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public MagneticRoundsItem(MagneticRoundsItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnDealDamage.Subscribe(OnDealDamage, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnDealDamage.Unsubscribe(OnDealDamage);
            }
        }

        private void OnDealDamage(Damageable.DamageEvent.OnDealDamageEvent dealDamageEvent)
        {
            Damageable.DamageEvent damageEvent = dealDamageEvent.damageEvent;

            if (damageEvent.Receiver.gameObject == source) return;

            if (damageEvent.Receiver.Shield > 0)
            {
                damageEvent.Multiplier.Multiply(data.DamageMultiplier.GetValue(stacks));
            }
        }
    }
}