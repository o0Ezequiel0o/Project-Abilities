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

        public override void OnAdded()
        {
            Damageable.DamageEvent.onDealDamage.Subscribe(source, OnDealDamage, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            Damageable.DamageEvent.onDealDamage.Unsubscribe(source, OnDealDamage);
        }

        private void OnDealDamage(Damageable.DamageEvent damageEvent)
        {
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            if (Vector3.Distance(source.transform.position, damageEvent.Receiver.transform.position) <= data.MinDistance)
            {
                damageEvent.damageMultiplier *= data.DamageMult.GetValue(stacks);
            }
        }
    }
}