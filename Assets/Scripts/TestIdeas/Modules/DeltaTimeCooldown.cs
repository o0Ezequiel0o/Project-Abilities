using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class DeltaTimeCooldown : AbilityModule
    {
        private ModularAbility ability;

        public override AbilityModule CreateDeepCopy() => new DeltaTimeCooldown();

        public override void OnInitialization(ModularAbilityController controller, Transform spawn, GameObject source, ModularAbility ability)
        {
            this.ability = ability;
        }

        public override bool CanActivate() => true;

        public override bool CanDeactivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding) { }

        public override void UpdateUnactive()
        {
            ability.SetCooldownTimer(ability.CooldownTimer + Time.deltaTime);
        }
    }
}