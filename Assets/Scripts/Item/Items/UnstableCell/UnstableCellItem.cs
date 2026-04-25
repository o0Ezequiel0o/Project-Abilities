using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;
using static Damageable;

namespace Zeke.Items
{
    public class UnstableCellItem : Item
    {
        public override ItemData Data => data;
        private readonly UnstableCellItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private float flatModifier = 0f;
        private float timer = 0f;
        
        private readonly List<Collider2D> hits = new List<Collider2D>();

        public UnstableCellItem(UnstableCellItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                damageable.onTakenDamage.Subscribe(OnTakenDamage, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                damageable.onTakenDamage.Unsubscribe(OnTakenDamage);
            }
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateShieldValue();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateShieldValue();
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;
        }

        private void UpdateShieldValue()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                float oldFlatModifier = flatModifier;
                flatModifier = data.ExtraShield.GetValue(stacks);

                damageable.MaxShield.ApplyFlatModifier(-oldFlatModifier);
                damageable.MaxShield.ApplyFlatModifier(flatModifier);
            }
        }

        private void OnTakenDamage(DamageEvent damageEvent)
        {
            if (timer < data.ExplosionCooldown.GetValue(stacks)) return;
            if (!damageEvent.DestroyedShield || damageEvent.Receiver.Shield > 0) return;

            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(source.transform.position, data.ExplosionRadius, contactFilter, hits);

            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].gameObject == source) continue;
                if (TeamManager.IsAlly(source, hits[i].gameObject)) continue;

                if (!TargetAwareness.HasLineOfSight(source.transform.position, hits[i], data.BlockLayers)) continue;

                if (hits[i].TryGetComponent(out Physics physics))
                {
                    Vector2 direction = (hits[i].transform.position - source.transform.position).normalized;
                    physics.AddForce(data.ExplosionKnockback, direction);
                }
            }

            timer = 0f;
        }
    }
}