using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class AreaGiveStatusEffectData : GiveStatusEffectData
    {
        [SerializeField] private Stat radius;
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public AreaGiveStatusEffect.TargetingType Targeting { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new AreaGiveStatusEffect(this, stacks.DeepCopy(), radius.DeepCopy());
        }
    }
}