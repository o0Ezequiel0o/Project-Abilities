using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class BasicAttack : AbilityModule
    {
        [SerializeField] private Stat damage;
        [SerializeField] private float knockback;

        [Space]

        [SerializeReferenceDropdown, SerializeReference] private OverlapShape shape;
        [SerializeField] private LayerMask hitLayers;
        [SerializeField] private float castOffset;
        [SerializeField] private bool castAtSourceCenter;

        private AbilityController controller;
        private GameObject source;
        private Transform spawn;

        public BasicAttack() { }

        public BasicAttack(BasicAttack original)
        {
            castOffset = original.castOffset;

            knockback = original.knockback;
            hitLayers = original.hitLayers;
            castAtSourceCenter = original.castAtSourceCenter;

            damage = original.damage.DeepCopy();
            shape = original.shape.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new BasicAttack(this);

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

            if (!castAtSourceCenter)
            {
                position = spawn.position + (castOffset * spawn.up);
            }
            else
            {
                position = source.transform.position + (castOffset * spawn.up);
            }

            List<Collider2D> hits = shape.GetHits(position, spawn.rotation.eulerAngles.z + 90f, hitLayers);

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
                    Damageable.DamageEvent damageEvent = damageable.DealDamage(new DamageInfo(damage, 0f, 1f), source, source);

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
                physics.AddForce(knockback, direction);
            }
        }

        public override void Upgrade()
        {
            damage.Upgrade();
        }
    }
}