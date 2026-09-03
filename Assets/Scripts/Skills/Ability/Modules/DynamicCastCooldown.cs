using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class DynamicCastCooldown : AbilityModule
    {
        private readonly DynamicCastCooldownData data;

        private readonly Stat cooldown;

        private float CooldownTime => cooldown.Value * controller.Abilities[controller.GetAbilityType(ability.Data)].CooldownMultiplier.Value;
        private float RechargeSpeed => controller.Abilities[controller.GetAbilityType(ability.Data)].RechargeSpeed.Value;

        private AbilityController controller;
        private Ability ability;

        private float timer = 0f;

        public DynamicCastCooldown(DynamicCastCooldownData data, Stat cooldown)
        {
            this.data = data;
            this.cooldown = cooldown;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.controller = controller;
            this.ability = ability;
            timer = cooldown.Value;
        }

        public override bool CanActivate() => timer > CooldownTime;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            timer = 0f;
        }

        public override void Update()
        {
            timer += Time.deltaTime * RechargeSpeed;
        }

        public override void Upgrade()
        {
            cooldown.Upgrade();
        }
    }
}