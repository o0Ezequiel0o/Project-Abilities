using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class HealthRegen : AbilityModule
    {
        private readonly HealthRegenData data;

        protected readonly Stat amount;
        protected readonly Stat interval;

        protected GameObject source;
        private Damageable damageable;

        private float timer = 0f;

        public HealthRegen(HealthRegenData data, Stat amount, Stat interval)
        {
            this.data = data;
            this.amount = amount;
            this.interval = interval;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;
            damageable = source.GetComponent<Damageable>();
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding) { }

        public override void UpdateActive()
        {
            timer += Time.deltaTime;

            if (timer > interval.Value)
            {
                OnHealthRegenTick();
                timer = 0f;
            }
        }

        public virtual void OnHealthRegenTick()
        {
            if (damageable != null)
            {
                HealInfo heal = new HealInfo(amount.Value, data.ProcCoefficient);
                damageable.GiveHealing(heal, source, source);
            }
        }

        public override void Upgrade()
        {
            amount.Upgrade();
            interval.Upgrade();
        }
    }
}