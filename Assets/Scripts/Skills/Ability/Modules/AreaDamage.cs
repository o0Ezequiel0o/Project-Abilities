using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.TeamSystem;
using static Damageable;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class AreaDamage : AbilityModule
    {
        [SerializeField] private Stat radius;
        [SerializeField] private Stat damage;

        [SerializeField] private float knockback;
        [SerializeField] private LayerMask hitLayers;

        private GameObject source;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public AreaDamage() { }

        public AreaDamage(AreaDamage original)
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

        public override AbilityModule DeepCopy() => new AreaDamage(this);

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(source.transform.position, radius.Value, contactFilter, hits);

            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].gameObject == source) continue;

                if (TeamManager.IsAlly(hits[i].gameObject, source)) continue;

                if (hits[i].TryGetComponent(out Damageable damageable))
                {
                    Vector3 direction = (damageable.transform.position - source.transform.position).normalized;
                    DamageInfo damageInfo = new DamageInfo(damage.Value, 0f, 1f) { direction = direction };
                    damageable.DealDamage(damageInfo, source, source, OnDamageDealt);
                }
            }
        }

        private void OnDamageDealt(DamageEvent damageEvent)
        {
            if (damageEvent.damageRejected) return;

            GameObject receiver = damageEvent.Receiver.gameObject;

            ApplyKnockback(receiver, damageEvent.Direction);
        }

        private void ApplyKnockback(GameObject receiver, Vector2 direction)
        {
            if (receiver.TryGetComponent(out Physics physics))
            {
                physics.AddForce(knockback * direction);
            }
        }

        public override void Upgrade()
        {
            radius.Upgrade();
            damage.Upgrade();
        }
    }
}