using UnityEngine;

namespace Zeke.Items
{
    public class HollowPointItem : Item
    {
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
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnDealDamage.Subscribe(OnDealDamage, data.ProcPriority);
                eventListener.OnHit.Subscribe(OnHit, data.ProcPriority);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnDealDamage.Unsubscribe(OnDealDamage);
                eventListener.OnHit.Unsubscribe(OnHit);
            }
        }

        public void OnDealDamage(Damageable.DamageEvent.OnDealDamageEvent dealDamageEvent)
        {
            Damageable.DamageEvent damageEvent = dealDamageEvent.damageEvent;

            if (damageEvent.Receiver.gameObject == source) return;
            damageEvent.Multiplier.ApplyFlatModifier(data.FlatMultDamage.GetValue(stacks));
        }

        private void OnHit(Damageable.DamageEvent.OnHitEvent hitEvent)
        {
            Damageable.DamageEvent damageEvent = hitEvent.damageEvent;

            if (damageEvent.Receiver.gameObject == source) return;
            damageEvent.FlatAccumulator += data.FlatDamage.GetValue(stacks);
        }
    }
}