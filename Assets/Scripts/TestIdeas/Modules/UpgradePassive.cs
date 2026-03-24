using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class UpgradePassive: AbilityModule
    {
        [SerializeReference] private PassiveData passive;
        [SerializeField] private int levels;

        private PassiveController passiveController;

        private bool hasRequiredComponents = true;

        public UpgradePassive(UpgradePassive original)
        {
            passive = original.passive;
            levels = original.levels;
        }

        public override AbilityModule CreateDeepCopy() => new UpgradePassive(this);

        public override void OnInitialization(ModularAbilityController controller, Transform spawn, GameObject source, ModularAbility ability)
        {
            if (!source.TryGetComponent(out passiveController)) hasRequiredComponents = false;
        }

        public override bool CanActivate() => true;

        public override bool CanDeactivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (!hasRequiredComponents) return;

            if (passiveController.TryGetPassive(passive, out IPassive passiveInstance))
            {
                for (int i = 0; i < levels; i++)
                {
                    passiveInstance.Upgrade();
                }
            }
        }
    }
}