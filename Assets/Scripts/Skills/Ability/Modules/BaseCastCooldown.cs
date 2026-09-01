using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class BaseCastCooldown : AbilityModule
    {
        private readonly BaseCastCooldownData data;

        private readonly Stat cooldown;

        private float timer = 0f;

        public BaseCastCooldown(BaseCastCooldownData data, Stat cooldown)
        {
            this.data = data;
            this.cooldown = cooldown;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            timer = cooldown.Value;
        }

        public override bool CanActivate() => timer > cooldown.Value;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            timer = 0f;
        }

        public override void Update()
        {
            timer += Time.deltaTime;
        }

        public override void Upgrade()
        {
            cooldown.Upgrade();
        }
    }
}