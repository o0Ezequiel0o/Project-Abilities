using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class LockAbility : AbilityModule
    {
        [SerializeReference, SerializeReferenceDropdown] private GetAbilityStrategy strategy = new GetAbilityType();

        private AbilityController controller;

        private readonly AbilityLock abilityLock = new AbilityLock();

        public LockAbility() { }

        public LockAbility(LockAbility original)
        {
            strategy = original.strategy;
        }

        public override AbilityModule DeepCopy() => new LockAbility(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.controller = controller;
            abilityLock.abilityType = strategy.GetAbility(controller).Data.AbilityType;
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            controller.RemoveAbilityLock(abilityLock);
            IAbility ability = strategy.GetAbility(controller);

            if (ability != null)
            {
                abilityLock.abilityType = ability.Data.AbilityType;
                controller.AddAbilityLock(abilityLock);
            }
        }

        public override void Deactivate()
        {
            controller.RemoveAbilityLock(abilityLock);
        }
    }
}