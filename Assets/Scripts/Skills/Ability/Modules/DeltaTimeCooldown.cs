using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class DeltaTimeCooldown : AbilityModule
    {
        private Ability ability;

        public override AbilityModule DeepCopy() => new DeltaTimeCooldown();

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.ability = ability;
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding) { }

        public override void UpdateUnactive()
        {
            ability.SetCooldownTimer(ability.CooldownTimer + Time.deltaTime);
        }
    }
}