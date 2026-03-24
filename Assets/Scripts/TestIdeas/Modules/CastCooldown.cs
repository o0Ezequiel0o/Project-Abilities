using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class CastCooldown : AbilityModule
    {
        [SerializeField] private Stat cooldown;

        private float timer = 0f;

        public CastCooldown(CastCooldown original)
        {
            cooldown = original.cooldown.DeepCopy();
        }

        public override void OnInitialization(ModularAbilityController controller, Transform spawn, GameObject source, ModularAbility ability)
        {
            timer = cooldown.Value;
        }

        public override AbilityModule CreateDeepCopy() => new CastCooldown(this);

        public override bool CanDeactivate() => true;

        public override bool CanActivate()
        {
            return timer > cooldown.Value;
        }

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            timer = 0f;
        }

        public override void Update()
        {
            timer += Time.deltaTime;
        }
    }
}