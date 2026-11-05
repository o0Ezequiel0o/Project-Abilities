using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Items
{
    public class DrillItem : Item
    {
        public override ItemData Data => data;
        private readonly DrillItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private readonly Dictionary<Damageable, float> increasedDamage = new Dictionary<Damageable, float>();

        public DrillItem(DrillItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnHit.Subscribe(OnHit, data.ProcPriority);
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

            if (increasedDamage.ContainsKey(damageEvent.Receiver))
            {
                float flatMultDamage = data.FlatMultDamage.GetValue(stacks);
                float maxFlatMult = data.MaxStacks.GetValue(stacks) * flatMultDamage;

                float newValue = increasedDamage[damageEvent.Receiver] + flatMultDamage;

                increasedDamage[damageEvent.Receiver] = Mathf.Min(maxFlatMult, newValue);
            }
            else
            {
                increasedDamage.Add(damageEvent.Receiver, data.FlatMultDamage.GetValue(stacks));
                damageEvent.Receiver.onDespawn += OnDamageableDeath;
            }

            damageEvent.Multiplier.ApplyFlatModifier(increasedDamage[damageEvent.Receiver]);
        }

        private void OnDamageableDeath(Damageable damageable)
        {
            increasedDamage.Remove(damageable);
        }
    }
}