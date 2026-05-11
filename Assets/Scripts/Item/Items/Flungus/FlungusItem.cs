using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Items
{
    public class FlungusItem : Item
    {
        public override ItemData Data => data;
        private readonly FlungusItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Vector3 lastPosition;
        private GameObject particlesInstance;

        private float delayTimer = 0f;
        private float healTimer = 0f;

        private bool active = false;

        private readonly List<Collider2D> hits = new List<Collider2D>();

        public FlungusItem(FlungusItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            lastPosition = source.transform.position;
            particlesInstance = GameObject.Instantiate(data.Particles, source.transform.position, Quaternion.identity);
            particlesInstance.SetActive(false);
        }

        public override void OnRemoved()
        {
            GameObject.Destroy(particlesInstance);
        }

        public override void OnStacksAdded(int amount)
        {
            float diameter = data.Radius.GetValue(stacks) * 2f;
            particlesInstance.transform.localScale = new Vector3(diameter, diameter, 1f);
        }

        public override void OnUpdate()
        {
            if (lastPosition == source.transform.position)
            {
                delayTimer += Time.deltaTime;

                if (active)
                {
                    UpdateHealing();
                }
                else if (delayTimer > data.ActivateDelay)
                {
                    Activate();
                }
            }
            else
            {
                delayTimer = 0f;
                healTimer = 0f;

                if (active)
                {
                    Deactivate();
                }
            }

            lastPosition = source.transform.position;
        }

        private void Deactivate()
        {
            active = false;
            particlesInstance.SetActive(false);
        }

        private void UpdateHealing()
        {
            healTimer += Time.deltaTime;

            if (healTimer > data.HealCooldown)
            {
                float healing = data.Healing.GetValue(stacks);

                ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
                for (int i = 0; i < Physics2D.OverlapCircle(source.transform.position, data.Radius.GetValue(stacks), contactFilter, hits); i++)
                {
                    if (TeamManager.IsEnemy(hits[i].gameObject, source)) continue;

                    if (hits[i].TryGetComponent(out Damageable damageable))
                    {
                        damageable.GiveHealing(healing, source, source);
                    }
                }

                healTimer = 0f;
            }
        }

        private void Activate()
        {
            active = true;
            particlesInstance.SetActive(true);
            particlesInstance.transform.position = source.transform.position;
        }
    }
}