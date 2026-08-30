using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class BasicAttack : AbilityModule
    {
        private readonly BasicAttackData data;

        private readonly Stat damage;
        private readonly OverlapShape shape;

        private AbilityController controller;
        private GameObject source;
        private Transform spawn;

        public BasicAttack(BasicAttackData data, Stat damage, OverlapShape shape)
        {
            this.data = data;
            this.damage = damage;
            this.shape = shape;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.controller = controller;
            this.source = source;
            this.spawn = spawn;
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            PerformAttack(damage.Value);
        }

        protected virtual void OnDamageDealt(GameObject source, AbilityController controller, Collider2D hit) { }

        private void PerformAttack(float damage)
        {
            Vector2 position;

            if (!data.CastAtSourceCenter)
            {
                position = spawn.position + (data.CastOffset * spawn.up);
            }
            else
            {
                position = source.transform.position + (data.CastOffset * spawn.up);
            }

            List<Collider2D> hits = shape.GetHits(position, spawn.rotation.eulerAngles.z, data.HitLayers);

            for (int i = 0; i < hits.Count; i++)
            {
                if (TryDealDamage(source, hits[i], damage))
                {
                    OnDamageDealt(source, controller, hits[i]);
                    ApplyKnockBack(hits[i], spawn.up);
                }
            }
        }

        private bool TryDealDamage(GameObject source, Collider2D target, float damage)
        {
            if (TeamManager.IsEnemy(source, target.gameObject))
            {
                if (target.TryGetComponent(out Damageable damageable))
                {
                    DamageInfo damageInfo = new DamageInfo(damage, data.ArmorPenetration, data.ProcCoefficient)
                    {
                        direction = (damageable.transform.position - source.transform.position).normalized
                    };

                    Damageable.DamageEvent damageEvent = damageable.DealDamage(damageInfo, source, source);

                    if (damageEvent.DamageDealt > 0f && !damageEvent.damageRejected)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ApplyKnockBack(Collider2D target, Vector2 direction)
        {
            if (target.TryGetComponent(out Physics physics))
            {
                physics.AddForce(data.Knockback, direction);
            }
        }

        public override void Upgrade()
        {
            damage.Upgrade();
        }
    }
}