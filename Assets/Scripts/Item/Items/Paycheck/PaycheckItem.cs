using UnityEngine;

namespace Zeke.Items
{
    public class PaycheckItem : Item
    {
        public override ItemData Data => data;
        private readonly PaycheckItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private MoneyHandler moneyHandler;

        public PaycheckItem(PaycheckItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out moneyHandler) && source.TryGetComponent(out DamageEventListener eventListener))
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

            int effectStacks = Mathf.FloorToInt(moneyHandler.Money / data.GoldRequiredForStack);
            float damageMultiplier = 1f + Mathf.Min(data.DamageMultCap.GetValue(stacks), data.DamageMultPerStack.GetValue(stacks) * effectStacks);
            damageEvent.Multiplier.Multiply(damageMultiplier);
        }
    }
}