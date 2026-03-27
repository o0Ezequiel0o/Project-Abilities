using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class CastCooldown : AbilityModule
    {
        [SerializeField] private Stat cooldown;

        private float timer = 0f;

        public CastCooldown() { }

        public CastCooldown(CastCooldown original)
        {
            cooldown = original.cooldown.DeepCopy();
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            timer = cooldown.Value;
        }

        public override AbilityModule DeepCopy() => new CastCooldown(this);

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

        public override void Upgrade()
        {
            cooldown.Upgrade();
        }
    }
}