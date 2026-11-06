using UnityEngine;

namespace Zeke.Items
{
    public class MilkItem : Item
    {
        public override ItemData Data => data;
        private readonly MilkItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;
        private bool HasRequiredComponents = false;

        public MilkItem(MilkItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            HasRequiredComponents = source.TryGetComponent(out damageable);

            if (source.TryGetComponent(out DamageableEventListener eventListener))
            {
                eventListener.OnHit.Subscribe(OnHit, data.ProcPriority);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out DamageableEventListener eventListener))
            {
                eventListener.OnHit.Unsubscribe(OnHit);
            }
        }

        private void OnHit(Damageable.DamageEvent.OnHitEvent hitEvent)
        {
            Damageable.DamageEvent damageEvent = hitEvent.damageEvent;

            if (!HasRequiredComponents) return;
            if (damageEvent.Receiver.gameObject == source) return;

            float healthPercentage = damageable.Health / damageable.MaxHealth.Value;
            float damage = data.DamageFlatMult.GetValue(stacks) * healthPercentage;

            damageEvent.Multiplier.ApplyFlatModifier(damage);
        }
    }
}