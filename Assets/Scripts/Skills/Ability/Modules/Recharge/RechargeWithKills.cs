using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public partial class RechargeWithKills : RechargeType
    {
        [SerializeField] private ValueType valueType;
        [SerializeField] private Stat amount;

        public RechargeWithKills() { }

        public RechargeWithKills(RechargeWithKills original)
        {
            valueType = original.valueType;
            amount = original.amount.DeepCopy();
        }

        public override RechargeType DeepCopy() => new RechargeWithKills(this);

        public override void OnInitialization(AbilityController controller, GameObject source, Ability ability)
        {
            base.OnInitialization(controller, source, ability);
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate()
        {
            Damageable.DamageEvent.onKill.Subscribe(source, OnKill);
        }

        public override void Deactivate()
        {
            Damageable.DamageEvent.onKill.Unsubscribe(source, OnKill);
        }

        public override void Upgrade()
        {
            amount.Upgrade();
        }

        public override void Destroy()
        {
            Damageable.DamageEvent.onKill.Unsubscribe(source, OnKill);
        }

        private void OnKill(Damageable.DamageEvent damageEvent)
        {
            UpdateCooldown(amount.Value, valueType);
        }
    }
}