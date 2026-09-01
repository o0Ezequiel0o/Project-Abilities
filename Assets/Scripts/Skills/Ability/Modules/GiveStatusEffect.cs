using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class GiveStatusEffect : AbilityModule
    {
        private readonly GiveStatusEffectData data;

        protected readonly Stat stacks;
        protected GameObject source;

        private StatusEffectHandler statusEffectHandler;

        public GiveStatusEffect(GiveStatusEffectData data, Stat stacks)
        {
            this.data = data;
            this.stacks = stacks;
        }

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
            statusEffectHandler.ApplyEffect(data.StatusEffect, source, stacks.ValueInt);
        }

        public override void Upgrade()
        {
            stacks.Upgrade();
        }
    }
}