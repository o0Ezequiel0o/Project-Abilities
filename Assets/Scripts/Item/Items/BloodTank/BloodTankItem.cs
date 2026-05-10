using UnityEngine;
using static Stat;
using static Damageable;

namespace Zeke.Items
{
    public class BloodTankItem : Item
    {
        public override ItemData Data => data;
        private readonly BloodTankItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;
        private StatusEffectHandler statusEffectHandler;

        private float reserveHealth = 0f;
        private float reserveHealthCap = 0f;

        private float timer = 0f;

        private bool hasRequiredComponents = false;

        public BloodTankItem(BloodTankItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            hasRequiredComponents = source.TryGetComponent(out damageable);

            if (hasRequiredComponents)
            {
                damageable.MaxHealth.onStatUpdated += OnHealthStatChanged;
                damageable.onTakeDamage.Subscribe(OnTakeDamage, data.TriggerOrder);
            }

            statusEffectHandler = source.GetComponent<StatusEffectHandler>();
        }

        public override void OnRemoved()
        {
            if (!hasRequiredComponents) return;
            damageable.onTakeDamage.Unsubscribe(OnTakeDamage);

            if (statusEffectHandler != null)
            {
                statusEffectHandler.RemoveEffect(data.IndicatorEffect);
            }
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateReserveHealthCap();
            UpdateDisplayer();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateReserveHealthCap();
            UpdateDisplayer();
        }

        public override void OnUpdate()
        {
            if (!hasRequiredComponents) return;

            timer += Time.deltaTime;

            if (timer >= damageable.Settings.RegenInterval)
            {
                float regen = damageable.HealthRegen.Value * timer;
                reserveHealth = Mathf.Min(reserveHealth + regen, reserveHealthCap);

                timer = 0f;
                UpdateDisplayer();
            }
        }

        private void OnTakeDamage(DamageEvent damageEvent)
        {
            if (damageEvent.SourceUser != null && damageEvent.SourceUser == source) return;

            float reducedDamage = Mathf.Min(damageEvent.Damage * data.DamageReductionRatio, reserveHealth);
            float reducedDamageRatio = reducedDamage / damageEvent.Damage;

            damageEvent.Multiplier.Multiply(1 - reducedDamageRatio);
            reserveHealth -= reducedDamage;

            UpdateDisplayer();
        }

        private void OnHealthStatChanged(StatUpdate statUpdate)
        {
            UpdateReserveHealthCap();
        }

        private void UpdateReserveHealthCap()
        {
            reserveHealthCap = damageable.MaxHealth.Value * data.HealthInheritRatio.GetValue(stacks);
            reserveHealth = Mathf.Min(reserveHealth, reserveHealthCap);
            UpdateDisplayer();
        }

        private void UpdateDisplayer()
        {
            if (statusEffectHandler == null) return;

            if (!statusEffectHandler.TryGetActiveStatusEffect(data.IndicatorEffect, out StatusEffect statusEffect))
            {
                if (reserveHealth == 0f) return;

                statusEffect = statusEffectHandler.ApplyEffect(data.IndicatorEffect, source, 1);
            }

            int stacks = statusEffect.stacks;
            int targetStacks = Mathf.CeilToInt(reserveHealth);

            int changeTarget = targetStacks - stacks;

            if (changeTarget > 0)
            {
                statusEffectHandler.ApplyEffect(data.IndicatorEffect, source, Mathf.Abs(changeTarget));
            }
            else if (changeTarget < 0)
            {
                statusEffectHandler.RemoveEffect(data.IndicatorEffect, Mathf.Abs(changeTarget));
            }
        }
    }
}