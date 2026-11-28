using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class AreaHealData : HealData
    {
        [SerializeField] private Stat radius;
        [field: SerializeField] public LayerMask HitLayers { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new AreaHeal(this, amount.DeepCopy(), radius.DeepCopy());
        }
    }
}