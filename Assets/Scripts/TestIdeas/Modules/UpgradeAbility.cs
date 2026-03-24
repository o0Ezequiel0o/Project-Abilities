using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class UpgradeAbility : AbilityModule
    {
        [SerializeReference, SerializeReferenceDropdown] private GetAbilityStrategy strategy = new GetAbilityType();
        [SerializeField] private int levels;

        private ModularAbilityController controller;

        public UpgradeAbility(UpgradeAbility original)
        {
            levels = original.levels;
            strategy = original.strategy.GetDeepCopy();
        }

        public override AbilityModule CreateDeepCopy() => new UpgradeAbility(this);

        public override void OnInitialization(ModularAbilityController controller, Transform spawn, GameObject source, ModularAbility ability)
        {
            this.controller = controller;
        }

        public override bool CanActivate() => true;

        public override bool CanDeactivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            for (int i = 0; i < levels; i++)
            {
                strategy.GetAbility(controller)?.Upgrade();
            }
        }
    }
}