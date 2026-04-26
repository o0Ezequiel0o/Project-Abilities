using UnityEngine;

namespace Zeke.Items
{
    public class HaloItem : Item
    {
        public override ItemData Data => data;
        private readonly HaloItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private readonly Stat.Multiplier healthReceivedMultiplier = new Stat.Multiplier(1f);

        public HaloItem(HaloItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                damageable.HealingReceivedMultiplier.AddMultiplier(healthReceivedMultiplier);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                damageable.HealingReceivedMultiplier.RemoveMultiplier(healthReceivedMultiplier);
            }
        }

        public override void OnStacksAdded(int amount)
        {
            healthReceivedMultiplier.UpdateMultiplier(data.HealthReceivedMult.GetValue(stacks));
        }

        public override void OnStacksRemoved(int amount)
        {
            healthReceivedMultiplier.UpdateMultiplier(data.HealthReceivedMult.GetValue(stacks));
        }
    }
}