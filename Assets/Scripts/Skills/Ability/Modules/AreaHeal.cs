using System;
using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class AreaHeal : Heal
    {
        [SerializeField] private Stat radius;
        [SerializeField] private LayerMask hitLayers;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public AreaHeal() { }

        public AreaHeal(AreaHeal original) : base (original)
        {
            hitLayers = original.hitLayers;
            radius = original.radius.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new AreaHeal(this);

        public override void Activate(bool holding)
        {
            base.Activate(holding);

            hits.Clear();
            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayers };
            Physics2D.OverlapCircle(source.transform.position, radius.Value, contactFilter, hits);

            for (int i = 0; i < hits.Count; i++)
            {
                if (TeamManager.IsEnemy(source, hits[i].gameObject)) continue;

                if (hits[i].TryGetComponent(out Damageable damageable))
                {
                    damageable.GiveHealing(amount.Value, source, source);
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