using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class WeakPointAnalyzerItem : Item
    {
        public override ItemData Data => data;
        private readonly WeakPointAnalyzerItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public WeakPointAnalyzerItem(WeakPointAnalyzerItemData data, ItemHandler itemHandler, GameObject source)
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
            if (!RollProc(data.Chance.GetValue(stacks), damageEvent.ProcCoefficient, itemHandler.Luck)) return;
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            damageEvent.Multiplier.Multiply(data.DamageMultiplier);
        }
    }
}