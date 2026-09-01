using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class UpgradeAbility : AbilityModule
    {
        private readonly UpgradeAbilityData data;

        private readonly GetAbilityStrategy strategy;

        private AbilityController controller;

        public UpgradeAbility(UpgradeAbilityData data, GetAbilityStrategy strategy)
        {
            this.data = data;
            this.strategy = strategy;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.controller = controller;
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            for (int i = 0; i < data.Levels; i++)
            {
                strategy.GetAbility(controller)?.QueueUpgrade();
            }
        }
    }
}