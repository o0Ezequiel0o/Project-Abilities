using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class AreaHealthRegenData : HealthRegenData
    {
        [SerializeField] private Stat radius;
        [field: SerializeField] public LayerMask HitLayers { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new AreaHealthRegen(this, amount.DeepCopy(), interval.DeepCopy(), radius.DeepCopy());
        }
    }
}