using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public partial class RechargeWithKills : RechargeType
    {
        private readonly RechargeWithKillsData data;

        private readonly Stat amount;

        public RechargeWithKills(RechargeWithKillsData data, Stat amount)
        {
            this.data = data;
            this.amount = amount;
        }

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
            UpdateCooldown(amount.Value, data.ValueType);
        }
    }
}