using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class UpgradePassive: AbilityModule
    {
        private readonly UpgradePassiveData data;

        private PassiveController passiveController;

        private bool hasRequiredComponents = true;

        public UpgradePassive(UpgradePassiveData data)
        {
            this.data = data;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            if (!source.TryGetComponent(out passiveController)) hasRequiredComponents = false;
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (!hasRequiredComponents) return;

            if (passiveController.TryGetPassive(data.Passive, out IPassive passiveInstance))
            {
                for (int i = 0; i < data.Levels; i++)
                {
                    passiveInstance.Upgrade();
                }
            }
        }
    }
}