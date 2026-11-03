using UnityEngine;

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

            if (strategy.TryGetAbilitySlot(controller, out AbilitySlotInfo abilityTypeSlot))
            {
                abilityLock.abilityType = abilityTypeSlot.abilityTypeSlot.AbilityType;
            }
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            controller.RemoveAbilityLock(abilityLock);

            if (strategy.TryGetAbilitySlot(controller, out AbilitySlotInfo abilityTypeSlot))
            {
                abilityLock.abilityType = abilityTypeSlot.abilityTypeSlot.AbilityType;
                controller.AddAbilityLock(abilityLock);
            }
        }

        public override void Deactivate()
        {
            controller.RemoveAbilityLock(abilityLock);
        }
    }
}