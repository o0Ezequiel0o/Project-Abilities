using UnityEngine;
using static Damageable;

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
            DamageEvent.onHit.Subscribe(source, OnHit, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            DamageEvent.onHit.Unsubscribe(source, OnHit);
        }

        private void OnHit(DamageEvent damageEvent)
        {
            if (!HasRequiredComponents) return;

            float healthPercentage = damageable.Health / damageable.MaxHealth.Value;
            float damage = data.DamageFlatMult.GetValue(stacks) * healthPercentage;

            damageEvent.Multiplier.ApplyFlatModifier(damage);
        }
    }
}