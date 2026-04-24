using UnityEngine;
using static Damageable;

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
            DamageEvent.onDealDamage.Subscribe(source, OnDealDamage, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            DamageEvent.onDealDamage.Unsubscribe(source, OnDealDamage);
        }

        private void OnDealDamage(DamageEvent damageEvent)
        {
            if (damageEvent.Receiver.gameObject == source) return;

            if (damageEvent.Receiver.Shield > 0)
            {
                damageEvent.Multiplier.Multiply(data.DamageMultiplier.GetValue(stacks));
            }
        }
    }
}