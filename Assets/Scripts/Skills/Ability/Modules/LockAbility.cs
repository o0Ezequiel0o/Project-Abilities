using UnityEngine;

using static Zeke.Abilities.AbilityController;

namespace Zeke.Abilities.Modules
{
    public class LockAbility : AbilityModule
    {
        private readonly LockAbilityData data;

        private readonly GetAbilityStrategy strategy;

        private AbilityController controller;

        private readonly AbilityLock abilityLock = new AbilityLock();

        public LockAbility(LockAbilityData data, GetAbilityStrategy strategy)
        {
            this.data = data;
            this.strategy = strategy;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.controller = controller;
            AbilitySlot abilitySlot = strategy.GetAbilitySlot(controller);

            if (abilitySlot != null)
            {
                abilityLock.abilityType = abilitySlot.Ability.Type;
            }
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            controller.RemoveAbilityLock(abilityLock);

            AbilitySlot abilitySlot = strategy.GetAbilitySlot(controller);

            if (abilitySlot != null)
            {
                abilityLock.abilityType = abilitySlot.AbilityType;
                controller.AddAbilityLock(abilityLock);
            }
        }

        public override void Deactivate()
        {
            controller.RemoveAbilityLock(abilityLock);
        }
    }
}