using UnityEngine;

namespace Zeke.Items
{
    public class BoxingGloveItem : Item
    {
        public override ItemData Data => data;
        private readonly BoxingGloveItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public BoxingGloveItem(BoxingGloveItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnHit.Subscribe(OnHit, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnHit.Unsubscribe(OnHit);
            }
        }

        private void OnHit(Damageable.DamageEvent.OnHitEvent hitEvent)
        {
            Damageable.DamageEvent damageEvent = hitEvent.damageEvent;

            if (damageEvent.Receiver.gameObject == source) return;

            if (damageEvent.Receiver.TryGetComponent(out Physics physics))
            {
                float damageRatio = damageEvent.Damage / damageEvent.Receiver.MaxHealth.Value;
                physics.AddForce(damageEvent.Direction * (data.Knockback.GetValue(stacks) * damageRatio));
            }
        }
    }
}