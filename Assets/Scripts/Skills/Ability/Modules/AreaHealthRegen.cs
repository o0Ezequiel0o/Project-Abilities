using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    public class AreaHealthRegen : HealthRegen
    {
        private readonly AreaHealthRegenData data;

        private readonly Stat radius;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public AreaHealthRegen(AreaHealthRegenData data, Stat amount, Stat interval, Stat radius) : base(data, amount, interval)
        {
            this.data = data;
            this.radius = radius;
        }

        public override void OnHealthRegenTick()
        {
            base.OnHealthRegenTick();

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