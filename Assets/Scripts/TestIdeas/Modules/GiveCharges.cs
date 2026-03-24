using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class GiveCharges : AbilityModule
    {
        [SerializeReferenceDropdown, SerializeReference] private GetAbilityStrategy strategy = new GetAbilityType();
        [SerializeField] private Stat chargesAmount;

        private ModularAbilityController controller;

        public GiveCharges(GiveCharges original)
        {
            strategy = original.strategy.GetDeepCopy();
            chargesAmount = original.chargesAmount.DeepCopy();
        }

        public override AbilityModule CreateDeepCopy() => new GiveCharges(this);

        public override void OnInitialization(ModularAbilityController controller, Transform spawn, GameObject source, ModularAbility ability)
        {
            this.controller = controller;
        }

        public override bool CanActivate() => true;

        public override bool CanDeactivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            IModularAbility ability = strategy.GetAbility(controller);
            ability?.SetCharges(ability.Charges + chargesAmount.ValueInt);
        }

        public override void Upgrade()
        {
            chargesAmount.Upgrade();
        }
    }
}