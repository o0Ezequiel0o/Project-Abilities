using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class GiveStatusEffect : AbilityModule
    {
        [SerializeField] protected StatusEffectData statusEffect;
        [SerializeField] protected Stat stacks;

        protected GameObject source;

        private StatusEffectHandler statusEffectHandler;

        public GiveStatusEffect() { }

        public GiveStatusEffect(GiveStatusEffect original)
        {
            statusEffect = original.statusEffect;
            stacks = original.stacks.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new GiveStatusEffect(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;
            statusEffectHandler = source.GetComponent<StatusEffectHandler>();
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (statusEffectHandler == null) return;
            statusEffectHandler.ApplyEffect(statusEffect, source, stacks.ValueInt);
        }

        public override void Upgrade()
        {
            stacks.Upgrade();
        }
    }
}