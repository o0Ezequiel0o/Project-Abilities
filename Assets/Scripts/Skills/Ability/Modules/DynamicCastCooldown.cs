using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class DynamicCastCooldown : AbilityModule
    {
        [SerializeField] private Stat cooldown = new Stat(0.05f, 0f, 0f, float.PositiveInfinity);

        private float CooldownTime => cooldown.Value * controller.cooldownMultiplier[ability.Data.AbilityType].Value;
        private float RechargeSpeed => controller.rechargeSpeed[ability.Data.AbilityType].Value;

        private AbilityController controller;
        private Ability ability;

        private float timer = 0f;

        public DynamicCastCooldown() { }

        public DynamicCastCooldown(DynamicCastCooldown original)
        {
            cooldown = original.cooldown.DeepCopy();
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.controller = controller;
            this.ability = ability;
            timer = cooldown.Value;
        }

        public override AbilityModule DeepCopy() => new DynamicCastCooldown(this);

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