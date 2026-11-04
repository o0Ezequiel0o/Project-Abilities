using UnityEngine;

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
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnHit.Subscribe(OnHit, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnHit.Unsubscribe(OnHit);
            }
        }

        private void OnHit(Damageable.DamageEvent.OnHitEvent hitEvent)
        {
            Damageable.DamageEvent damageEvent = hitEvent.damageEvent;

            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;
            float missingHealthRatio = 1f - (damageEvent.Receiver.Health / damageEvent.Receiver.MaxHealth.Value);

            int effectStacks = Mathf.FloorToInt(missingHealthRatio / data.MissingHealthRatioForStack);
            damageEvent.Multiplier.Multiply(1f + (effectStacks * data.DamageMultPerStack.GetValue(stacks)));
        }
    }
}