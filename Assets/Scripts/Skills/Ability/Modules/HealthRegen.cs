using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class HealthRegen : AbilityModule
    {
        [SerializeField] protected Stat amount;
        [SerializeField] protected Stat interval;

        protected GameObject source;
        private Damageable damageable;

        private float timer = 0f;

        public HealthRegen() { }

        public HealthRegen(HealthRegen original)
        {
            amount = original.amount.DeepCopy();
            interval = original.interval.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new HealthRegen(this);

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
                damageable.GiveHealing(amount.Value, source, source);
            }
        }

        public override void Upgrade()
        {
            amount.Upgrade();
            interval.Upgrade();
        }
    }
}