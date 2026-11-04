using UnityEngine;

namespace Zeke.Items
{
    public class GlassShardsItem : Item
    {
        public override ItemData Data => data;
        private readonly GlassShardsItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public GlassShardsItem(GlassShardsItemData data, ItemHandler itemHandler, GameObject source)
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

            if (!RollProc(data.ProcChance.GetValue(stacks), damageEvent.ProcCoefficient, itemHandler.Luck.ValueInt)) return;
            if (damageEvent.Receiver != null && damageEvent.Receiver.gameObject == source) return;

            if (damageEvent.Receiver.TryGetComponent(out StatusEffectHandler statusEffectHandler))
            {
                statusEffectHandler.ApplyEffect(data.StatusEffect, source);
            }
        }
    }
}