using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class BlackBloodItem : Item
    {
        public override ItemData Data => data;
        private readonly BlackBloodItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private StatusEffectHandler statusEffectHandler;

        private float timer = 0f;
        private bool active = false;

        public BlackBloodItem(BlackBloodItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                damageable.onTakeDamage.Subscribe(OnTakeDamage, data.TriggerOrder);
            }

            statusEffectHandler = source.GetComponent<StatusEffectHandler>();
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                damageable.onTakeDamage.Unsubscribe(OnTakeDamage);
            }

            if (active) DeactivateEffect();
        }

        public override void OnUpdate()
        {
            if (active) return;

            timer += Time.deltaTime;

            if (timer > data.Cooldown.GetValue(stacks))
            {
                ActivateEffect();
            }
        }

        private void OnTakeDamage(DamageEvent damageEvent)
        {
            if (!active || damageEvent.damageRejected) return;

            damageEvent.Multiplier.Multiply(1 - data.DamageReductionRatio);
            DeactivateEffect();
        }

        private void ActivateEffect()
        {
            if (active) return;

            if (statusEffectHandler != null)
            {
                statusEffectHandler.ApplyEffect(data.DisplayEffect, source);
            }

            active = true;
            timer = 0f;
        }

        private void DeactivateEffect()
        {
            if (!active) return;

            if (statusEffectHandler != null)
            {
                statusEffectHandler.RemoveEffect(data.DisplayEffect);
            }

            active = false;
        }
    }
}