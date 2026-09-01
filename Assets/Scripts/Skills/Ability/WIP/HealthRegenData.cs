using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class HealthRegenData : AbilityModuleData
    {
        [SerializeField] protected Stat amount;
        [SerializeField] protected Stat interval;
        [field: SerializeField] public float ProcCoefficient { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new HealthRegen(this, amount.DeepCopy(), interval.DeepCopy());
        }
    }
}