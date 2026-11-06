using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Items
{
    public class DeadMansSwitchItem : Item
    {
        public override ItemData Data => data;
        private readonly DeadMansSwitchItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public DeadMansSwitchItem(DeadMansSwitchItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out DamageableEventListener eventListener))
            {
                eventListener.OnKill.Subscribe(OnKill, data.ProcPriority);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out DamageableEventListener eventListener))
            {
                eventListener.OnKill.Unsubscribe(OnKill);
            }
        }

        private void OnKill(Damageable.DamageEvent.OnKillEvent killEvent)
        {
            Damageable.DamageEvent damageEvent = killEvent.damageEvent;

            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;
            if (damageEvent.ProcChainBranch.Contains(data)) return;

            hits.Clear();

            float damage = Mathf.Max(data.MinDamage.GetValue(stacks), damageEvent.OverflowDamage);
            List<ItemData> newProcChainBranch = new List<ItemData>(damageEvent.ProcChainBranch) { data };

            Vector3 position = damageEvent.Receiver.transform.position;

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(position, data.Radius.GetValue(stacks), contactFilter, hits);

            for (int i = 0; i < hits.Count; i++)
            {
                if (TeamManager.IsAlly(source, hits[i].gameObject)) continue;

                if (hits[i].TryGetComponent(out Damageable damageable))
                {
                    DamageInfo damageInfo = new DamageInfo(damage, data.ArmorPenetration, data.ProcCoefficient)
                    {
                        direction = (hits[i].transform.position - damageEvent.Receiver.transform.position).normalized
                    };
                    damageable.DealDamage(damageInfo, source, null, newProcChainBranch);
                }
            }
        }
    }
}