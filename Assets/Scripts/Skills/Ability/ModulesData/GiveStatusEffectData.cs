using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class GiveStatusEffectData : AbilityModuleData
    {
        [field: SerializeField] public StatusEffectData StatusEffect { get; private set; }
        [SerializeField] protected Stat stacks;

        public override AbilityModule CreateModule()
        {
            return new GiveStatusEffect(this, stacks.DeepCopy());
        }
    }
}