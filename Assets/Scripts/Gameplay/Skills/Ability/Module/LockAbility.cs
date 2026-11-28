using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class LockAbility : AbilityModule
    {
        private readonly LockAbilityData data;

        private AbilityController controller;

        private readonly AbilityLock abilityLock = new AbilityLock();

        public LockAbility(LockAbilityData data)
        {
            this.data = data;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.controller = controller;
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            controller.RemoveAbilityLock(abilityLock);
            controller.AddAbilityLock(abilityLock);
        }

        public override void Deactivate()
        {
            controller.RemoveAbilityLock(abilityLock);
        }
    }
}