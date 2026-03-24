using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class BoxAttack : AbilityModule
    {
        [Header("Local Data")]
        [SerializeField] private Stat damage;

        [Header("Should Be Global Data")]
        [SerializeField] private float knockback; //make data
        [SerializeField] private Vector2 range; //make data
        [SerializeField] private LayerMask hitLayers; //make data

        private AbilityController controller;
        private GameObject source;
        private Transform spawn;

        public BoxAttack(BoxAttack original)
        {
            damage = original.damage.DeepCopy();
            knockback = original.knockback;

            range = original.range;
            hitLayers = original.hitLayers;
        }

        public override AbilityModule DeepCopy() => new BoxAttack(this);

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
            Vector3 distanceFromCenter = 0.5f * range.y * spawn.up;
            Vector2 position = spawn.position + distanceFromCenter;

            Collider2D[] hits = Physics2D.OverlapBoxAll(position, range, hitLayers);

            for (int i = 0; i < hits.Length; i++)
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