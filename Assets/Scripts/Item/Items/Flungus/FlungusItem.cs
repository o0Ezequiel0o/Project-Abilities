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

        public FlungusItem(FlungusItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnAdded()
        {
            lastPosition = source.transform.position;
            particlesInstance = GameObject.Instantiate(data.Particles, source.transform.position, Quaternion.identity);
            particlesInstance.SetActive(false);
        }

        public override void OnRemoved()
        {
            GameObject.Destroy(particlesInstance);
        }

        public override void OnStackAdded()
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
                Collider2D[] hits = Physics2D.OverlapCircleAll(source.transform.position, data.Radius.GetValue(stacks), data.HitLayers);

                float healing = data.Healing.GetValue(stacks);

                for (int i = 0; i < hits.Length; i++)
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