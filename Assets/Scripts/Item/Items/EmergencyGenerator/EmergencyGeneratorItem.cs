using UnityEngine;

namespace Zeke.Items
{
    public class EmergencyGeneratorItem : Item
    {
        public override ItemData Data => data;
        private readonly EmergencyGeneratorItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;

        private bool hasRequiredComponents = false;

        private float timer = 0f;

        public EmergencyGeneratorItem(EmergencyGeneratorItemData data, ItemHandler itemHandler, GameObject source)
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

            if (timer >= data.Interval)
            {
                damageable.GiveShield(data.Amount.GetValue(stacks), source, source);
                timer = 0f;
            }
        }
    }
}