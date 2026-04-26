using UnityEngine;
using static Damageable;

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
            DamageEvent.onHit.Subscribe(source, OnHit, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            DamageEvent.onHit.Unsubscribe(source, OnHit);
        }

        private void OnHit(DamageEvent damageEvent)
        { 
            if (damageEvent.Receiver.gameObject == source) return;

            if (damageEvent.Receiver.TryGetComponent(out Physics physics))
            {
                float damageRatio = damageEvent.Damage / damageEvent.Receiver.MaxHealth.Value;
                physics.AddForce(damageEvent.Direction * (data.Knockback.GetValue(stacks) * damageRatio));
            }
        }
    }
}