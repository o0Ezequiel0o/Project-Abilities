using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Items
{
    public class ThiefBootsItem : Item
    {
        public override ItemData Data => data;
        private readonly ThiefBootsItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private EntityMove entityMove;

        private bool active = false;
        private float timer = 0f;

        private readonly List<Collider2D> hits = new List<Collider2D>();
        private readonly Stat.Multiplier moveSpeedMultiplier = new Stat.Multiplier();

        private bool hasRequiredComponents = false;

        public ThiefBootsItem(ThiefBootsItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            hasRequiredComponents = source.TryGetComponent(out entityMove);
        }

        public override void OnRemoved()
        {
            if (active)
            {
                entityMove.MoveSpeed.RemoveMultiplier(moveSpeedMultiplier);
            }
        }

        public override void OnStacksAdded(int amount)
        {
            moveSpeedMultiplier.UpdateMultiplier(data.SpeedMultiplier.GetValue(stacks));
        }

        public override void OnStacksRemoved(int amount)
        {
            moveSpeedMultiplier.UpdateMultiplier(data.SpeedMultiplier.GetValue(stacks));
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            if (!hasRequiredComponents) return;
            if (timer < data.RequiredTime) return;

            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(source.transform.position, data.RequiredRadius, contactFilter, hits);

            bool detectedEnemies = false;

            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].gameObject == source) continue;
                if (TeamManager.IsAlly(source, hits[i].gameObject)) continue;

                detectedEnemies = true;
                break;
            }

            if (detectedEnemies && active)
            {
                DeactivateEffect();
            }
            if (!detectedEnemies && !active)
            {
                ActivateEffect();
            }

            moveSpeedMultiplier.UpdateMultiplier(data.SpeedMultiplier.GetValue(stacks));
        }

        private void ActivateEffect()
        {
            entityMove.MoveSpeed.AddMultiplier(moveSpeedMultiplier);
            active = true;
        }

        private void DeactivateEffect()
        {
            entityMove.MoveSpeed.RemoveMultiplier(moveSpeedMultiplier);
            active = false;
            timer = 0f;
        }
    }
}