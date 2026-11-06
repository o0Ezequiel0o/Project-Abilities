using UnityEngine;

namespace Zeke.Abilities.Modules
{
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
            if (source.TryGetComponent(out DamageableEventListener eventListener))
            {
                eventListener.OnKill.Subscribe(OnKill);
            }
        }

        public override void Deactivate()
        {
            if (source.TryGetComponent(out DamageableEventListener eventListener))
            {
                eventListener.OnKill.Unsubscribe(OnKill);
            }
        }

        public override void Upgrade(int levels)
        {
            amount.Upgrade(levels);
        }

        public override void Destroy()
        {
            if (source.TryGetComponent(out DamageableEventListener eventListener))
            {
                eventListener.OnKill.Unsubscribe(OnKill);
            }
        }

        private void OnKill(Damageable.DamageEvent.OnKillEvent _)
        {
            UpdateCooldown(amount.Value, data.ValueType);
        }
    }
}