using UnityEngine;

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
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnHit.Subscribe(OnHit, data.ProcPriority);
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

            if (!RollProc(data.Chance.GetValue(stacks), damageEvent.ProcCoefficient, itemHandler.Luck.ValueInt)) return;
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            damageEvent.Multiplier.Multiply(data.DamageMultiplier);
        }
    }
}