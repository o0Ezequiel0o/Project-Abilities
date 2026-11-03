using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class HealData : AbilityModuleData
    {
        [SerializeField] protected Stat amount;
        [field: SerializeField] public float ProcCoefficient { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new Heal(this, amount.DeepCopy());
        }
    }
}