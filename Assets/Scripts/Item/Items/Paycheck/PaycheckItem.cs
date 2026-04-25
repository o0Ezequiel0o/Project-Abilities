using UnityEngine;
using static Damageable;

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
            if (source.TryGetComponent(out moneyHandler))
            {
                DamageEvent.onHit.Subscribe(source, OnHit, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (moneyHandler != null)
            {
                DamageEvent.onHit.Unsubscribe(source, OnHit);
            }
        }

        private void OnHit(DamageEvent damageEvent)
        {
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            int effectStacks = Mathf.FloorToInt(moneyHandler.Money / (data.GoldRequiredForStack * GameInstance.GoldMultiplier));
            float damageMultiplier = 1f + Mathf.Min(data.DamageMultCap.GetValue(stacks), data.DamageMultPerStack.GetValue(stacks) * effectStacks);
            damageEvent.Multiplier.Multiply(damageMultiplier);
        }
    }
}