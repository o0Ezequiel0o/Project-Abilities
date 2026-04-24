using System;
using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;
using static Damageable;

namespace Zeke.Items
{
    public class CopperWiresItem : Item
    {
        public override ItemData Data => data;
        private readonly CopperWiresItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private readonly List<Collider2D> hits = new List<Collider2D>();
        private readonly List<Transform> procTargets = new List<Transform>();

        private Predicate<GameObject> targetFilter = null;

        public CopperWiresItem(CopperWiresItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            targetFilter = CanTarget;
            DamageEvent.onHit.Subscribe(source, OnHit, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            DamageEvent.onHit.Unsubscribe(source, OnHit);
        }
        
        private void OnHit(DamageEvent damageEvent)
        {
            if (!RollProc(data.ProcChance, damageEvent.ProcCoefficient, itemHandler.Luck)) return;
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;
            if (damageEvent.ProcChainBranch.Contains(Data)) return;

            hits.Clear();
            procTargets.Clear();

            int maxTargets = Mathf.CeilToInt(data.MaxTargets.GetValue(stacks));

            for (int i = 0; i < maxTargets; i++)
            {
                Transform target = TargetAwareness.GetClosestTargetToDirection(
                    damageEvent.Receiver.transform.position,
                    damageEvent.Direction,
                    data.Radius.GetValue(stacks),
                    data.HitLayers,
                    data.BlockLayers,
                    targetFilter
                    );

                if (target != null)
                {
                    procTargets.Add(target);
                }
                else
                {
                    break;
                }
            }

            Vector2 direction = damageEvent.Direction;

            for (int i = 0; i < procTargets.Count; i++)
            {
                if (procTargets[i].TryGetComponent(out Damageable damageable))
                {
                    if (i > 0)
                    {
                        direction = (procTargets[i].position - procTargets[i - 1].position).normalized;
                    }

                    List<ItemData> newProcChainBranch = new List<ItemData>(damageEvent.ProcChainBranch) { Data };
                    DamageInfo damage = new DamageInfo(damageEvent.BaseDamage, data.ArmorPenetration, data.ProcCoefficient)
                    {
                        hit = true,
                        direction = direction
                    };
                    damageable.DealDamage(damage, source, null, newProcChainBranch);
                }
            }
        }

        private bool CanTarget(GameObject target)
        {
            if (procTargets.Contains(target.transform))
            {
                return false;
            }

            if (TeamManager.IsAlly(source, target))
            {
                return false;
            }

            return true;
        }
    }
}