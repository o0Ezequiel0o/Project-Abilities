using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class Recharge : AbilityModule
    {
        [SerializeField] private UpdateMode updateMode;
        [SerializeReferenceDropdown, SerializeReference] private RechargeType type = new RechargeWithTime();

        public Recharge() { }

        public Recharge(Recharge original)
        {
            type = original.type.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new Recharge(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            type.OnInitialization(controller, source, ability);
        }

        public override bool CanActivate() => type.CanActivate();
        public override bool CanUpgrade() => type.CanUpgrade();

        public override void Activate(bool holding)
        {
            switch (updateMode)
            {
                case UpdateMode.Unactive:
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
            switch (updateMode)
            {
                case UpdateMode.Unactive:
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
            if (updateMode == UpdateMode.Active)
            {
                type.UpdateDuration();
            }
        }

        public override void UpdateUnactive()
        {
            if (updateMode == UpdateMode.Unactive)
            {
                type.UpdateDuration();
            }
        }

        public override void Update()
        {
            type.UpdateDuration();
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
            Unactive,
            Active
        }
    }
}