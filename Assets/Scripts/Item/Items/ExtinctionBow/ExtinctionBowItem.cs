using UnityEngine;

namespace Zeke.Items
{
    public class ExtinctionBowItem : Item
    {
        public override ItemData Data => data;
        private readonly ExtinctionBowItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public ExtinctionBowItem(ExtinctionBowItemData data, ItemHandler itemHandler, GameObject source)
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

            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            EntityType entityType = EntityTypeIdentifier.GetEntityType(damageEvent.Receiver.gameObject);

            if (data.Types.Contains(entityType))
            {
                damageEvent.Multiplier.Multiply(data.DamageMultiplier.GetValue(stacks));
            }
        }
    }
}