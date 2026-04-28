using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;
using static Damageable;

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
            DamageEvent.onKill.Subscribe(source, OnKill, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            DamageEvent.onKill.Unsubscribe(source, OnKill);
        }

        private void OnKill(DamageEvent damageEvent)
        {
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            hits.Clear();

            float damage = Mathf.Max(data.MinDamage.GetValue(stacks), damageEvent.OverflowDamage);

            Vector3 position = damageEvent.Receiver.transform.position;

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(position, data.Radius.GetValue(stacks), contactFilter, hits);

            for (int i = 0; i < hits.Count; i++)
            {
                if (TeamManager.IsAlly(source, hits[i].gameObject)) continue;
                if (!TargetAwareness.HasLineOfSight(position, hits[i], data.BlockLayers)) continue;

                if (hits[i].TryGetComponent(out Damageable damageable))
                {
                    DamageInfo damageInfo = new DamageInfo(damage, data.ArmorPenetration, data.ProcCoefficient)
                    {
                        direction = (hits[i].transform.position - damageEvent.Receiver.transform.position).normalized
                    };
                    damageable.DealDamage(damageInfo, source, null);
                }
            }
        }
    }
}