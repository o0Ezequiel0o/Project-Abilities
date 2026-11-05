using UnityEngine;

namespace Zeke.Items
{
    public class MonsterFangsItem : Item
    {
        public override ItemData Data => data;
        private readonly MonsterFangsItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;

        private float healthFlatModifier = 0f;
        private int effectStacks = 0;

        private bool hasRequiredComponents = false;

        public MonsterFangsItem(MonsterFangsItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            hasRequiredComponents = source.TryGetComponent(out damageable);

            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnKill.Subscribe(OnKill, data.ProcPriority);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnKill.Unsubscribe(OnKill);
            }
        }

        public override void OnStacksAdded(int amount)
        {
            if (!hasRequiredComponents) return;
            UpdateHealthValue();
        }

        public override void OnStacksRemoved(int amount)
        {
            if (!hasRequiredComponents) return;
            effectStacks = Mathf.FloorToInt(Mathf.Min(effectStacks, data.MaxStacks.GetValue(stacks)));
            UpdateHealthValue();
        }

        private void OnKill(Damageable.DamageEvent.OnKillEvent killEvent)
        {
            Damageable.DamageEvent damageEvent = killEvent.damageEvent;

            if (!hasRequiredComponents) return;
            if (effectStacks >= data.MaxStacks.GetValue(stacks)) return;
            if (damageEvent.Receiver != null && damageEvent.Receiver == source) return;

            effectStacks += 1;
            UpdateHealthValue();
        }

        private void UpdateHealthValue()
        {
            float oldFlatModifier = healthFlatModifier;
            healthFlatModifier = data.ExtraHealth.GetValue(stacks) * effectStacks;
            damageable.MaxHealth.ApplyFlatModifier(-oldFlatModifier, healthFlatModifier);
        }
    }
}