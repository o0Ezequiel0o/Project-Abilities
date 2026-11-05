using UnityEngine;

namespace Zeke.Items
{
    public class MagnifyingGlassItem : Item
    {
        public override ItemData Data => data;
        private readonly MagnifyingGlassItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public MagnifyingGlassItem(MagnifyingGlassItemData data, ItemHandler itemHandler, GameObject source)
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
            
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            if (Vector3.Distance(source.transform.position, damageEvent.Receiver.transform.position) <= data.MinDistance)
            {
                damageEvent.Multiplier.Multiply(data.DamageMult.GetValue(stacks));
            }
        }
    }
}