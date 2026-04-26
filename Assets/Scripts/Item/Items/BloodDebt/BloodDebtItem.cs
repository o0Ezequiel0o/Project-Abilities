using System.Collections.Generic;
using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class BloodDebtItem : Item
    {
        public override ItemData Data => data;
        private readonly BloodDebtItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;

        public BloodDebtItem(BloodDebtItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out damageable))
            {
                damageable.onTakenDamage.Subscribe(OnTakenDamage, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out damageable))
            {
                damageable.onTakenDamage.Unsubscribe(OnTakenDamage);
            }
        }

        private void OnTakenDamage(DamageEvent damageEvent)
        {
            if (damageEvent.SourceUser != null & damageEvent.SourceUser == source) return;
            if (damageEvent.ProcChainBranch.Contains(data)) return;

            List<ItemData> newProcChainBranch = new List<ItemData>(damageEvent.ProcChainBranch) { Data };
            damageable.GiveHealing(data.Healing.GetValue(stacks), source, source, newProcChainBranch);
        }
    }
}