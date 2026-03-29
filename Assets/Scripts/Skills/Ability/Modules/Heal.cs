using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class Heal : AbilityModule
    {
        [SerializeField] protected Stat amount;

        protected GameObject source;
        private Damageable damageable;

        public Heal() { }

        public Heal(Heal original)
        {
            amount = original.amount.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new Heal(this);

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
                damageable.GiveHealing(amount.Value, source, source);
            }
        }

        public override void Upgrade()
        {
            amount.Upgrade();
        }
    }
}