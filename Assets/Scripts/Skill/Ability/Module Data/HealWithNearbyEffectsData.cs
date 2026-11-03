using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class HealWithNearbyEffectsData : AbilityModuleData
    {
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public bool ConsumesEffects { get; private set; } = true;
        [field: SerializeField] public List<StatusEffectData> Effects { get; private set; }

        [Space]

        [SerializeField] private Stat healingPerStack;
        [field: SerializeField] public float ProcCoefficient { get; private set; }
        [SerializeField] private Stat radius;

        public override AbilityModule CreateModule()
        {
            return new HealWithNearbyEffects(this, healingPerStack.DeepCopy(), radius.DeepCopy());
        }
    }
}