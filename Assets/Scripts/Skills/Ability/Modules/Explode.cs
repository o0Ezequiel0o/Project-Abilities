using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class Explode : AbilityModule
    {
        [SerializeField] private Stat radius;
        [SerializeField] private Stat damage;

        [SerializeField] private float knockback;
        [SerializeField] private LayerMask hitLayers;

        private GameObject source;

        public Explode(Explode original)
        {
            radius = original.radius.DeepCopy();
            damage = original.damage.DeepCopy();

            knockback = original.knockback;
            hitLayers = original.hitLayers;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;
        }

        public override AbilityModule DeepCopy() => new Explode(this);

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(source.transform.position, radius.Value, hitLayers);

            for (int i = 0; i < hits.Length; i++)
            {
                bool damageRejected = false;

                if (hits[i].gameObject == source || TeamManager.IsAlly(hits[i].gameObject, source)) continue;

                if (hits[i].TryGetComponent(out Damageable damageable))
                {
                    damageRejected = damageable.DealDamage(new DamageInfo(damage.Value, 0f, 1f), source, source).damageRejected;
                }

                if (damageRejected) continue;

                Vector3 knockBackDirection = (hits[i].transform.position - source.transform.position).normalized;

                if (hits[i].TryGetComponent(out Physics physics))
                {
                    physics.AddForce(knockback * knockBackDirection);
                }
            }

            if (source.TryGetComponent(out Damageable sourceDamageable))
            {
                sourceDamageable.DealDamage(new DamageInfo(damage.Value, 0f, 1f), source, source);
            }
        }

        public override void Upgrade()
        {
            radius.Upgrade();
            damage.Upgrade();
        }
    }
}