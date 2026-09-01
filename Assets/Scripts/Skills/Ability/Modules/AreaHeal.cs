using System;
using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class AreaHeal : Heal
    {
        private readonly AreaHealData data;

        private readonly Stat radius;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public AreaHeal(AreaHealData data, Stat amount, Stat radius) : base(data, amount)
        {
            this.data = data;
            this.radius = radius;
        }

        public override void Activate(bool holding)
        {
            base.Activate(holding);

            hits.Clear();
            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(source.transform.position, radius.Value, contactFilter, hits);

            for (int i = 0; i < hits.Count; i++)
            {
                if (TeamManager.IsEnemy(source, hits[i].gameObject)) continue;

                if (hits[i].TryGetComponent(out Damageable damageable))
                {
                    HealInfo heal = new HealInfo(amount.Value, data.ProcCoefficient);
                    damageable.GiveHealing(heal, source, source);
                }
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();
            radius.Upgrade();
        }
    }
}