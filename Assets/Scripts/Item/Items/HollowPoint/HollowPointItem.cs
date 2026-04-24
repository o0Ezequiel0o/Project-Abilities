using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class HollowPointItem : Item
    {
        //Template
        public override ItemData Data => data;
        private readonly HollowPointItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public HollowPointItem(HollowPointItemData data, ItemHandler itemHandler, GameObject source)
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

        public void OnDealDamage(DamageEvent damageEvent)
        {
            if (damageEvent.Receiver.gameObject == source) return;
            damageEvent.Multiplier.ApplyFlatModifier(data.FlatMultDamage.GetValue(stacks));
        }
    }
}