using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;
using static Damageable;

namespace Zeke.Items
{
    public class HitlistItem : Item
    {
        public override ItemData Data => data;
        private readonly HitlistItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private GameObject visualInstance;
        private Transform target;

        private float timer = 0f;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public HitlistItem(HitlistItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            DamageEvent.onDealDamage.Subscribe(source, OnDealDamage, data.TriggerOrder);
            visualInstance = GameObject.Instantiate(data.TargetVisual, GameInstance.WorldCanvas.transform);
            visualInstance.SetActive(false);
        }

        public override void OnRemoved()
        {
            DamageEvent.onDealDamage.Unsubscribe(source, OnDealDamage);
            GameObject.Destroy(visualInstance);
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            if (timer >= data.Duration)
            {
                FindNewTarget();
                timer = 0f;
            }

            if (!visualInstance.activeSelf)
            {
                return;
            }

            if (target == null)
            {
                visualInstance.SetActive(false);
            }
            else
            {
                visualInstance.transform.position = target.transform.position;
            }
        }

        private void OnDealDamage(DamageEvent damageEvent)
        {
            if (damageEvent.Receiver.gameObject == source) return;
            if (damageEvent.Receiver.transform != target) return;

            damageEvent.Multiplier.Multiply(data.DamageMultiplier.GetValue(stacks));
        }

        private void FindNewTarget()
        {
            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.TargetLayers, useLayerMask = true };

            for (int i = 0; i < Physics2D.OverlapCircle(source.transform.position, data.SearchRadius, contactFilter, hits); i++)
            {
                if (hits[i].gameObject == source) continue;
                if (TeamManager.IsAlly(source, hits[i].gameObject)) continue;
                if (!data.ValidTypes.Contains(EntityTypeIdentifier.GetEntityType(hits[i].gameObject))) continue;

                target = hits[i].transform;
            }

            if (target != null)
            {
                visualInstance.SetActive(true);
            }
        }
    }
}