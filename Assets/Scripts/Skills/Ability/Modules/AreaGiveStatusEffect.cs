using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class AreaGiveStatusEffect : GiveStatusEffect
    {
        [SerializeField] private Stat radius;
        [SerializeField] private LayerMask hitLayers;
        [SerializeField] private TargetingType targeting;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public AreaGiveStatusEffect() { }

        public AreaGiveStatusEffect(AreaGiveStatusEffect original) : base(original)
        {
            hitLayers = original.hitLayers;
            targeting = original.targeting;
            radius = original.radius.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new AreaGiveStatusEffect(this);

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (targeting == TargetingType.Allies)
            {
                base.Activate(holding);
            }

            hits.Clear();
            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayers };
            Physics2D.OverlapCircle(source.transform.position, radius.Value, contactFilter, hits);

            for (int i = 0; i < hits.Count; i++)
            {
                if (targeting == TargetingType.Allies)
                {
                    if (TeamManager.IsEnemy(source, hits[i].gameObject)) continue;
                }
                else if (targeting == TargetingType.Enemies)
                {
                    if (TeamManager.IsAlly(source, hits[i].gameObject)) continue;
                }

                if (hits[i].TryGetComponent(out StatusEffectHandler statusEffectHandler))
                {
                    statusEffectHandler.ApplyEffect(statusEffect, source, stacks.ValueInt);
                }
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();
            radius.Upgrade();
        }

        private enum TargetingType
        {
            Enemies,
            Allies
        }
    }
}