using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    public class TitanAreaDamage : AbilityModule
    {
        private readonly TitanAreaDamageData data;

        private readonly Stat radius;
        private readonly Stat damage;

        private readonly Stat healthPercentageDamageBoost;

        private GameObject source;
        private Damageable sourceDamageable;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public TitanAreaDamage(TitanAreaDamageData data, Stat radius, Stat damage, Stat healthPercentageDamageBoost)
        {
            this.data = data;
            this.radius = radius;
            this.damage = damage;

            this.healthPercentageDamageBoost = healthPercentageDamageBoost;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;
            sourceDamageable = source.GetComponent<Damageable>();
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(source.transform.position, radius.Value, contactFilter, hits);

            for (int i = 0; i < hits.Count; i++)
            {
                bool damageRejected = false;

                if (hits[i].gameObject == source) continue;

                if (TeamManager.IsAlly(hits[i].gameObject, source)) continue;

                if (hits[i].TryGetComponent(out Damageable damageable))
                {
                    float totalDamage = damage.Value;

                    if (sourceDamageable != null)
                    {
                        totalDamage += sourceDamageable.MaxHealth.Value * healthPercentageDamageBoost.Value;
                    }

                    damageRejected = damageable.DealDamage(new DamageInfo(totalDamage, data.ArmorPenetration, data.ProcCoefficient), source, source).damageRejected;
                }

                if (damageRejected) continue;

                Vector3 knockBackDirection = (hits[i].transform.position - source.transform.position).normalized;

                if (hits[i].TryGetComponent(out Physics physics))
                {
                    physics.AddForce(data.Knockback * knockBackDirection);
                }
            }
        }

        public override void Upgrade()
        {
            radius.Upgrade();
            damage.Upgrade();
        }
    }
}