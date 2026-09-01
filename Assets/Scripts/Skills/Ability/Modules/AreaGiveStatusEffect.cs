using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class AreaGiveStatusEffect : GiveStatusEffect
    {
        private readonly AreaGiveStatusEffectData data;

        private readonly Stat radius;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public AreaGiveStatusEffect(AreaGiveStatusEffectData data, Stat stacks, Stat radius) : base(data, stacks)
        {
            this.data = data;
            this.radius = radius;
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (data.Targeting == TargetingType.Allies)
            {
                base.Activate(holding);
            }

            hits.Clear();
            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(source.transform.position, radius.Value, contactFilter, hits);

            for (int i = 0; i < hits.Count; i++)
            {
                if (data.Targeting == TargetingType.Allies)
                {
                    if (TeamManager.IsEnemy(source, hits[i].gameObject)) continue;
                }
                else if (data.Targeting == TargetingType.Enemies)
                {
                    if (TeamManager.IsAlly(source, hits[i].gameObject)) continue;
                }

                if (hits[i].TryGetComponent(out StatusEffectHandler statusEffectHandler))
                {
                    statusEffectHandler.ApplyEffect(data.StatusEffect, source, stacks.ValueInt);
                }
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();
            radius.Upgrade();
        }

        public enum TargetingType
        {
            Enemies,
            Allies
        }
    }
}