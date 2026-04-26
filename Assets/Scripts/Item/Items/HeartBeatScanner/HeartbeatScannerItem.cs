using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class HeartbeatScannerItem : Item
    {
        public override ItemData Data => data;
        private readonly HeartbeatScannerItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public HeartbeatScannerItem(HeartbeatScannerItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            DamageEvent.onHit.Subscribe(source, OnHit, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            DamageEvent.onHit.Unsubscribe(source, OnHit);
        }

        private void OnHit(DamageEvent damageEvent)
        {
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;
            float missingHealthRatio = 1f - (damageEvent.Receiver.Health / damageEvent.Receiver.MaxHealth.Value);

            int effectStacks = Mathf.FloorToInt(missingHealthRatio / data.MissingHealthRatioForStack);
            damageEvent.Multiplier.Multiply(1f + (effectStacks * data.DamageMultPerStack.GetValue(stacks)));
        }
    }
}