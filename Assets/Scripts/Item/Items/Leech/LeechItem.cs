using System.Collections.Generic;
using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class LeechItem : Item
    {
        public override ItemData Data => data;
        private readonly LeechItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;

        private bool hasRequiredComponents = false;

        public LeechItem(LeechItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            DamageEvent.onDamageDealt.Subscribe(source, OnDamageDealt, data.TriggerOrder);
            hasRequiredComponents = source.TryGetComponent(out damageable);
        }

        public override void OnRemoved()
        {
            DamageEvent.onDamageDealt.Unsubscribe(source, OnDamageDealt);
        }

        private void OnDamageDealt(DamageEvent damageEvent)
        {
            if (!hasRequiredComponents) return;

            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;
            if (damageEvent.ProcChainBranch.Contains(Data)) return;

            List<ItemData> newProcChainBranch = new List<ItemData>(damageEvent.ProcChainBranch) { Data };
            float healing = damageEvent.Damage * data.DamageHealRatio.GetValue(stacks);

            damageable.GiveHealing(healing, source, source, newProcChainBranch);
        }
    }
}