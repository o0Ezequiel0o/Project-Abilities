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
        }

        public override void OnRemoved()
        {
            if (!hasRequiredComponents) return;
            damageable.onTakeDamage.Unsubscribe(OnTakeDamage);
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateReserveHealthCap();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateReserveHealthCap();
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
            }
        }

        private void OnTakeDamage(DamageEvent damageEvent)
        {
            if (damageEvent.SourceUser != null && damageEvent.SourceUser == source) return;

            float reducedDamage = Mathf.Min(damageEvent.Damage * data.DamageReductionRatio, reserveHealth);
            float reducedDamageRatio = reducedDamage / damageEvent.Damage;

            damageEvent.Multiplier.Multiply(1 - reducedDamageRatio);
            reserveHealth -= reducedDamage;
        }

        private void OnHealthStatChanged(StatUpdate statUpdate)
        {
            UpdateReserveHealthCap();
        }

        private void UpdateReserveHealthCap()
        {
            reserveHealthCap = damageable.MaxHealth.Value * data.HealthInheritRatio.GetValue(stacks);
            reserveHealth = Mathf.Min(reserveHealth, reserveHealthCap);
        }
    }
}