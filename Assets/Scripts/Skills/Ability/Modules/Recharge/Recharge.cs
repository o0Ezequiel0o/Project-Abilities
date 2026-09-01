using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class Recharge : AbilityModule
    {
        private readonly RechargeData data;
        private readonly RechargeType type;

        public Recharge(RechargeData data, RechargeType type)
        {
            this.data = data;
            this.type = type;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            type.OnInitialization(controller, source, ability);
        }

        public override bool CanActivate() => type.CanActivate();
        public override bool CanUpgrade() => type.CanUpgrade();

        public override void Activate(bool holding)
        {
            switch (data.UpdateMode)
            {
                case UpdateMode.Inactive:
                    type.Deactivate();
                    break;

                case UpdateMode.Active:
                    type.Activate();
                    break;

                default:
                    break;
            }
        }

        public override void Deactivate()
        {
            switch (data.UpdateMode)
            {
                case UpdateMode.Inactive:
                    type.Activate();
                    break;

                case UpdateMode.Active:
                    type.Deactivate();
                    break;

                default:
                    break;
            }
        }

        public override void UpdateActive()
        {
            if (data.UpdateMode == UpdateMode.Active)
            {
                type.UpdateDuration();
            }
        }

        public override void UpdateInactive()
        {
            if (data.UpdateMode == UpdateMode.Inactive)
            {
                type.UpdateDuration();
            }
        }

        public override void Upgrade()
        {
            type.Upgrade();
        }

        public override void Destroy()
        {
            type.Destroy();
        }

        public enum UpdateMode
        {
            Inactive,
            Active
        }
    }
}