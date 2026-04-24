using System.Collections.Generic;
using UnityEngine;
using static Damageable;

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
            DamageEvent.onHit.Subscribe(source, OnHit, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            DamageEvent.onHit.Unsubscribe(source, OnHit);
        }

        private void OnHit(DamageEvent damageEvent)
        {
            if (damageEvent.Receiver.gameObject == source) return;

            if (increasedDamage.ContainsKey(damageEvent.Receiver))
            {
                increasedDamage[damageEvent.Receiver] += data.FlatMultDamage.GetValue(stacks);
            }
            else
            {
                increasedDamage.Add(damageEvent.Receiver, data.FlatMultDamage.GetValue(stacks));
                damageEvent.Receiver.onDeath.Subscribe(OnDamageableDeath);
            }

            damageEvent.Multiplier.ApplyFlatModifier(increasedDamage[damageEvent.Receiver]);
        }

        private void OnDamageableDeath(DamageEvent damageEvent)
        {
            increasedDamage.Remove(damageEvent.Receiver);
        }
    }
}