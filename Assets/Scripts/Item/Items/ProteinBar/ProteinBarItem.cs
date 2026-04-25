using UnityEngine;

namespace Zeke.Items
{
    public class ProteinBarItem : Item
    {
        public override ItemData Data => data;
        private readonly ProteinBarItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;

        private float accumulatedDistance = 0f;
        private Vector3 lastPosition = Vector3.zero;

        private bool hasRequiredComponents = false;

        public ProteinBarItem(ProteinBarItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            hasRequiredComponents = source.TryGetComponent(out damageable);
            lastPosition = source.transform.position;
        }

        public override void OnUpdate()
        {
            if (!hasRequiredComponents) return;

            float distance = Vector2.Distance(lastPosition, source.transform.position);
            accumulatedDistance += distance;

            if (accumulatedDistance > data.DistanceRequired)
            {
                damageable.GiveHealing(data.Healing.GetValue(stacks), source, source);
                accumulatedDistance = 0f;
            }

            lastPosition = source.transform.position;
        }
    }
}