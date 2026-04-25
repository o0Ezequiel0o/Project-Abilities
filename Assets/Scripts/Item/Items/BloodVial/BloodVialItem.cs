using UnityEngine;

namespace Zeke.Items
{
    public class BloodVialItem : Item
    {
        public override ItemData Data => data;
        private readonly BloodVialItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;

        private float timer = 0f;

        private bool hasRequiredComponents = false;

        public BloodVialItem(BloodVialItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            hasRequiredComponents = source.TryGetComponent(out damageable);
        }

        public override void OnUpdate()
        {
            if (!hasRequiredComponents) return;

            timer += Time.deltaTime;

            if (timer > data.Cooldown)
            {
                damageable.GiveHealing(data.Healing.GetValue(stacks), source, source);

                timer = 0f;
            }
        }
    }
}