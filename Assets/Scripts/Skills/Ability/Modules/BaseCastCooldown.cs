using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class BaseCastCooldown : AbilityModule
    {
        [SerializeField] private Stat cooldown = new Stat(0.05f, 0f, 0f, float.PositiveInfinity);

        private float timer = 0f;

        public BaseCastCooldown() { }

        public BaseCastCooldown(BaseCastCooldown original)
        {
            cooldown = original.cooldown.DeepCopy();
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            timer = cooldown.Value;
        }

        public override AbilityModule DeepCopy() => new BaseCastCooldown(this);

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