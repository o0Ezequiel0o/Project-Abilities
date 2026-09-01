using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class Heal : AbilityModule
    {
        private readonly HealData data;

        protected readonly Stat amount;

        protected GameObject source;
        private Damageable damageable;

        public Heal(HealData data, Stat amount)
        {
            this.data = data;
            this.amount = amount;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;
            damageable = source.GetComponent<Damageable>();
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
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
        }
    }
}