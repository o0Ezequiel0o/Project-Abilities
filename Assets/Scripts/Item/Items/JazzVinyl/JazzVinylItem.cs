using UnityEngine;

namespace Zeke.Items
{
    public class JazzVinylItem : Item
    {
        public override ItemData Data => data;
        private readonly JazzVinylItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public JazzVinylItem(JazzVinylItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            Damageable.DamageEvent.onHit.Subscribe(source, OnHit, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            Damageable.DamageEvent.onHit.Unsubscribe(source, OnHit);
        }

        private void OnHit(Damageable.DamageEvent damageEvent)
        {
            if (!RollProc(data.ProcChance.GetValue(stacks), damageEvent.ProcCoefficient, itemHandler.Luck.ValueInt)) return;
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            if (damageEvent.Receiver.TryGetComponent(out StatusEffectHandler statusEffectHandler))
            {
                statusEffectHandler.ApplyEffect(data.StatusEffect, source);
            }
        }
    }
}