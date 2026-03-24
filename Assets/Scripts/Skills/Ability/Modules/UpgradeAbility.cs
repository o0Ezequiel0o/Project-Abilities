using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class UpgradeAbility : AbilityModule
    {
        [SerializeReference, SerializeReferenceDropdown] private GetAbilityStrategy strategy = new GetAbilityType();
        [SerializeField] private int levels;

        private AbilityController controller;

        public UpgradeAbility(UpgradeAbility original)
        {
            levels = original.levels;
            strategy = original.strategy.GetDeepCopy();
        }

        public override AbilityModule DeepCopy() => new UpgradeAbility(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.controller = controller;
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            for (int i = 0; i < levels; i++)
            {
                strategy.GetAbility(controller)?.QueueUpgrade();
            }
        }
    }
}