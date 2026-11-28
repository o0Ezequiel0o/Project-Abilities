using UnityEngine;

namespace Zeke.Items
{
    public class WreckingBallItem : Item
    {
        public override ItemData Data => data;
        private readonly WreckingBallItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Physics physics;
        private Damageable damageable;

        private float flatModifier = 0f;

        private bool hasRequiredComponents = false;

        public WreckingBallItem(WreckingBallItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (!source.TryGetComponent(out physics)) return;
            if (!source.TryGetComponent(out damageable)) return;

            hasRequiredComponents = true;
        }

        public override void OnRemoved()
        {
            damageable.Armor.ApplyFlatModifier(-flatModifier);
        }

        public override void OnUpdate()
        {
            if (!hasRequiredComponents) return;

            float speed = physics.Velocity.magnitude;
            float armor = data.ArmorPerMeter.GetValue(stacks);

            float oldModifier = flatModifier;
            flatModifier = armor * speed;

            damageable.Armor.ApplyFlatModifier(-oldModifier, flatModifier);
        }
    }
}